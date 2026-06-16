using System;
using System.IO;
#if NET8_0_OR_GREATER
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;
#endif

namespace RhythmBase.Global.Components
{

	public class JsonCompactStream : Stream
	{
		private const string SeekNotSupported = "This stream does not support seeking.";
		private const string WriteNotSupported = "This stream does not support writing.";
		private const int _overlapSize = 5;

		private readonly Stream _baseStream;
		private readonly byte[] _buffer;
		private readonly int _windowSize;
		private readonly bool _leaveOpen;
		private bool _disposed;

		private int _scanEnd;
		private int _scanIdx;
		private bool _isEndOfStream;

		private enum ParserState : byte
		{
			Normal,
			InString,
			InEscape,
			InKeyword,
			InValue,
		}

		private ParserState _parserState;
		private int _nestDepth;
		private string _currentKeyword = "";
		private int _kwIndex;

		private readonly bool _allowNewlinesInStrings;
		private readonly bool _lenientComma;

		private bool _lastWasValue;
		private int _pendingByte = -1;

		public override bool CanRead => !_disposed;
		public override bool CanSeek => false;
		public override bool CanWrite => false;

		public override long Length => throw new NotSupportedException(SeekNotSupported);
		public override long Position
		{
			get => throw new NotSupportedException(SeekNotSupported);
			set => throw new NotSupportedException(SeekNotSupported);
		}

		public JsonCompactStream(
			Stream stream,
			int bufferSize = 8192,
			bool leaveOpen = false,
			bool allowNewlinesInStrings = false,
			bool lenientComma = false)
		{
			ArgumentNullException.ThrowIfNull(stream, nameof(stream));
			if (bufferSize < 1) throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size must be greater than 0.");

			_baseStream = stream;
			_windowSize = bufferSize;
			_leaveOpen = leaveOpen;
			_allowNewlinesInStrings = allowNewlinesInStrings;
			_lenientComma = lenientComma;
			_buffer = new byte[bufferSize + _overlapSize];

			int totalRead = 0;
			while (totalRead < _buffer.Length)
			{
				int read = _baseStream.Read(_buffer, totalRead, _buffer.Length - totalRead);
				if (read == 0) break;
				totalRead += read;
			}

			if (totalRead < _buffer.Length)
			{
				_isEndOfStream = true;
				_scanEnd = totalRead;
			}
			else
			{
				_scanEnd = bufferSize;
			}

			if (totalRead >= 3
				&& _buffer[0] == 0xEF
				&& _buffer[1] == 0xBB
				&& _buffer[2] == 0xBF)
			{
				_scanIdx = 3;
			}
		}

		public override void Flush() { }

		public override int Read(byte[] buffer, int offset, int count)
			=> Read(buffer.AsSpan(offset, count));

		public
#if NET8_0_OR_GREATER
			override
#endif
			int Read(Span<byte> buffer)
		{
			ObjectDisposedException.ThrowIf(_disposed, nameof(JsonCompactStream));

			int i = 0;
			while (i < buffer.Length)
			{
				if (_pendingByte >= 0)
				{
					buffer[i++] = (byte)_pendingByte;
					_pendingByte = -1;
					continue;
				}

				if (RingDistance(_scanIdx, _scanEnd) == 0)
				{
					if (_isEndOfStream) break;
					SwitchBuffer();
				}

#if NET8_0_OR_GREATER
				// ── SIMD fast path ──────────────────────────────────────────
				if ((Sse2.IsSupported || AdvSimd.IsSupported) && _pendingByte < 0)
				{
					int contiguous = Math.Min(
						RingDistance(_scanIdx, _scanEnd),
						_buffer.Length - _scanIdx);

					if (contiguous >= Vector128<byte>.Count)
					{
						ref byte scanStart = ref _buffer[_scanIdx];
						bool advanced = false;

						if (_parserState == ParserState.InString)
						{
							// Bulk-copy string content up to the next " or \
							int str = ScanStringContent(ref scanStart, contiguous, _allowNewlinesInStrings);
							if (str > 0)
							{
								int copy = Math.Min(str, buffer.Length - i);
								_buffer.AsSpan(_scanIdx, copy).CopyTo(buffer[i..]);
								i += copy;
								_scanIdx = (_scanIdx + copy) % _buffer.Length;
								advanced = true;
							}
						}
						else if (_parserState == ParserState.Normal)
						{
							// 1) Skip whitespace
							int ws = ScanWhitespace(ref scanStart, contiguous);
							if (ws > 0)
							{
								_scanIdx = (_scanIdx + ws) % _buffer.Length;
								contiguous -= ws;
								scanStart = ref _buffer[_scanIdx];
							}

							if (contiguous >= Vector128<byte>.Count)
							{
								byte first = _buffer[_scanIdx];

								// 2) Insert implicit comma before value if needed
								if (_lenientComma && _lastWasValue && IsValueStart(first) && i < buffer.Length)
								{
									buffer[i++] = (byte)',';
									_lastWasValue = false;
								}

								// 3) Found " → full string lifecycle
								if (first == (byte)'"')
								{
									if (i < buffer.Length)
									{
										buffer[i++] = (byte)'"';
										_scanIdx = (_scanIdx + 1) % _buffer.Length;
										contiguous--;
										if (contiguous > 0)
										{
											ref byte strStart = ref _buffer[_scanIdx];
											int contentLen = ScanStringContent(ref strStart, contiguous, _allowNewlinesInStrings);
											if (contentLen > 0)
											{
												int copy = Math.Min(contentLen, buffer.Length - i);
												_buffer.AsSpan(_scanIdx, copy).CopyTo(buffer[i..]);
												i += copy;
												_scanIdx = (_scanIdx + copy) % _buffer.Length;
											}
										}
										_parserState = ParserState.InString;
										advanced = true;
									}
								}
								// 4) Found digit/- → batch copy value content
								else if (first == (byte)'-' || (first >= (byte)'0' && first <= (byte)'9'))
								{
									int val = ScanValueContent(ref scanStart, contiguous);
									if (val > 0)
									{
										int copy = Math.Min(val, buffer.Length - i);
										_buffer.AsSpan(_scanIdx, copy).CopyTo(buffer[i..]);
										i += copy;
										_scanIdx = (_scanIdx + copy) % _buffer.Length;
										_parserState = ParserState.InValue;
										advanced = true;
									}
								}
							}

							if (!advanced && ws > 0)
								advanced = true;
						}
						else if (_parserState == ParserState.InValue)
						{
							int val = ScanValueContent(ref scanStart, contiguous);
							if (val > 0)
							{
								int copy = Math.Min(val, buffer.Length - i);
								_buffer.AsSpan(_scanIdx, copy).CopyTo(buffer[i..]);
								i += copy;
								_scanIdx = (_scanIdx + copy) % _buffer.Length;
								advanced = true;
							}
						}

						if (advanced) continue;
					}
				}
				// ── end SIMD fast path ──────────────────────────────────────
#endif

				if (i >= buffer.Length) break;

				byte b = _buffer[_scanIdx];

				if (_lenientComma && _lastWasValue && IsValueStart(b))
				{
					buffer[i++] = (byte)',';
					_lastWasValue = false;
					if (i >= buffer.Length) return i;
				}

				switch (_parserState)
				{
					case ParserState.Normal:
						switch (b)
						{
							case (byte)'"':
								_parserState = ParserState.InString;
								_lastWasValue = false;
								buffer[i++] = b;
								_scanIdx = (_scanIdx + 1) % _buffer.Length;
								break;
							case (byte)'{':
							case (byte)'[':
								_nestDepth++;
								_lastWasValue = false;
								buffer[i++] = b;
								_scanIdx = (_scanIdx + 1) % _buffer.Length;
								break;
							case (byte)'}':
							case (byte)']':
								_nestDepth--;
								_lastWasValue = true;
								buffer[i++] = b;
								_scanIdx = (_scanIdx + 1) % _buffer.Length;
								break;
							case (byte)',':
								if (_lenientComma)
									_scanIdx = (_scanIdx + 1) % _buffer.Length;
								else
								{
									buffer[i++] = b;
									_scanIdx = (_scanIdx + 1) % _buffer.Length;
								}
								break;
							case (byte)':':
								_lastWasValue = false;
								buffer[i++] = b;
								_scanIdx = (_scanIdx + 1) % _buffer.Length;
								break;
							case (byte)'f':
								_parserState = ParserState.InKeyword;
								_currentKeyword = "false";
								_kwIndex = 1;
								_lastWasValue = false;
								buffer[i++] = b;
								_scanIdx = (_scanIdx + 1) % _buffer.Length;
								break;
							case (byte)'t':
								_parserState = ParserState.InKeyword;
								_currentKeyword = "true";
								_kwIndex = 1;
								_lastWasValue = false;
								buffer[i++] = b;
								_scanIdx = (_scanIdx + 1) % _buffer.Length;
								break;
							case (byte)'n':
								_parserState = ParserState.InKeyword;
								_currentKeyword = "null";
								_kwIndex = 1;
								_lastWasValue = false;
								buffer[i++] = b;
								_scanIdx = (_scanIdx + 1) % _buffer.Length;
								break;
							default:
								if (b == (byte)'-' || (b >= (byte)'0' && b <= (byte)'9'))
								{
									_parserState = ParserState.InValue;
									_lastWasValue = false;
									buffer[i++] = b;
									_scanIdx = (_scanIdx + 1) % _buffer.Length;
								}
								else if (b == (byte)' ' || b == (byte)'\t' || b == (byte)'\n' || b == (byte)'\r')
								{
									_scanIdx = (_scanIdx + 1) % _buffer.Length;
								}
								else
								{
									buffer[i++] = b;
									_scanIdx = (_scanIdx + 1) % _buffer.Length;
								}
								break;
						}
						break;

					case ParserState.InString:
						if (b == (byte)'\\')
						{
							_parserState = ParserState.InEscape;
							buffer[i++] = b;
							_scanIdx = (_scanIdx + 1) % _buffer.Length;
						}
						else if (b == (byte)'"')
						{
							_parserState = ParserState.Normal;
							_lastWasValue = true;
							buffer[i++] = b;
							_scanIdx = (_scanIdx + 1) % _buffer.Length;
						}
						else if (_allowNewlinesInStrings && (b == (byte)'\n' || b == (byte)'\r' || b == (byte)'\t' || b == (byte)'\b'))
						{
							buffer[i++] = (byte)'\\';
							if (i >= buffer.Length)
							{
								_pendingByte = b switch
								{
									(byte)'\n' => (byte)'n',
									(byte)'\r' => (byte)'r',
									(byte)'\t' => (byte)'t',
									_ => (byte)'b',
								};
								_scanIdx = (_scanIdx + 1) % _buffer.Length;
								return i;
							}
							buffer[i++] = b switch
							{
								(byte)'\n' => (byte)'n',
								(byte)'\r' => (byte)'r',
								(byte)'\t' => (byte)'t',
								_ => (byte)'b',
							};
							_scanIdx = (_scanIdx + 1) % _buffer.Length;
						}
						else
						{
							buffer[i++] = b;
							_scanIdx = (_scanIdx + 1) % _buffer.Length;
						}
						break;

					case ParserState.InEscape:
						_parserState = ParserState.InString;
						buffer[i++] = b;
						_scanIdx = (_scanIdx + 1) % _buffer.Length;
						break;

					case ParserState.InKeyword:
						if (_kwIndex < _currentKeyword.Length && b == (byte)_currentKeyword[_kwIndex])
						{
							_kwIndex++;
							buffer[i++] = b;
							_scanIdx = (_scanIdx + 1) % _buffer.Length;
							if (_kwIndex >= _currentKeyword.Length)
							{
								_parserState = ParserState.Normal;
								_lastWasValue = true;
							}
						}
						else
						{
							_parserState = ParserState.Normal;
							_lastWasValue = true;
						}
						break;

					case ParserState.InValue:
						if (b >= (byte)'0' && b <= (byte)'9')
						{
							buffer[i++] = b;
							_scanIdx = (_scanIdx + 1) % _buffer.Length;
						}
						else if (b == (byte)'.' || b == (byte)'e' || b == (byte)'E' || b == (byte)'+' || b == (byte)'-')
						{
							buffer[i++] = b;
							_scanIdx = (_scanIdx + 1) % _buffer.Length;
						}
						else
						{
							_parserState = ParserState.Normal;
							_lastWasValue = true;
						}
						break;
				}
			}
			return i;
		}

		public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(SeekNotSupported);
		public override void SetLength(long value) => throw new NotSupportedException(SeekNotSupported);
		public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException(WriteNotSupported);

		protected override void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing && !_leaveOpen)
					_baseStream?.Dispose();

				_disposed = true;
			}
			base.Dispose(disposing);
		}

		private int RingDistance(int from, int to)
			=> (to - from + _buffer.Length) % _buffer.Length;

		private static bool IsValueStart(byte b)
			=> b == (byte)'"'
			|| b == (byte)'-'
			|| (b >= (byte)'0' && b <= (byte)'9')
			|| b == (byte)'t'
			|| b == (byte)'f'
			|| b == (byte)'n'
			|| b == (byte)'{'
			|| b == (byte)'[';

		private void SwitchBuffer()
		{
			int overlapEnd = (_scanEnd + _overlapSize) % _buffer.Length;

			if (_scanEnd < overlapEnd)
			{
				int total1 = 0;
				int target1 = _buffer.Length - overlapEnd;
				while (total1 < target1)
				{
					int read1 = _baseStream.Read(_buffer, overlapEnd + total1, target1 - total1);
					if (read1 == 0) { _isEndOfStream = true; _scanEnd = overlapEnd + total1; return; }
					total1 += read1;
				}

				int total2 = 0;
				while (total2 < _scanEnd)
				{
					int read2 = _baseStream.Read(_buffer, total2, _scanEnd - total2);
					if (read2 == 0) { _isEndOfStream = true; _scanEnd = total2; return; }
					total2 += read2;
				}
			}
			else
			{
				int total = 0;
				int target = _scanEnd - overlapEnd;
				while (total < target)
				{
					int read = _baseStream.Read(_buffer, overlapEnd + total, target - total);
					if (read == 0) { _isEndOfStream = true; _scanEnd = overlapEnd + total; return; }
					total += read;
				}
			}

			_scanEnd = (_scanEnd + _windowSize) % _buffer.Length;
		}

		// ── SIMD scan helpers (NET8+) ────────────────────────────────────

#if NET8_0_OR_GREATER
		/// <summary>
		/// Scans string content for the first byte that needs special handling:
		/// <c>"</c>, <c>\</c>, and optionally <c>\n \r \t \b</c>.
		/// Returns the number of bytes that can be copied directly.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int ScanStringContent(ref byte start, int maxLen, bool escapeSpecialWhitespace)
		{
			int i = 0;
			var vQuote = Vector128.Create((byte)'"');
			var vBackslash = Vector128.Create((byte)'\\');

			if (escapeSpecialWhitespace)
			{
				var vNL = Vector128.Create((byte)'\n');
				var vCR = Vector128.Create((byte)'\r');
				var vTab = Vector128.Create((byte)'\t');
				var vBS = Vector128.Create((byte)'\b');

				while (i + Vector128<byte>.Count <= maxLen)
				{
					var vec = Vector128.LoadUnsafe(ref start, (nuint)i);
					uint bits = (uint)Vector128.ExtractMostSignificantBits(
						Vector128.Equals(vec, vQuote)
						| Vector128.Equals(vec, vBackslash)
						| Vector128.Equals(vec, vNL)
						| Vector128.Equals(vec, vCR)
						| Vector128.Equals(vec, vTab)
						| Vector128.Equals(vec, vBS));
					if (bits != 0)
						return i + BitOperations.TrailingZeroCount(bits);
					i += Vector128<byte>.Count;
				}
			}
			else
			{
				while (i + Vector128<byte>.Count <= maxLen)
				{
					var vec = Vector128.LoadUnsafe(ref start, (nuint)i);
					uint bits = (uint)Vector128.ExtractMostSignificantBits(
						Vector128.Equals(vec, vQuote)
						| Vector128.Equals(vec, vBackslash));
					if (bits != 0)
						return i + BitOperations.TrailingZeroCount(bits);
					i += Vector128<byte>.Count;
				}
			}

			// Scalar tail
			while (i < maxLen)
			{
				byte b = Unsafe.Add(ref start, (nuint)i);
				if (b == '"' || b == '\\') break;
				if (escapeSpecialWhitespace && (b == '\n' || b == '\r' || b == '\t' || b == '\b')) break;
				i++;
			}
			return i;
		}

		/// <summary>
		/// Scans for the first non-whitespace byte.
		/// Returns the number of whitespace bytes to skip.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int ScanWhitespace(ref byte start, int maxLen)
		{
			int i = 0;
			var vSpace = Vector128.Create((byte)' ');
			var vTab = Vector128.Create((byte)'\t');
			var vNL = Vector128.Create((byte)'\n');
			var vCR = Vector128.Create((byte)'\r');

			while (i + Vector128<byte>.Count <= maxLen)
			{
				var vec = Vector128.LoadUnsafe(ref start, (nuint)i);
				uint wsBits = (uint)Vector128.ExtractMostSignificantBits(
					Vector128.Equals(vec, vSpace)
					| Vector128.Equals(vec, vTab)
					| Vector128.Equals(vec, vNL)
					| Vector128.Equals(vec, vCR));
				uint allBits = (1u << Vector128<byte>.Count) - 1;
				if (wsBits != allBits)
					return i + BitOperations.TrailingZeroCount(~wsBits & allBits);
				i += Vector128<byte>.Count;
			}

			// Scalar tail
			while (i < maxLen)
			{
				byte b = Unsafe.Add(ref start, (nuint)i);
				if (b != ' ' && b != '\t' && b != '\n' && b != '\r') break;
				i++;
			}
			return i;
		}

		/// <summary>
		/// Scans a JSON numeric value for the first byte that terminates it.
		/// Valid value bytes: <c>0-9 . e E + -</c>.
		/// Returns the number of value bytes before termination.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int ScanValueContent(ref byte start, int maxLen)
		{
			int i = 0;
			var vDot = Vector128.Create((byte)'.');
			var vE = Vector128.Create((byte)'e');
			var vEE = Vector128.Create((byte)'E');
			var vPlus = Vector128.Create((byte)'+');
			var vMinus = Vector128.Create((byte)'-');
			var vZero = Vector128.Create((byte)'0');
			var vNine = Vector128.Create((byte)'9');

			while (i + Vector128<byte>.Count <= maxLen)
			{
				var vec = Vector128.LoadUnsafe(ref start, (nuint)i);
				uint valBits = (uint)Vector128.ExtractMostSignificantBits(
					Vector128.Equals(vec, vDot)
					| Vector128.Equals(vec, vE)
					| Vector128.Equals(vec, vEE)
					| Vector128.Equals(vec, vPlus)
					| Vector128.Equals(vec, vMinus)
					| (Vector128.GreaterThanOrEqual(vec, vZero)
					   & Vector128.LessThanOrEqual(vec, vNine)));
				uint allBits = (1u << Vector128<byte>.Count) - 1;
				if (valBits != allBits)
					return i + BitOperations.TrailingZeroCount(~valBits & allBits);
				i += Vector128<byte>.Count;
			}

			// Scalar tail
			while (i < maxLen)
			{
				byte b = Unsafe.Add(ref start, (nuint)i);
				if (!((b >= '0' && b <= '9') || b == '.' || b == 'e' || b == 'E' || b == '+' || b == '-'))
					break;
				i++;
			}
			return i;
		}
#endif
	}
}
