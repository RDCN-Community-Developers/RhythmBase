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
    /// Asynchronously obtains the JSON data as a <see cref="ReadOnlyMemory{T}"/> of UTF-8 bytes.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The JSON data as UTF-8 bytes.</returns>
    ValueTask<ReadOnlyMemory<byte>> GetMemoryAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Synchronously obtains the JSON data as a <see cref="ReadOnlyMemory{T}"/> of UTF-8 bytes.
    /// </summary>
    /// <returns>The JSON data as UTF-8 bytes.</returns>
    ReadOnlyMemory<byte> GetMemory();
    /// <summary>
    /// Gets a value indicating whether <see cref="GetMemory"/> can return data directly without buffering.
    /// </summary>
    bool CanGetMemoryDirectly { get; }
}
/// <summary>
/// An <see cref="IJsonDataSource"/> backed by a <see cref="Stream"/>. The stream is fully buffered
/// into memory on construction, with special character escaping applied.
/// </summary>
public class StreamDataSource : IJsonDataSource
{
    private readonly Stream stream;
    /// <summary>
    /// Initializes a new instance of <see cref="StreamDataSource"/> from the specified stream.
    /// </summary>
    /// <param name="stream">The stream containing JSON data.</param>
    public StreamDataSource(Stream stream)
    {
        MemoryStream ms = new();
        using JsonCompactStream escStream = new(stream,
          allowNewlinesInStrings: true,
          allowImplicitComma: true,
          allowTrailingComma: true);
        escStream.CopyTo(ms);
        ms.Position = 0;
        this.stream = ms;
    }
    /// <inheritdoc/>
    public bool CanGetMemoryDirectly => false;
    /// <inheritdoc/>
    public ReadOnlyMemory<byte> GetMemory()
    {
        throw new NotSupportedException();
    }
    /// <inheritdoc/>
    public async ValueTask<ReadOnlyMemory<byte>> GetMemoryAsync(CancellationToken cancellationToken = default)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent((int)stream.Length);
        try
        {
            int bytesRead = 3;
            // bom
            await stream.ReadAsync(buffer, 0, 3, cancellationToken);
            if (buffer is [0xEF, 0xBB, 0xBF, ..])
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length - 3, cancellationToken);
            else
                bytesRead += await stream.ReadAsync(buffer, 3, buffer.Length - 3, cancellationToken);
            return new ReadOnlyMemory<byte>(buffer, 0, bytesRead);
        }
        catch
        {
            ArrayPool<byte>.Shared.Return(buffer);
            throw;
        }
    }
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
    public bool CanGetMemoryDirectly => true;
    /// <inheritdoc/>
    public ReadOnlyMemory<byte> GetMemory()
    {
        return Encoding.UTF8.GetBytes(jsonDocument.RootElement.GetRawText());
    }
    /// <inheritdoc/>
    public ValueTask<ReadOnlyMemory<byte>> GetMemoryAsync(CancellationToken cancellationToken = default)
    {
        return new(GetMemory());
    }
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
    public bool CanGetMemoryDirectly => true;
    /// <inheritdoc/>
    public ReadOnlyMemory<byte> GetMemory()
    {
        return jsonData;
    }
    /// <inheritdoc/>
    public ValueTask<ReadOnlyMemory<byte>> GetMemoryAsync(CancellationToken cancellationToken = default)
    {
        return new(jsonData);
    }
}
