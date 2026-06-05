using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;
using RhythmBase.Global.Settings;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;

namespace RhythmBase.BeatBlock.Components;

partial class Level
{
	private static readonly BaseEventConverter baseEventConverter = new();
	private static class FileConverter
	{
		public static void DeserializeLevel(IJsonDataSource dataSource, MetadataJsonSerializerOptions options, Chart variant, LevelReadSettings settings)
		{
			ReadOnlyMemory<byte> jsonData =
					dataSource.CanGetMemoryDirectly
					? dataSource.GetMemory()
					: dataSource.GetMemoryAsync().GetAwaiter().GetResult();
			Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
			reader.Read();
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
					break;
				ReadOnlySpan<byte> propertyName = reader.ValueSpan;
				reader.Read();
				if (propertyName.SequenceEqual("events"u8))
					foreach (IBaseEvent e in DeserializeEvents(ref reader, options, settings))
						variant.Add(e);
				else
				{
					reader.Skip();
				}
			}
		}
		public static void DeserializeChart(IJsonDataSource dataSource, MetadataJsonSerializerOptions options, Chart variant, LevelReadSettings settings)
		{
			ReadOnlyMemory<byte> jsonData =
					dataSource.CanGetMemoryDirectly
					? dataSource.GetMemory()
					: dataSource.GetMemoryAsync().GetAwaiter().GetResult();
			Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
			reader.Read();
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
			foreach (IBaseEvent e in DeserializeEvents(ref reader, options, settings))
			{
				variant.Add(e);
			}
			reader.Read();
		}
		public static void DeserializeTag(IJsonDataSource dataSource, MetadataJsonSerializerOptions options, TagEventCollection collection, LevelReadSettings settings)
		{
			ReadOnlyMemory<byte> jsonData =
					dataSource.CanGetMemoryDirectly
					? dataSource.GetMemory()
					: dataSource.GetMemoryAsync().GetAwaiter().GetResult();
			Utf8JsonReader reader = new(jsonData.Span, new() { AllowTrailingCommas = true });
			reader.Read();
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
			foreach (IBaseEvent e in DeserializeEvents(ref reader, options, settings))
			{
				collection.Add(e);
			}
			reader.Read();
		}
		private static List<IBaseEvent> DeserializeEvents(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options, LevelReadSettings settings)
		{
			List<IBaseEvent> events = [];
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
			int index = 0;
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndArray)
					break;
				IBaseEvent? e = null;
#if DEBUG
				try
				{
					e = baseEventConverter.Read(ref reader, typeof(IBaseEvent), options);
					index++;
				}
				catch (Exception ex)
				{
					Debug.Print($"Current index: {index}");
					throw;
				}
#else
                Utf8JsonReader checkpoint = reader;
                try
                {
                    e = baseEventConverter.Read(ref reader, typeof(IBaseEvent), options);
                }
                catch (JsonException)
                {
                    level.Dispose();
                    throw;
                }
                catch (Exception ex)
                {
                    JsonElement element = JsonElement.ParseValue(ref checkpoint);
                    settings.HandleUnreadableEvent(element, ex.Message);
                    continue;
                }
#endif
				if (e == null)
					continue;
				events.Add(e);
			}
			return events;
		}
		public static void WriteManifestToStream(Stream stream, Level level, MetadataJsonSerializerOptions options)
		{
			using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
			TypeConverterRegistry.Write(writer, level, options);
			writer.Flush();
		}
		public static void WriteVariantLevelToStream(Stream stream, NoIndentScope noIndentScope, Chart level, MetadataJsonSerializerOptions options)
		{
			using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
			writer.WriteStartObject();
			writer.WriteStartArray("events"u8);
			noIndentScope.WriteNoIndentArrayTo(options.WriteIndented, false, writer, level, baseEventConverter.Write);
			writer.WriteEndArray();
			writer.WriteEndObject();
			writer.Flush();
		}
		public static void WriteVariantChartsToStream(Stream stream, NoIndentScope noIndentScope, Chart variant, MetadataJsonSerializerOptions options)
		{
			using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
			writer.WriteStartArray();
			noIndentScope.WriteNoIndentArrayTo(options.WriteIndented, false, writer, variant, baseEventConverter.Write);
			writer.WriteEndArray();
			writer.Flush();
		}
		public static void WriteTagEventsToStream(Stream stream, NoIndentScope noIndentScope, TagEventCollection collection, MetadataJsonSerializerOptions options)
		{
			using Utf8JsonWriter writer = new(stream, new() { Indented = options.JsonSerializerOptions.WriteIndented });
			writer.WriteStartArray();
			noIndentScope.WriteNoIndentArrayTo(options.WriteIndented, false, writer, collection, baseEventConverter.Write);
			writer.WriteEndArray();
			writer.Flush();
		}
	}
	#region dir
	/// <inheritdoc/>
	public static Level FromDirectory(string directoryPath, LevelReadSettings? settings = null)
			=> FromDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromDirectoryAsync(string directoryPath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadSettings();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(settings);
		Level? level;
		string manifestFilePath = Path.Combine(directoryPath, "manifest.json");
		using FileStream manifestFs = File.Open(manifestFilePath, FileMode.Open, FileAccess.Read);
		level = await FileMainEntryConverter.DeserializeMainEntryAsync<Level>(new StreamDataSource(manifestFs), options, cancellationToken);
		string defaultLevelFile = Path.Combine(directoryPath, "level.json");
		if (File.Exists(defaultLevelFile))
		{
			using FileStream levelFs = File.Open(defaultLevelFile, FileMode.Open, FileAccess.Read);
			FileConverter.DeserializeLevel(new StreamDataSource(levelFs), options, level.Variants.Default, settings);
		}
		foreach (Chart variant in level.Variants)
		{
			if (!string.IsNullOrEmpty(variant.LevelFile))
			{
				string levelFile = Path.Combine(directoryPath, variant.LevelFile);
				if (File.Exists(levelFile))
				{
					using FileStream levelFsVariant = File.Open(levelFile, FileMode.Open, FileAccess.Read);
					FileConverter.DeserializeLevel(new StreamDataSource(levelFsVariant), options, variant, settings);
				}
			}
			string chartFile = Path.Combine(directoryPath, $"chart-{variant.Name}.json");
			using FileStream chartFs = File.Open(chartFile, FileMode.Open, FileAccess.Read);
			FileConverter.DeserializeChart(new StreamDataSource(chartFs), options, variant, settings);
		}
		if (Directory.Exists(Path.Combine(directoryPath, "tags")))
		{
			string[] tags = Directory.GetFiles(Path.Combine(directoryPath, "tags"), "*.json");
			foreach (string tagFile in tags)
			{
				TagEventCollection collection = [];
				using FileStream tagFs = File.Open(tagFile, FileMode.Open, FileAccess.Read);
				FileConverter.DeserializeTag(new StreamDataSource(tagFs), options, collection, settings);
				string tagName = Path.GetFileNameWithoutExtension(tagFile);
				level.TagEvents[tagName] = collection;
			}
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
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForWrite(settings);
		using NoIndentScope noIndentScope = new(options.JsonSerializerOptions.Encoder, options);
		string manifestFilePath = Path.Combine(directoryPath, "manifest.json");
		if (!Directory.Exists(manifestFilePath))
			Directory.CreateDirectory(directoryPath);
		using FileStream manifestFs = File.Open(manifestFilePath, FileMode.Create, FileAccess.Write);
		FileMainEntryConverter.SerializeMainEntry(this, manifestFs, options);
		string defaultLevelFile = Path.Combine(directoryPath, "level.json");
		using FileStream levelFs = File.Open(defaultLevelFile, FileMode.Create, FileAccess.Write);
		if (this.Variants.Any(i => i.IsUsingDefaultLevel))
			FileConverter.WriteVariantLevelToStream(levelFs, noIndentScope, this.Variants.Default, options);
		foreach (Chart variant in Variants)
		{
			if (!variant.IsUsingDefaultLevel)
			{
				string levelFile = Path.Combine(directoryPath, variant.LevelFile);
				using FileStream levelFsVariant = File.Open(levelFile, FileMode.Create, FileAccess.Write);
				FileConverter.WriteVariantLevelToStream(levelFsVariant, noIndentScope, variant, options);
			}
			string chartFile = Path.Combine(directoryPath, $"chart-{variant.Name}.json");
			using FileStream chartFs = File.Open(chartFile, FileMode.Create, FileAccess.Write);
			FileConverter.WriteVariantChartsToStream(chartFs, noIndentScope, variant, options);
		}
		if (this.TagEvents.Count > 0)
		{
			string tagsDir = Path.Combine(directoryPath, "tags");
			Directory.CreateDirectory(tagsDir);
			foreach (var tag in TagEvents)
			{
				string tagFile = Path.Combine(tagsDir, $"{tag.Key}.json");
				using FileStream tagFs = File.Open(tagFile, FileMode.Create, FileAccess.Write);
				FileConverter.WriteTagEventsToStream(tagFs, noIndentScope, tag.Value, options);
			}
		}
	}
	#endregion
	#region zip
	/// <inheritdoc/>
	public static Level FromZip(string filepath, LevelReadSettings? settings = null)
			=> FromZipAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromZipAsync(string filepath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadSettings();
		string extension = Path.GetExtension(filepath);
		if (extension is not ".zip" and not ".bbz")
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
			level.isZip = true;
			level.isExtracted = true;
			return level;
		}
		catch (Exception ex)
		{
			tempDirectory.Delete(true);
			throw new RhythmBaseException("Cannot extract the file.", ex);
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
}
