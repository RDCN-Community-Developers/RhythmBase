using RhythmBase.Global.Serialization;
using System.Text;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Components;

partial class Chart
{
	#region file
	/// <summary>
	/// Loads a chart from a single file containing only the <c>chart</c> part (an array of chart
	/// events). The <c>level</c> part is not loaded; the default level data applies. Use
	/// <see cref="FromFile(string, string)"/> to read both documents together.
	/// </summary>
	public static Chart FromFile(string filepath, LevelReadConfig? settings = null)
		=> FromFileAsync(filepath, settings).GetAwaiter().GetResult();
	/// <summary>
	/// Asynchronously loads a chart from a single file containing only the <c>chart</c> part. The
	/// <c>level</c> part is not loaded; the default level data applies. Use
	/// <see cref="FromFileAsync(string, string, CancellationToken)"/> to read both documents together.
	/// </summary>
	public static async Task<Chart> FromFileAsync(string filepath, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadConfig();
		using FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
		return await Task.Run(() => FromStream(stream, settings), cancellationToken);
	}
	/// <summary>
	/// Loads a chart from both documents: the <c>chart</c> file (an array of chart events) and the
	/// <c>level</c> file (an object of shared events).
	/// </summary>
	/// <param name="chartFile">The path of the chart document.</param>
	/// <param name="levelFile">The path of the level document.</param>
	/// <param name="settings">Optional read settings.</param>
	public static Chart FromFile(string chartFile, string levelFile, LevelReadConfig? settings = null)
		=> FromFileAsync(chartFile, levelFile, settings).GetAwaiter().GetResult();
	/// <summary>
	/// Asynchronously loads a chart from both documents: the <c>chart</c> file and the <c>level</c> file.
	/// </summary>
	public static async Task<Chart> FromFileAsync(string chartFile, string levelFile, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadConfig();
		using FileStream chartFs = File.Open(chartFile, FileMode.Open, FileAccess.Read);
		using FileStream levelFs = File.Open(levelFile, FileMode.Open, FileAccess.Read);
		return await Task.Run(() => FromStream(chartFs, levelFs, settings), cancellationToken);
	}
	/// <summary>
	/// Saves only the <c>chart</c> part of this chart (an array of chart events) to a single file.
	/// Use <see cref="SaveToFile(string, string)"/> to write both documents together.
	/// </summary>
	public void SaveToFile(string filepath, LevelWriteConfig? settings = null)
		=> SaveToFileAsync(filepath, settings).GetAwaiter().GetResult();
	/// <summary>
	/// Asynchronously saves only the <c>chart</c> part of this chart to a single file. Use
	/// <see cref="SaveToFileAsync(string, string, CancellationToken)"/> to write both documents together.
	/// </summary>
	public async Task SaveToFileAsync(string filepath, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteConfig();
		DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
		if (!directory.Exists)
			directory.Create();
		using FileStream stream = File.Open(filepath, FileMode.Create, FileAccess.Write);
		await SaveToStreamAsync(stream, settings, cancellationToken);
	}
	/// <summary>
	/// Saves both documents of this chart: the <c>chart</c> file (an array of chart events) and the
	/// <c>level</c> file (an object of shared events).
	/// </summary>
	public void SaveToFile(string chartFile, string levelFile, LevelWriteConfig? settings = null)
		=> SaveToFileAsync(chartFile, levelFile, settings).GetAwaiter().GetResult();
	/// <summary>
	/// Asynchronously saves both documents of this chart: the <c>chart</c> file and the <c>level</c> file.
	/// </summary>
	public async Task SaveToFileAsync(string chartFile, string levelFile, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteConfig();
		DirectoryInfo directory = new FileInfo(chartFile).Directory ?? new("");
		if (!directory.Exists)
			directory.Create();
		using FileStream chartFs = File.Open(chartFile, FileMode.Create, FileAccess.Write);
		using FileStream levelFs = File.Open(levelFile, FileMode.Create, FileAccess.Write);
		await SaveToStreamAsync(chartFs, levelFs, settings, cancellationToken);
	}
	#endregion
	#region stream
	/// <summary>
	/// Reads a chart from a stream containing only the <c>chart</c> part (an array of chart events).
	/// The <c>level</c> part is not loaded; the default level data applies. Use
	/// <see cref="FromStream(Stream, Stream)"/> to read both documents together.
	/// </summary>
	public static Chart FromStream(Stream chartStream, LevelReadConfig? settings = null)
	{
		settings ??= new LevelReadConfig();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(settings);
		Chart chart = [with("")];
		Level.FileConverter.DeserializeChart(new StreamDataSource(chartStream), options, chart, settings);
		return chart;
	}
	/// <summary>
	/// Asynchronously reads a chart from a stream containing only the <c>chart</c> part. The
	/// <c>level</c> part is not loaded; the default level data applies. Use
	/// <see cref="FromStreamAsync(Stream, Stream, CancellationToken)"/> to read both documents together.
	/// </summary>
	public static Task<Chart> FromStreamAsync(Stream chartStream, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
		=> Task.FromResult(FromStream(chartStream, settings));
	/// <summary>
	/// Reads a chart from both documents: the <c>chart</c> stream (an array of chart events) and the
	/// <c>level</c> stream (an object of shared events).
	/// </summary>
	/// <param name="chartStream">The stream of the chart document.</param>
	/// <param name="levelStream">The stream of the level document.</param>
	/// <param name="settings">Optional read settings.</param>
	public static Chart FromStream(Stream chartStream, Stream levelStream, LevelReadConfig? settings = null)
	{
		settings ??= new LevelReadConfig();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(settings);
		Chart chart = [with("")];
		Level.FileConverter.DeserializeChart(new StreamDataSource(chartStream), options, chart, settings);
		Level.FileConverter.DeserializeLevel(new StreamDataSource(levelStream), options, chart, settings);
		return chart;
	}
	/// <summary>
	/// Asynchronously reads a chart from both documents: the <c>chart</c> stream and the <c>level</c> stream.
	/// </summary>
	public static Task<Chart> FromStreamAsync(Stream chartStream, Stream levelStream, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
		=> Task.FromResult(FromStream(chartStream, levelStream, settings));
	/// <summary>
	/// Saves only the <c>chart</c> part of this chart (an array of chart events) to a stream. Use
	/// <see cref="SaveToStream(Stream, Stream)"/> to write both documents together.
	/// </summary>
	public void SaveToStream(Stream stream, LevelWriteConfig? settings = null)
	{
		settings ??= new LevelWriteConfig();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForWrite(settings);
		using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, options);
		Level.FileConverter.WriteVariantChartsToStream(stream, noIndentScope, this, options);
	}
	/// <summary>
	/// Asynchronously saves only the <c>chart</c> part of this chart to a stream. Use
	/// <see cref="SaveToStreamAsync(Stream, Stream, CancellationToken)"/> to write both documents together.
	/// </summary>
	public Task SaveToStreamAsync(Stream stream, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		SaveToStream(stream, settings);
		return Task.CompletedTask;
	}
	/// <summary>
	/// Saves both documents of this chart: the <c>chart</c> stream (an array of chart events) and the
	/// <c>level</c> stream (an object of shared events).
	/// </summary>
	public void SaveToStream(Stream chartStream, Stream levelStream, LevelWriteConfig? settings = null)
	{
		settings ??= new LevelWriteConfig();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForWrite(settings);
		using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, options);
		Level.FileConverter.WriteVariantChartsToStream(chartStream, noIndentScope, this, options);
		Level.FileConverter.WriteVariantLevelToStream(levelStream, noIndentScope, this, options);
	}
	/// <summary>
	/// Asynchronously saves both documents of this chart: the <c>chart</c> stream and the <c>level</c> stream.
	/// </summary>
	public Task SaveToStreamAsync(Stream chartStream, Stream levelStream, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		SaveToStream(chartStream, levelStream, settings);
		return Task.CompletedTask;
	}
	#endregion
	#region json
	/// <summary>
	/// Reads a chart from a JSON string containing only the <c>chart</c> part (an array of chart
	/// events). The <c>level</c> part is not loaded; the default level data applies. Use
	/// <see cref="FromJsonString(string, string)"/> to read both documents together.
	/// </summary>
	public static Chart FromJsonString(string json, LevelReadConfig? settings = null)
	{
		settings ??= new LevelReadConfig();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(settings);
		Chart chart = [with("")];
		Level.FileConverter.DeserializeChart(new ReadOnlyMemoryDataSource(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(json))), options, chart, settings);
		return chart;
	}
	/// <summary>
	/// Reads a chart from both documents: the <c>chart</c> JSON string (an array of chart events) and
	/// the <c>level</c> JSON string (an object of shared events).
	/// </summary>
	/// <param name="chartJson">The JSON of the chart document.</param>
	/// <param name="levelJson">The JSON of the level document.</param>
	/// <param name="settings">Optional read settings.</param>
	public static Chart FromJsonString(string chartJson, string levelJson, LevelReadConfig? settings = null)
	{
		settings ??= new LevelReadConfig();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(settings);
		Chart chart = [with("")];
		Level.FileConverter.DeserializeChart(new ReadOnlyMemoryDataSource(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(chartJson))), options, chart, settings);
		Level.FileConverter.DeserializeLevel(new ReadOnlyMemoryDataSource(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(levelJson))), options, chart, settings);
		return chart;
	}
	/// <summary>
	/// Reads a chart from a <see cref="JsonDocument"/> containing only the <c>chart</c> part (an
	/// array of chart events). The <c>level</c> part is not loaded; the default level data applies.
	/// Use <see cref="FromJsonDocument(JsonDocument, JsonDocument)"/> to read both documents together.
	/// </summary>
	public static Chart FromJsonDocument(JsonDocument jsonDocument, LevelReadConfig? settings = null)
	{
		settings ??= new LevelReadConfig();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(settings);
		Chart chart = [with("")];
		Level.FileConverter.DeserializeChart(new JsonDocumentDataSource(jsonDocument), options, chart, settings);
		return chart;
	}
	/// <summary>
	/// Reads a chart from both documents: the <c>chart</c> <see cref="JsonDocument"/> (an array of
	/// chart events) and the <c>level</c> <see cref="JsonDocument"/> (an object of shared events).
	/// </summary>
	public static Chart FromJsonDocument(JsonDocument chartDocument, JsonDocument levelDocument, LevelReadConfig? settings = null)
	{
		settings ??= new LevelReadConfig();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(settings);
		Chart chart = [with("")];
		Level.FileConverter.DeserializeChart(new JsonDocumentDataSource(chartDocument), options, chart, settings);
		Level.FileConverter.DeserializeLevel(new JsonDocumentDataSource(levelDocument), options, chart, settings);
		return chart;
	}
	/// <summary>
	/// Serializes only the <c>chart</c> part of this chart (an array of chart events) to a JSON
	/// string. Use <see cref="ToJsonString(out string, out string)"/> to serialize both documents together.
	/// </summary>
	public string ToJsonString(LevelWriteConfig? settings = null)
	{
		settings ??= new LevelWriteConfig();
		using MemoryStream stream = new();
		SaveToStream(stream, settings);
		stream.Seek(0, SeekOrigin.Begin);
		return Encoding.UTF8.GetString(stream.ToArray());
	}
	/// <summary>
	/// Serializes both documents of this chart to JSON strings: the <c>chart</c> part (an array of
	/// chart events) and the <c>level</c> part (an object of shared events).
	/// </summary>
	/// <param name="levelJson">When this method returns, contains the JSON of the level document.</param>
	/// <param name="chartJson">When this method returns, contains the JSON of the chart document.</param>
	/// <param name="settings">Optional write settings.</param>
	public void ToJsonString(out string levelJson, out string chartJson, LevelWriteConfig? settings = null)
	{
		settings ??= new LevelWriteConfig();
		using MemoryStream level = new();
		using MemoryStream chart = new();
		SaveToStream(chart, level, settings);
		chart.Seek(0, SeekOrigin.Begin);
		level.Seek(0, SeekOrigin.Begin);
		chartJson = Encoding.UTF8.GetString(chart.ToArray());
		levelJson = Encoding.UTF8.GetString(level.ToArray());
	}
	/// <summary>
	/// Serializes only the <c>chart</c> part of this chart (an array of chart events) to a
	/// <see cref="JsonDocument"/>. The level document has no <see cref="JsonDocument"/> counterpart
	/// because <see cref="JsonDocument"/> is read-only.
	/// </summary>
	public JsonDocument ToJsonDocument(LevelWriteConfig? settings = null)
	{
		settings ??= new LevelWriteConfig();
		using MemoryStream stream = new();
		SaveToStream(stream, settings);
		stream.Seek(0, SeekOrigin.Begin);
		return JsonDocument.Parse(stream);
	}
	#endregion
}
