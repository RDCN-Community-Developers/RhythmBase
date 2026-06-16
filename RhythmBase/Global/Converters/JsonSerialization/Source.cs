using System.Buffers;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Global.Converters.JsonSerialization;

/// <summary>
/// Abstracts the source of JSON data for level deserialization, allowing callers to provide
/// a <see cref="Stream"/>, <see cref="JsonDocument"/>, or raw <see cref="ReadOnlyMemory{T}"/> of bytes.
/// </summary>
public interface IJsonDataSource
{
    /// <summary>
    /// Asynchronously obtains the JSON data as a <see cref="ReadOnlySequence{T}"/> of UTF-8 bytes.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The JSON data as a sequence of UTF-8 byte segments.</returns>
    ValueTask<ReadOnlySequence<byte>> GetSequenceAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Synchronously obtains the JSON data as a <see cref="ReadOnlySequence{T}"/> of UTF-8 bytes.
    /// </summary>
    /// <returns>The JSON data as a sequence of UTF-8 byte segments.</returns>
    ReadOnlySequence<byte> GetSequence();
    /// <summary>
    /// Maps a byte position in the processed (compacted) JSON data back to an approximate
    /// byte position in the original source stream. Returns <c>-1</c> if mapping is unavailable.
    /// </summary>
    /// <param name="processedPosition">The byte position from <see cref="Utf8JsonReader.BytesConsumed"/> or <see cref="Utf8JsonReader.TokenStartIndex"/>.</param>
    /// <returns>The approximate byte position in the original stream, or <c>-1</c> if unavailable.</returns>
    long MapToInputPosition(long processedPosition);
}

/// <summary>
/// An <see cref="IJsonDataSource"/> backed by a <see cref="Stream"/>. The stream is processed
/// through a <see cref="JsonCompactStream"/> and buffered in segmented chunks to avoid large
/// contiguous allocations.
/// </summary>
public class StreamDataSource : IJsonDataSource
{
    private readonly ReadOnlySequence<byte> dataSequence;
    private readonly (long output, long input)[] _positionMap;

    private sealed class JsonSegment : ReadOnlySequenceSegment<byte>
    {
        public JsonSegment(ReadOnlyMemory<byte> memory)
        {
            Memory = memory;
        }

        public JsonSegment Append(ReadOnlyMemory<byte> memory)
        {
            var next = new JsonSegment(memory)
            {
                RunningIndex = RunningIndex + Memory.Length
            };
            Next = next;
            return next;
        }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="StreamDataSource"/> from the specified stream.
    /// </summary>
    /// <param name="stream">The stream containing JSON data.</param>
    public StreamDataSource(Stream stream)
    {
        using JsonCompactStream escStream = new(stream,
            allowNewlinesInStrings: true,
            lenientComma: true);

        const int chunkSize = 65536;
        byte[] rentBuf = ArrayPool<byte>.Shared.Rent(chunkSize);
        try
        {
            int firstRead = escStream.Read(rentBuf, 0, rentBuf.Length);
            if (firstRead == 0)
            {
                dataSequence = ReadOnlySequence<byte>.Empty;
                _positionMap = Array.Empty<(long, long)>();
                return;
            }

            if (firstRead < rentBuf.Length)
            {
                // Small file: first read didn't fill buffer, file is fully read
                byte[] single = new byte[firstRead];
                Buffer.BlockCopy(rentBuf, 0, single, 0, firstRead);
                dataSequence = new ReadOnlySequence<byte>(single);
                _positionMap = escStream.PositionMap.ToArray();
                return;
            }

            // Large file: first read filled buffer, build linked list from chunks
            var head = new JsonSegment(ReadOnlyMemory<byte>.Empty);
            var current = head;

            byte[] firstOwned = new byte[firstRead];
            Buffer.BlockCopy(rentBuf, 0, firstOwned, 0, firstRead);
            current = current.Append(firstOwned);

            int read;
            while ((read = escStream.Read(rentBuf, 0, rentBuf.Length)) > 0)
            {
                byte[] owned = new byte[read];
                Buffer.BlockCopy(rentBuf, 0, owned, 0, read);
                current = current.Append(owned);
            }

            dataSequence = new ReadOnlySequence<byte>(head, 0, current, current.Memory.Length);
            _positionMap = escStream.PositionMap.ToArray();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rentBuf);
        }
    }

    /// <inheritdoc/>
    public long MapToInputPosition(long processedPosition)
    {
        if (_positionMap.Length == 0) return -1;

        int lo = 0, hi = _positionMap.Length - 1;
        int best = 0;
        while (lo <= hi)
        {
            int mid = (lo + hi) >>> 1;
            if (_positionMap[mid].output <= processedPosition)
            {
                best = mid;
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }
        return _positionMap[best].input;
    }

    /// <inheritdoc/>
    public ReadOnlySequence<byte> GetSequence() => dataSequence;

    /// <inheritdoc/>
    public ValueTask<ReadOnlySequence<byte>> GetSequenceAsync(CancellationToken cancellationToken = default)
        => new(dataSequence);
}

/// <summary>
/// An <see cref="IJsonDataSource"/> backed by a <see cref="JsonDocument"/>.
/// </summary>
public class JsonDocumentDataSource : IJsonDataSource
{
    private readonly JsonDocument jsonDocument;

    /// <summary>
    /// Initializes a new instance of <see cref="JsonDocumentDataSource"/> from the specified document.
    /// </summary>
    /// <param name="jsonDocument">The JSON document to read from.</param>
    public JsonDocumentDataSource(JsonDocument jsonDocument)
    {
        this.jsonDocument = jsonDocument;
    }

    /// <inheritdoc/>
    public ReadOnlySequence<byte> GetSequence()
        => new(Encoding.UTF8.GetBytes(jsonDocument.RootElement.GetRawText()));

    /// <inheritdoc/>
    public ValueTask<ReadOnlySequence<byte>> GetSequenceAsync(CancellationToken cancellationToken = default)
        => new(GetSequence());

    /// <inheritdoc/>
    public long MapToInputPosition(long processedPosition) => -1;
}

/// <summary>
/// An <see cref="IJsonDataSource"/> backed by a pre-loaded <see cref="ReadOnlyMemory{T}"/> of UTF-8 bytes.
/// </summary>
public class ReadOnlyMemoryDataSource : IJsonDataSource
{
    private readonly ReadOnlyMemory<byte> jsonData;

    /// <summary>
    /// Initializes a new instance of <see cref="ReadOnlyMemoryDataSource"/> from the specified byte memory.
    /// </summary>
    /// <param name="jsonData">The UTF-8 encoded JSON data.</param>
    public ReadOnlyMemoryDataSource(ReadOnlyMemory<byte> jsonData)
    {
        this.jsonData = jsonData;
    }

    /// <inheritdoc/>
    public ReadOnlySequence<byte> GetSequence() => new(jsonData);

    /// <inheritdoc/>
    public ValueTask<ReadOnlySequence<byte>> GetSequenceAsync(CancellationToken cancellationToken = default)
        => new(new ReadOnlySequence<byte>(jsonData));

    /// <inheritdoc/>
    public long MapToInputPosition(long processedPosition) => processedPosition;
}
