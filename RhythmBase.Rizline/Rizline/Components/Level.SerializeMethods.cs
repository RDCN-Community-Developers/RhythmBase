using RhythmBase.Rizline.Converters;
using RhythmBase.Rizline.Events;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Rizline.Components;

partial class Level
{
	public static LevelType LevelType => LevelType.RhythmDoctor;
	private static class FileConverter
	{
		public static void DeserializeChart(IJsonDataSource dataSource, MetadataJsonSerializerOptions options, Level level, LevelReadSettings settings)
		{
			ReadOnlyMemory<byte> jsonData =
					dataSource.CanGetMemoryDirectly
					? dataSource.GetMemory()
					: dataSource.GetMemoryAsync().GetAwaiter().GetResult();
			Utf8JsonReader reader = new Utf8JsonReader(jsonData.Span, new() { AllowTrailingCommas = true });
			reader.Read();
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
			Chart chart = new Chart();
			while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("fileVersion"u8))
					chart.FileVersion = reader.GetInt32();
				else if (propertyName.SequenceEqual("songsName"u8))
					chart.SongsName = reader.GetString() ?? "";
				else if (propertyName.SequenceEqual("chartDelayMs"u8))
					chart.Delay = TimeSpan.FromMilliseconds(reader.GetDouble());
				else if (propertyName.SequenceEqual("offset"u8))
					chart.Offset = TimeSpan.FromMilliseconds(reader.GetDouble());
				else if (propertyName.SequenceEqual("themes"u8))
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
					reader.Read();
					chart.Themes.MainTheme = ConverterHub.Read<Theme>(ref reader, options);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
						chart.Themes.RiztimeThemes.Add(ConverterHub.Read<Theme>(ref reader, options));
				}
				else if (propertyName.SequenceEqual("challengeTimes"u8))
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
					{
						ChallengeTime e = ConverterMap
							.GetConverter(EventType.ChallengeTime)
							.WithReadSettings(settings)
							.ReadProperties(ref reader, options)
							as ChallengeTime
							?? throw new JsonException("Failed to read a ChallengeTime event.");
						chart.ChallengeTimes.Add(e);
					}
				}
				else if (propertyName.SequenceEqual("bPM"u8))
					chart.Bpm = reader.GetSingle();
				else if (propertyName.SequenceEqual("bpmShifts"u8))
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
					{
						BpmShift e = ConverterMap
							.GetConverter(EventType.BpmShift)
							.WithReadSettings(settings)
							.ReadProperties(ref reader, options)
							as BpmShift
							?? throw new JsonException("Failed to read a BpmShift event.");
						chart.BpmShifts.Add(e);
					}
				}
				else if (propertyName.SequenceEqual("lines"u8))
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
						chart.Lines.Add(ConverterHub.Read<Line>(ref reader, options));
				}
				else if (propertyName.SequenceEqual("canvasMoves"u8))
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
					{
						JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
						CanvasMove canvasMove = new();
						while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
						{
							JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
							ReadOnlySpan<byte> cmPropertyName = reader.ValueSpan;
							reader.Read();
							if (cmPropertyName.SequenceEqual("index"u8))
								canvasMove.Index = reader.GetInt32();
							else if (cmPropertyName.SequenceEqual("xPositionKeyPoints"u8))
							{
								while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
								{
									CanvasPosition e = ConverterMap
										.GetConverter(EventType.CanvasPosition)
										.WithReadSettings(settings)
										.ReadProperties(ref reader, options)
										as CanvasPosition
										?? throw new JsonException("Failed to read a CanvasPosition event.");
									canvasMove.XPosition.Add(e);
								}
							}
							else if (cmPropertyName.SequenceEqual("speedKeyPoints"u8))
							{
								while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
								{
									CanvasSpeed e = ConverterMap
										.GetConverter(EventType.CanvasSpeed)
										.WithReadSettings(settings)
										.ReadProperties(ref reader, options)
										as CanvasSpeed
										?? throw new JsonException("Failed to read a CanvasSpeed event.");
									canvasMove.Speed.Add(e);
								}
							}
							else
#if DEBUG
								throw new JsonException($"Unexpected property in canvasMoves: {Encoding.UTF8.GetString(cmPropertyName.ToArray())}");
#else
								reader.Skip();
#endif
						}
					}
				}
				else if (propertyName.SequenceEqual("cameraMove"u8))
				{
					JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
					CameraMove cameraMove = new CameraMove();
					while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
					{
						JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.PropertyName);
						ReadOnlySpan<byte> cmPropertyName = reader.ValueSpan;
						reader.Read();
						if (cmPropertyName.SequenceEqual("scaleKeyPoints"u8))
						{
							while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
							{
								CameraScale e = ConverterMap
									.GetConverter(EventType.CameraScale)
									.WithReadSettings(settings)
									.ReadProperties(ref reader, options)
									as CameraScale
									?? throw new JsonException("Failed to read a CameraScale event.");
								cameraMove.Scales.Add(e);
							}
						}
						else if (cmPropertyName.SequenceEqual("xPositionKeyPoints"u8))
						{
							while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
							{
								CameraPosition e = ConverterMap
									.GetConverter(EventType.CameraPosition)
									.WithReadSettings(settings)
									.ReadProperties(ref reader, options)
									as CameraPosition
									?? throw new JsonException("Failed to read a CameraPosition event.");
								cameraMove.XPosition.Add(e);
							}
						}
						else
#if DEBUG
							throw new JsonException($"Unexpected property in cameraMoves: {Encoding.UTF8.GetString(cmPropertyName.ToArray())}");
#else
							reader.Skip();
#endif
					}
					chart.CameraMove = cameraMove;
				}
				else
#if DEBUG
					throw new JsonException($"Unexpected property in chart: {Encoding.UTF8.GetString(propertyName.ToArray())}");
#else
					reader.Skip();
#endif
			}
			level.Charts.Add(chart);
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
			noIndentScope.WriteNoIndentArrayTo(options, writer, [chart.Themes.MainTheme, .. chart.Themes.RiztimeThemes], ConverterHub.Write);
			writer.WriteEndArray();
			writer.WriteStartArray("challengeTimes"u8);
			noIndentScope.WriteNoIndentArrayTo(options, writer, chart.ChallengeTimes, (w, e, o) => ConverterMap.GetConverter(EventType.ChallengeTime).WithWriteSettings(settings).WriteProperties(w, e, o));
			writer.WriteEndArray();
			writer.WriteNumber("bPM", chart.Bpm);
			writer.WriteStartArray("bpmShifts"u8);
			noIndentScope.WriteNoIndentArrayTo(options, writer, chart.BpmShifts, (w, e, o) => ConverterMap.GetConverter(EventType.BpmShift).WithWriteSettings(settings).WriteProperties(w, e, o));
			writer.WriteEndArray();
			writer.WriteStartArray("lines"u8);
			foreach (Line line in chart.Lines)
				ConverterHub.Write(writer, line, options);
			writer.WriteEndArray();
			writer.WriteStartArray("canvasMoves"u8);
			foreach (CanvasMove canvasMove in chart.CanvasMoves)
			{
				writer.WriteStartObject();
				writer.WriteNumber("index", canvasMove.Index);
				writer.WriteStartArray("xPositionKeyPoints"u8);
				noIndentScope.WriteNoIndentArrayTo(options, writer, canvasMove.XPosition, (w, e, o) => ConverterMap.GetConverter(EventType.CanvasPosition).WithWriteSettings(settings).WriteProperties(w, e, o));
				writer.WriteEndArray();
				writer.WriteStartArray("speedKeyPoints"u8);
				noIndentScope.WriteNoIndentArrayTo(options, writer, canvasMove.Speed, (w, e, o) => ConverterMap.GetConverter(EventType.CanvasSpeed).WithWriteSettings(settings).WriteProperties(w, e, o));
				writer.WriteEndArray();
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
			writer.WriteStartObject("cameraMove"u8);
			writer.WriteStartArray("scaleKeyPoints"u8);
			noIndentScope.WriteNoIndentArrayTo(options, writer, chart.CameraMove.Scales, (w, e, o) => ConverterMap.GetConverter(EventType.CameraScale).WithWriteSettings(settings).WriteProperties(w, e, o));
			writer.WriteEndArray();
			writer.WriteStartArray("xPositionKeyPoints"u8);
			noIndentScope.WriteNoIndentArrayTo(options, writer, chart.CameraMove.XPosition, (w, e, o) => ConverterMap.GetConverter(EventType.CameraPosition).WithWriteSettings(settings).WriteProperties(w, e, o));
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
	public static Task<Level> FromZipAsync(string filepath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
	/// <inheritdoc/>
	public void SaveToZip(string filepath, LevelWriteSettings? settings = null)
			=> SaveToZipAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public Task SaveToZipAsync(string filepath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
	#endregion
	#region json
	/// <inheritdoc/>
	public static Level FromJsonDocument(JsonDocument jsonDocument, LevelReadSettings? settings = null)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public static Level FromJsonString(string json, LevelReadSettings? settings = null)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public JsonDocument ToJsonDocument(LevelWriteSettings? settings = null)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public string ToJsonString(LevelWriteSettings? settings = null)
	{
		throw new NotImplementedException();
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
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(LevelType, settings);
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
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForWrite(LevelType, settings);
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