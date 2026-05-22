namespace RhythmBase.Global.Components;

internal class EscapeSpecialCharacterStream(Stream inner) : Stream
{
    private readonly Stream _inner = inner;
#if NET8_0_OR_GREATER
    private readonly Memory<byte> _mbuffer = new byte[1];
#endif
    private readonly byte[] _buffer = new byte[1];
    private bool _inQuotes = false;
    private bool _prevIsEscape = false;
    private int _peeked = -1; // 用于缓存下一个字节

    public override bool CanRead => _inner.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override void Flush() { }
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (count == 0) return 0;
        int written = 0;

        static bool IsQuote(byte b) => b == (byte)'"';
        static bool IsEscape(byte b) => b == (byte)'\\';
        static bool IsCR(byte b) => b == 0x0D;
        static bool IsLF(byte b) => b == 0x0A;
        static bool IsTab(byte b) => b == (byte)'\t';

        while (written < count)
        {
            byte b;
            if (_peeked >= 0)
            {
                b = (byte)_peeked;
                _peeked = -1;
            }
            else
            {
                int read = _inner.Read(_buffer, 0, 1);
                if (read == 0)
                {
                    break; // EOF
                }
                b = _buffer[0];
            }
            if (IsQuote(b) && !_prevIsEscape)
            {
                _inQuotes = !_inQuotes;
                _prevIsEscape = false;
                buffer[offset + written++] = b;
            }
            else if (IsTab(b) && _inQuotes)
            {
                // \t -> \\t
                if (written + 2 > count)
                {
                    _peeked = b;
                    break;
                }
                buffer[offset + written++] = (byte)'\\';
                buffer[offset + written++] = (byte)'t';
                _prevIsEscape = false;
            }
            else if (IsEscape(b) && _inQuotes)
            {
                _prevIsEscape = !_prevIsEscape;
                buffer[offset + written++] = b;
            }
            else if (_inQuotes && IsCR(b))
            {
                // 可能是 \r\n
                int read = _inner.Read(_buffer, 0, 1);
                if (read == 0) break;
                if (read > 0 && IsLF(_buffer[0]))
                {
                    // \r\n -> \\r\\n
                    if (written + 4 > count)
                    {
                        _peeked = _buffer[0];
                        break;
                    }
                    buffer[offset + written++] = (byte)'\\';
                    buffer[offset + written++] = (byte)'r';
                    buffer[offset + written++] = (byte)'\\';
                    buffer[offset + written++] = (byte)'n';
                }
                else
                {
                    // 只有 \r
                    if (written + 2 > count)
                    {
                        if (read > 0) _peeked = _buffer[0];
                        break;
                    }
                    buffer[offset + written++] = (byte)'\\';
                    buffer[offset + written++] = (byte)'r';
                    if (read > 0) _peeked = _buffer[0];
                }
                _prevIsEscape = false;
            }
            else if (_inQuotes && IsLF(b))
            {
                // \n -> \\n
                if (written + 2 > count)
                    break;
                buffer[offset + written++] = (byte)'\\';
                buffer[offset + written++] = (byte)'n';
                _prevIsEscape = false;
            }
            else
            {
                buffer[offset + written++] = b;
                _prevIsEscape = false;
            }
        }
        return written > 0 ? written : 0;
    }
    public override async Task<int> ReadAsync(byte[] buffer,
        int offset,
        int count,
        CancellationToken ct = default)
    {
        if (count == 0) return 0;
        int written = 0;

        static bool IsQuote(byte b) => b == (byte)'"';
        static bool IsEscape(byte b) => b == (byte)'\\';
        static bool IsCR(byte b) => b == 0x0D;
        static bool IsLF(byte b) => b == 0x0A;
        static bool IsTab(byte b) => b == (byte)'\t';

        while (written < count)
        {
            byte b;
            if (_peeked >= 0)
            {
                b = (byte)_peeked;
                _peeked = -1;
            }
            else
            {
#if NET8_0_OR_GREATER
                int read = await _inner.ReadAsync(_buffer.AsMemory(0, 1), ct);
#else
				int read = await _inner.ReadAsync(_buffer, 0, 1, ct);
#endif
                if (read == 0)
                {
                    break; // EOF
                }
                b = _buffer[0];
            }
            if (IsQuote(b) && !_prevIsEscape)
            {
                _inQuotes = !_inQuotes;
                _prevIsEscape = false;
                buffer[offset + written++] = b;
            }
            else if (IsTab(b) && _inQuotes)
            {
                // \t -> \\t
                if (written + 2 > count)
                {
                    _peeked = b;
                    break;
                }
                buffer[offset + written++] = (byte)'\\';
                buffer[offset + written++] = (byte)'t';
                _prevIsEscape = false;
            }
            else if (IsEscape(b) && _inQuotes)
            {
                _prevIsEscape = !_prevIsEscape;
                buffer[offset + written++] = b;
            }
            else if (_inQuotes && IsCR(b))
            {
                // 可能是 \r\n
#if NET8_0_OR_GREATER
                int read = await _inner.ReadAsync(_buffer.AsMemory(0, 1), ct);
#else
				int read = await _inner.ReadAsync(_buffer, 0, 1, ct);
#endif
                if (read == 0) break;
                if (read > 0 && IsLF(_buffer[0]))
                {
                    // \r\n -> \\r\\n
                    if (written + 4 > count)
                    {
                        _peeked = _buffer[0];
                        break;
                    }
                    buffer[offset + written++] = (byte)'\\';
                    buffer[offset + written++] = (byte)'r';
                    buffer[offset + written++] = (byte)'\\';
                    buffer[offset + written++] = (byte)'n';
                }
                else
                {
                    // 只有 \r
                    if (written + 2 > count)
                    {
                        if (read > 0) _peeked = _buffer[0];
                        break;
                    }
                    buffer[offset + written++] = (byte)'\\';
                    buffer[offset + written++] = (byte)'r';
                    if (read > 0) _peeked = _buffer[0];
                }
                _prevIsEscape = false;
            }
            else if (_inQuotes && IsLF(b))
            {
                // \n -> \\n
                if (written + 2 > count)
                    break;
                buffer[offset + written++] = (byte)'\\';
                buffer[offset + written++] = (byte)'n';
                _prevIsEscape = false;
            }
            else
            {
                buffer[offset + written++] = b;
                _prevIsEscape = false;
            }
        }
        return written > 0 ? written : 0;
    }
#if NET8_0_OR_GREATER
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (buffer.Length == 0) return 0;
        int written = 0;
        Span<byte> span = buffer.Span;

        static bool IsQuote(byte b) => b == (byte)'"';
        static bool IsEscape(byte b) => b == (byte)'\\';
        static bool IsCR(byte b) => b == 0x0D;
        static bool IsLF(byte b) => b == 0x0A;
        static bool IsTab(byte b) => b == (byte)'\t';

        while (written < span.Length)
        {
            byte b;
            if (_peeked >= 0)
            {
                b = (byte)_peeked;
                _peeked = -1;
            }
            else
            {
                int read = _inner.Read(_mbuffer.Span);
                if (read == 0) break;
                b = _buffer[0];
            }

            if (IsQuote(b) && !_prevIsEscape)
            {
                _inQuotes = !_inQuotes;
                _prevIsEscape = false;
                span[written++] = b;
            }
            else if (IsTab(b) && _inQuotes)
            {
                if (written + 2 > span.Length)
                {
                    _peeked = b;
                    break;
                }
                span[written++] = (byte)'\\';
                span[written++] = (byte)'t';
                _prevIsEscape = false;
            }
            else if (IsEscape(b) && _inQuotes)
            {
                _prevIsEscape = !_prevIsEscape;
                span[written++] = b;
            }
            else if (_inQuotes && IsCR(b))
            {
                int read = _inner.Read(_mbuffer.Span);
                if (read == 0)
                {
                    if (written + 2 > span.Length)
                    {
                        _peeked = b;
                        break;
                    }
                    span[written++] = (byte)'\\';
                    span[written++] = (byte)'r';
                    break;
                }

                if (IsLF(_buffer[0]))
                {
                    if (written + 4 > span.Length)
                    {
                        _peeked = _buffer[0];
                        _peeked = _buffer[0];
                        _inner.Position -= 1;
                        break;
                    }
                    span[written++] = (byte)'\\';
                    span[written++] = (byte)'r';
                    span[written++] = (byte)'\\';
                    span[written++] = (byte)'n';
                }
                else
                {
                    if (written + 2 > span.Length)
                    {
                        _peeked = _buffer[0];
                        break;
                    }
                    span[written++] = (byte)'\\';
                    span[written++] = (byte)'r';
                    _peeked = _buffer[0];
                }
                _prevIsEscape = false;
            }
            else if (_inQuotes && IsLF(b))
            {
                if (written + 2 > span.Length)
                {
                    _peeked = b;
                    break;
                }
                span[written++] = (byte)'\\';
                span[written++] = (byte)'n';
                _prevIsEscape = false;
            }
            else
            {
                span[written++] = b;
                _prevIsEscape = false;
            }
        }

        return written > 0 ? written : 0;
    }
#endif
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw new NotSupportedException();
#if NET8_0_OR_GREATER
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return base.WriteAsync(buffer, cancellationToken);
    }
#endif
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _inner.Dispose();
        base.Dispose(disposing);
    }
}
