using RhythmBase.Rizline.Converters;
using RhythmBase.Rizline.Events;
using RhythmBase.Global.Settings;
using System;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Rizline.Components;

partial class Level
{
	private static readonly JsonReaderOptions _readerOptions = new();
	private static class FileConverter
	{
		public static void DeserializeChart(IJsonDataSource dataSource, MetadataJsonSerializerOptions options, Level level, LevelReadSettings settings)
		{
			var seq = dataSource.GetSequence();
			Utf8JsonReader reader = seq.IsSingleSegment
				? new Utf8JsonReader(seq.First.Span, _readerOptions)
				: new Utf8JsonReader(seq, _readerOptions);
			try
			{
			reader.Read();
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
			Chart chart = new();
			while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
			{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
			if (reader.ValueTextEquals("fileVersion"u8) && reader.Read())
				chart.FileVersion = reader.GetInt32();
			else if (reader.ValueTextEquals("songsName"u8) && reader.Read())
				chart.SongsName = reader.GetString() ?? "";
			else if (reader.ValueTextEquals("chartDelayMs"u8) && reader.Read())
				chart.Delay = TimeSpan.FromMilliseconds(reader.GetDouble());
			else if (reader.ValueTextEquals("offset"u8) && reader.Read())
				chart.Offset = TimeSpan.FromMilliseconds(reader.GetDouble());
			else if (reader.ValueTextEquals("themes"u8) && reader.Read())
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
					reader.Read();
					chart.Themes.MainTheme = TypeConverterRegistry.Read<Theme>(ref reader, options);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
						chart.Themes.RiztimeThemes.Add(TypeConverterRegistry.Read<Theme>(ref reader, options));
				}
				else if (reader.ValueTextEquals("challengeTimes"u8) && reader.Read())
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
					{
						ChallengeTime e = EventConverterMap
							.GetConverter(EventType.ChallengeTime)
							.ReadProperties(ref reader, options)
							as ChallengeTime
							?? throw new JsonException("Failed to read a ChallengeTime event.");
						chart.ChallengeTimes.Add(e);
					}
				}
				else if (reader.ValueTextEquals("bPM"u8) && reader.Read())
					chart.Bpm = reader.GetSingle();
				else if (reader.ValueTextEquals("bpmShifts"u8) && reader.Read())
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
					{
						BpmShift e = EventConverterMap
							.GetConverter(EventType.BpmShift)
							.ReadProperties(ref reader, options)
							as BpmShift
							?? throw new JsonException("Failed to read a BpmShift event.");
						chart.BpmShifts.Add(e);
					}
				}
				else if (reader.ValueTextEquals("lines"u8) && reader.Read())
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
						chart.Lines.Add(TypeConverterRegistry.Read<Line>(ref reader, options));
				}
				else if (reader.ValueTextEquals("canvasMoves"u8) && reader.Read())
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
					{
						JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
						CanvasMove canvasMove = new();
						while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
						{
							JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
							if (reader.ValueTextEquals("index"u8) && reader.Read())
								canvasMove.Index = reader.GetInt32();
							else if (reader.ValueTextEquals("xPositionKeyPoints"u8) && reader.Read())
							{
								while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
								{
									CanvasPosition e = EventConverterMap
										.GetConverter(EventType.CanvasPosition)
										.ReadProperties(ref reader, options)
										as CanvasPosition
										?? throw new JsonException("Failed to read a CanvasPosition event.");
									canvasMove.XPosition.Add(e);
								}
							}
							else if (reader.ValueTextEquals("speedKeyPoints"u8) && reader.Read())
							{
								while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
								{
									CanvasSpeed e = EventConverterMap
										.GetConverter(EventType.CanvasSpeed)
										.ReadProperties(ref reader, options)
										as CanvasSpeed
										?? throw new JsonException("Failed to read a CanvasSpeed event.");
									canvasMove.Speed.Add(e);
								}
							}
							else
#if DEBUG
								throw new JsonException($"Unexpected property in canvasMoves: {reader.GetString()}");
#else
								reader.Skip();
#endif
						}
					}
				}
				else if (reader.ValueTextEquals("cameraMove"u8) && reader.Read())
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
					CameraMove cameraMove = new();
					while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
					{
						JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
						if (reader.ValueTextEquals("scaleKeyPoints"u8) && reader.Read())
						{
							while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
							{
								CameraScale e = EventConverterMap
									.GetConverter(EventType.CameraScale)
									.ReadProperties(ref reader, options)
									as CameraScale
									?? throw new JsonException("Failed to read a CameraScale event.");
								cameraMove.Scales.Add(e);
							}
						}
						else if (reader.ValueTextEquals("xPositionKeyPoints"u8) && reader.Read())
						{
							while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
							{
								CameraPosition e = EventConverterMap
									.GetConverter(EventType.CameraPosition)
									.ReadProperties(ref reader, options)
									as CameraPosition
									?? throw new JsonException("Failed to read a CameraPosition event.");
								cameraMove.XPosition.Add(e);
							}
						}
						else
#if DEBUG
							throw new JsonException($"Unexpected property in cameraMoves: {reader.GetString()}");
#else
							reader.Skip();
#endif
					}
					chart.CameraMove = cameraMove;
				}
				else
#if DEBUG
					throw new JsonException($"Unexpected property in chart: {reader.GetString()}");
#else
					reader.Skip();
#endif
			}
			level.Charts.Add(chart);
			}
			catch (JsonException ex) { WrapAndThrow(ex, dataSource, reader.BytesConsumed); }
		}
		private static void WrapAndThrow(JsonException ex, IJsonDataSource dataSource, long bytesConsumed)
		{
			long originalPos = dataSource.MapToInputPosition(bytesConsumed);
			if (originalPos >= 0)
				throw new JsonException($"{ex.Message}\n  at processed byte position {bytesConsumed}, original stream byte position ~{originalPos}", ex);
			throw new JsonException($"{ex.Message}\n  at processed byte position {bytesConsumed}", ex);
		}

		internal static void WriteChartToStream(Stream stream, NoIndentScope noIndentScope, Chart chart, LevelWriteSettings settings, MetadataJsonSerializerOptions options)
		{
			using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
			writer.WriteStartObject();
			writer.WriteNumber("fileVersion", chart.FileVersion);
			writer.WriteString("songsName", chart.SongsName);
			writer.WriteNumber("chartDelayMs", chart.Delay.TotalMilliseconds);
			writer.WriteNumber("offset", chart.Offset.TotalMilliseconds);
			writer.WriteStartArray("themes"u8);
			noIndentScope.WriteNoIndentArrayTo(options, writer, [chart.Themes.MainTheme, .. chart.Themes.RiztimeThemes], TypeConverterRegistry.Write);
			writer.WriteEndArray();
			writer.WriteStartArray("challengeTimes"u8);
			noIndentScope.WriteNoIndentArrayTo(options, writer, chart.ChallengeTimes, (w, e, o) => EventConverterMap.GetConverter(EventType.ChallengeTime).WriteProperties(w, e, o));
			writer.WriteEndArray();
			writer.WriteNumber("bPM", chart.Bpm);
			writer.WriteStartArray("bpmShifts"u8);
			noIndentScope.WriteNoIndentArrayTo(options, writer, chart.BpmShifts, (w, e, o) => EventConverterMap.GetConverter(EventType.BpmShift).WriteProperties(w, e, o));
			writer.WriteEndArray();
			writer.WriteStartArray("lines"u8);
			foreach (Line line in chart.Lines)
				TypeConverterRegistry.Write(writer, line, options);
			writer.WriteEndArray();
			writer.WriteStartArray("canvasMoves"u8);
			foreach (CanvasMove canvasMove in chart.CanvasMoves)
			{
				writer.WriteStartObject();
				writer.WriteNumber("index", canvasMove.Index);
				writer.WriteStartArray("xPositionKeyPoints"u8);
				noIndentScope.WriteNoIndentArrayTo(options, writer, canvasMove.XPosition, (w, e, o) => EventConverterMap.GetConverter(EventType.CanvasPosition).WriteProperties(w, e, o));
				writer.WriteEndArray();
				writer.WriteStartArray("speedKeyPoints"u8);
				noIndentScope.WriteNoIndentArrayTo(options, writer, canvasMove.Speed, (w, e, o) => EventConverterMap.GetConverter(EventType.CanvasSpeed).WriteProperties(w, e, o));
				writer.WriteEndArray();
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
			writer.WriteStartObject("cameraMove"u8);
			writer.WriteStartArray("scaleKeyPoints"u8);
			noIndentScope.WriteNoIndentArrayTo(options, writer, chart.CameraMove.Scales, (w, e, o) => EventConverterMap.GetConverter(EventType.CameraScale).WriteProperties(w, e, o));
			writer.WriteEndArray();
			writer.WriteStartArray("xPositionKeyPoints"u8);
			noIndentScope.WriteNoIndentArrayTo(options, writer, chart.CameraMove.XPosition, (w, e, o) => EventConverterMap.GetConverter(EventType.CameraPosition).WriteProperties(w, e, o));
			writer.WriteEndArray();
			writer.WriteEndObject();
			writer.WriteEndObject();
		}
	}
	#region zip
	/// <inheritdoc/>
	public static Level FromZip(string filepath, LevelReadSettings? settings = null)
			=> FromZipAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromZipAsync(string filepath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadSettings();
		string extension = Path.GetExtension(filepath);
		if (extension is not ".zip" and not ".rlz")
			throw new NotSupportedException($"File type '{extension}' is not supported.");
		DirectoryInfo tempDirectory = new(Path.Combine(
			GlobalSettings.CachePath, GlobalSettings.CacheDirectoryPrefix + Path.GetRandomFileName()));
		tempDirectory.Create();
		try
		{
#if NET8_0_OR_GREATER
			using Stream stream = File.OpenRead(filepath);
			ZipFile.ExtractToDirectory(stream, tempDirectory.FullName, overwriteFiles: true);
#else
			ZipFile.ExtractToDirectory(filepath, tempDirectory.FullName);
#endif
			Level level = await FromDirectoryAsync(tempDirectory.FullName, settings, cancellationToken);
			level.ResolvedPath = Path.GetFullPath(filepath);
			level.Filepath = Path.GetFullPath(filepath);
			return level;
		}
		catch (Exception ex)
		{
			tempDirectory.Delete(true);
			throw new InvalidDataException("Cannot extract the file.", ex);
		}
	}
	/// <inheritdoc/>
	public void SaveToZip(string filepath, LevelWriteSettings? settings = null)
			=> SaveToZipAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToZipAsync(string filepath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteSettings();
		DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
		if (!directory.Exists)
			directory.Create();
		string tempDir = Path.Combine(
			GlobalSettings.CachePath, GlobalSettings.CacheDirectoryPrefix + Path.GetRandomFileName());
		await SaveToDirectoryAsync(tempDir, settings, cancellationToken);
		using Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
		using ZipArchive archive = new(stream, ZipArchiveMode.Create);
		foreach (string file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
		{
#if NET8_0_OR_GREATER
			string entryName = Path.GetRelativePath(tempDir, file).Replace('\\', '/');
#else
			string entryName = file.Substring(tempDir.Length + 1).Replace('\\', '/');
#endif
			archive.CreateEntryFromFile(file, entryName);
		}
		Directory.Delete(tempDir, true);
	}
	#endregion
	#region dir
	/// <inheritdoc/>
	public static Level FromDirectory(string directoryPath, LevelReadSettings? settings = null)
			=> FromDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromDirectoryAsync(string directoryPath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadSettings();
		using FileStream metadataFs = new(Path.Combine(directoryPath, "metadata.json"), FileMode.Open, FileAccess.Read);
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(settings);
		Level level = await FileMainEntryConverter.DeserializeMainEntryAsync<Level>(new StreamDataSource(metadataFs), options, cancellationToken);
		string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
		foreach (string jsonFile in jsonFiles)
		{
			if (Path.GetFileNameWithoutExtension(jsonFile) != "chart") continue;
			//if (!Path.GetFileName(jsonFile).StartsWith("chart")) continue;
			//if (jsonFile == "metadata.json") continue;
			using FileStream jsonFs = new(jsonFile, FileMode.Open, FileAccess.Read);
			FileConverter.DeserializeChart(new StreamDataSource(jsonFs), options, level, settings);
		}
		return level;
	}
	/// <inheritdoc/>
	public void SaveToDirectory(string directoryPath, LevelWriteSettings? settings = null)
			=> SaveToDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToDirectoryAsync(string directoryPath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteSettings();
		if (!Directory.Exists(directoryPath))
			Directory.CreateDirectory(directoryPath);
		using FileStream metadataFs = new(Path.Combine(directoryPath, "metadata.json"), FileMode.Create, FileAccess.Write);
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForWrite(settings);
		FileMainEntryConverter.SerializeMainEntry(this, metadataFs, options);
		using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, options);
		if (Charts.Count == 1)
		{
			using FileStream jsonFs = new(Path.Combine(directoryPath, $"chart.json"), FileMode.Create, FileAccess.Write);
			FileConverter.WriteChartToStream(jsonFs, noIndentScope, Charts[0], settings, options);
		}
		else
			foreach (Chart chart in Charts)
			{
				using FileStream jsonFs = new(Path.Combine(directoryPath, $"chart_{chart.SongsName}.json"), FileMode.Create, FileAccess.Write);
				FileConverter.WriteChartToStream(jsonFs, noIndentScope, chart, settings, options);
			}
	}
	#endregion
}