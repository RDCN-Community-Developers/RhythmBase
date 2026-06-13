using System;
using System.IO;

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
		private readonly bool _allowTrailingComma;
		private readonly bool _allowImplicitComma;

		private bool _lastWasValue;
		private bool _pendingComma;
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
			int bufferSize = 1024,
			bool leaveOpen = false,
			bool allowNewlinesInStrings = false,
			bool allowTrailingComma = false,
			bool allowImplicitComma = false)
		{
			ArgumentNullException.ThrowIfNull(stream, nameof(stream));
			if (bufferSize < 1) throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size must be greater than 0.");

			_baseStream = stream;
			_windowSize = bufferSize;
			_leaveOpen = leaveOpen;
			_allowNewlinesInStrings = allowNewlinesInStrings;
			_allowTrailingComma = allowTrailingComma;
			_allowImplicitComma = allowImplicitComma;
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

				byte b = _buffer[_scanIdx];

				if (_pendingComma)
				{
					if (b == (byte)' ' || b == (byte)'\t' || b == (byte)'\n' || b == (byte)'\r')
					{
						_scanIdx = (_scanIdx + 1) % _buffer.Length;
						continue;
					}
					if (b == (byte)']' || b == (byte)'}')
					{
						_pendingComma = false;
					}
					else
					{
						buffer[i++] = (byte)',';
						_pendingComma = false;
						if (i >= buffer.Length)
						{
							_pendingByte = -1;
							return i;
						}
					}
				}

				if (_allowImplicitComma && _lastWasValue && !_pendingComma && IsValueStart(b))
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
								_lastWasValue = false;
								if (_allowTrailingComma)
								{
									_pendingComma = true;
									_scanIdx = (_scanIdx + 1) % _buffer.Length;
								}
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
				int read1 = _baseStream.Read(_buffer, overlapEnd, _buffer.Length - overlapEnd);
				if (read1 < _buffer.Length - overlapEnd)
				{
					_isEndOfStream = true;
					_scanEnd = overlapEnd + read1;
					return;
				}

				int read2 = _baseStream.Read(_buffer, 0, _scanEnd);
				if (read2 < _scanEnd)
				{
					_isEndOfStream = true;
					_scanEnd = read2;
					return;
				}
			}
			else
			{
				int read = _baseStream.Read(_buffer, overlapEnd, _scanEnd - overlapEnd);
				if (read < _scanEnd - overlapEnd)
				{
					_isEndOfStream = true;
					_scanEnd = overlapEnd + read;
					return;
				}
			}

			_scanEnd = (_scanEnd + _windowSize) % _buffer.Length;
		}
	}
}
