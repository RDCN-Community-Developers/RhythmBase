using System.Buffers;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Global.Converters.JsonSerialization
{

    internal interface IJsonDataSource
    {
        ValueTask<ReadOnlyMemory<byte>> GetMemoryAsync(CancellationToken cancellationToken = default);
        ReadOnlyMemory<byte> GetMemory();
        bool CanGetMemoryDirectly { get; }
    }
    internal readonly struct StreamDataSource : IJsonDataSource
    {
        private readonly Stream stream;
        public StreamDataSource(Stream stream)
        {
            this.stream = stream;
        }
        public bool CanGetMemoryDirectly => false;
        public ReadOnlyMemory<byte> GetMemory()
        {
            throw new NotSupportedException();
        }
        public async ValueTask<ReadOnlyMemory<byte>> GetMemoryAsync(CancellationToken cancellationToken = default)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent((int)stream.Length);
            try
            {
                int bytesRead = 3;
                // bom
                await stream.ReadAsync(buffer, 0, 3, cancellationToken);
                if (buffer is [0xEF, 0xBB, 0xBF, ..])
                    bytesRead = await stream.ReadAsync(buffer, 3, buffer.Length - 3, cancellationToken);
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
    internal readonly struct JsonDocumentDataSource : IJsonDataSource
    {
        private readonly JsonDocument jsonDocument;
        public JsonDocumentDataSource(JsonDocument jsonDocument)
        {
            this.jsonDocument = jsonDocument;
        }
        public bool CanGetMemoryDirectly => true;
        public ReadOnlyMemory<byte> GetMemory()
        {
            return Encoding.UTF8.GetBytes(jsonDocument.RootElement.GetRawText());
        }
        public ValueTask<ReadOnlyMemory<byte>> GetMemoryAsync(CancellationToken cancellationToken = default)
        {
            return new(GetMemory());
        }
    }
    internal readonly struct ReadOnlyMemoryDataSource : IJsonDataSource
    {
        private readonly ReadOnlyMemory<byte> jsonData;
        public ReadOnlyMemoryDataSource(ReadOnlyMemory<byte> jsonData)
        {
            this.jsonData = jsonData;
        }
        public bool CanGetMemoryDirectly => true;
        public ReadOnlyMemory<byte> GetMemory()
        {
            return jsonData;
        }
        public ValueTask<ReadOnlyMemory<byte>> GetMemoryAsync(CancellationToken cancellationToken = default)
        {
            return new(jsonData);
        }
    }
}
