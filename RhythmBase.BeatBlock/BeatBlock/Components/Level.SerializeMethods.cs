using RhythmBase.BeatBlock.Events;
using RhythmBase.BeatBlock.Serialization;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using RhythmBase.Global;

namespace RhythmBase.BeatBlock.Components;

partial class Level
{
	private static readonly BaseEventConverter baseEventConverter = new();
	private static readonly JsonReaderOptions _readerOptions = new();
	internal static class FileConverter
	{
		public static void DeserializeLevel(IJsonDataSource dataSource, MetadataJsonSerializerOptions options, Chart variant, LevelReadConfig settings)
		{
			var seq = dataSource.GetSequence();
			Utf8JsonReader reader = seq.IsSingleSegment
				? new Utf8JsonReader(seq.First.Span, _readerOptions)
				: new Utf8JsonReader(seq, _readerOptions);
			try
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartObject);
				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndObject)
						break;
					if (reader.ValueTextEquals("events"u8) && reader.Read())
						foreach (IBaseEvent e in DeserializeEvents(ref reader, options, settings))
							variant.Add(e);
					else
					{
						reader.Skip();
					}
				}
			}
			catch { throw; }
		}
		public static void DeserializeChart(IJsonDataSource dataSource, MetadataJsonSerializerOptions options, Chart variant, LevelReadConfig settings)
		{
			var seq = dataSource.GetSequence();
			Utf8JsonReader reader = seq.IsSingleSegment
				? new Utf8JsonReader(seq.First.Span, _readerOptions)
				: new Utf8JsonReader(seq, _readerOptions);
			try
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				foreach (IBaseEvent e in DeserializeEvents(ref reader, options, settings))
				{
					variant.Add(e);
				}
				reader.Read();
			}
			catch { throw; }
		}
		public static void DeserializeTag(IJsonDataSource dataSource, MetadataJsonSerializerOptions options, TagEventCollection collection, LevelReadConfig settings)
		{
			var seq = dataSource.GetSequence();
			Utf8JsonReader reader = seq.IsSingleSegment
				? new Utf8JsonReader(seq.First.Span, _readerOptions)
				: new Utf8JsonReader(seq, _readerOptions);
			try
			{
				reader.Read();
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				foreach (IBaseEvent e in DeserializeEvents(ref reader, options, settings))
				{
					collection.Add(e);
				}
				reader.Read();
			}
			catch { throw; }
		}
		private static List<IBaseEvent> DeserializeEvents(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options, LevelReadConfig settings)
		{
			List<IBaseEvent> events = [];
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndArray)
					break;
				Utf8JsonReader checkpoint = reader;
				IBaseEvent? e;
				try
				{
					e = baseEventConverter.Read(ref reader, typeof(IBaseEvent), options);
				}
				catch (JsonException)
				{
					throw;
				}
				catch (Exception ex)
				{
					JsonElement element = JsonElement.ParseValue(ref checkpoint);
					settings.OnUnreadableEventEncountered(null, element, ex.Message);
					continue;
				}
				if (e != null)
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
	public static Level FromDirectory(string directoryPath, LevelReadConfig? settings = null)
			=> FromDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromDirectoryAsync(string directoryPath, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadConfig();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(settings);
		Level? level;
		string manifestFilePath = Path.Combine(directoryPath, "manifest.json");
		if(!File.Exists(manifestFilePath))
		{
			var dirs = Directory.GetDirectories(directoryPath);
			if(dirs.Length == 1)
			{
				manifestFilePath = Path.Combine(dirs[0], "manifest.json");
				if (!File.Exists(manifestFilePath))
					throw new FileNotFoundException($"Manifest file not found in directory '{directoryPath}' or its only subdirectory.");
				directoryPath = dirs[0];
			}
			else
				throw new FileNotFoundException($"Manifest file not found in directory '{directoryPath}'.");			
		}
		using FileStream manifestFs = File.Open(manifestFilePath, FileMode.Open, FileAccess.Read);
		level = await FileMainEntryConverter.DeserializeMainEntryAsync<Level>(new StreamDataSource(manifestFs), options, cancellationToken);
		options.Version = level.Properties.FormatVersion;
		string defaultLevelFile = Path.Combine(directoryPath, "level.json");
		if (File.Exists(defaultLevelFile))
		{
			using FileStream levelFs = File.Open(defaultLevelFile, FileMode.Open, FileAccess.Read);
			FileConverter.DeserializeLevel(BeatBlockUpgrader.Wrap(new StreamDataSource(levelFs), options), options, level.Variants.Default, settings);
		}
		foreach (Chart variant in level.Variants)
		{
			if (!string.IsNullOrEmpty(variant.LevelFile))
			{
				string levelFile = Path.Combine(directoryPath, variant.LevelFile);
				if (File.Exists(levelFile))
				{
					using FileStream levelFsVariant = File.Open(levelFile, FileMode.Open, FileAccess.Read);
					FileConverter.DeserializeLevel(BeatBlockUpgrader.Wrap(new StreamDataSource(levelFsVariant), options), options, variant, settings);
				}
			}
			string chartFile = Path.Combine(directoryPath, ChartNaming.Instance.GetFileName(variant.Name));
			if (options.Strictness == JsonStrictness.Strict || File.Exists(chartFile))
			{
				using FileStream chartFs = File.Open(chartFile, FileMode.Open, FileAccess.Read);
				FileConverter.DeserializeChart(BeatBlockUpgrader.Wrap(new StreamDataSource(chartFs), options), options, variant, settings);
			}
		}
		if (Directory.Exists(Path.Combine(directoryPath, "tags")))
		{
			string[] tags = Directory.GetFiles(Path.Combine(directoryPath, "tags"), "*.json");
			foreach (string tagFile in tags)
			{
				TagEventCollection collection = [];
				using FileStream tagFs = File.Open(tagFile, FileMode.Open, FileAccess.Read);
				FileConverter.DeserializeTag(BeatBlockUpgrader.Wrap(new StreamDataSource(tagFs), options), options, collection, settings);
				string tagName = Path.GetFileNameWithoutExtension(tagFile);
				level.TagEvents[tagName] = collection;
			}
		}
		return level;
	}
	/// <inheritdoc/>
	public void SaveToDirectory(string directoryPath, LevelWriteConfig? settings = null)
			=> SaveToDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToDirectoryAsync(string directoryPath, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteConfig();
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
			string chartFile = Path.Combine(directoryPath, ChartNaming.Instance.GetFileName(variant.Name));
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
	public static Level FromZip(string filepath, LevelReadConfig? settings = null)
			=> FromZipAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromZipAsync(string filepath, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		string extension = Path.GetExtension(filepath);
		if (extension is not ".zip")
			throw new NotSupportedException($"File type '{extension}' is not supported.");
		using FileStream stream = File.OpenRead(filepath);
		Level level = await FromZipAsync(stream, settings, cancellationToken);
		level.ResolvedPath = Path.GetFullPath(filepath);
		level.Filepath = Path.GetFullPath(filepath);
		return level;
	}
	/// <inheritdoc/>
	public static Task<Level> FromZip(Stream zipStream, LevelReadConfig? settings = null)
			=> FromZipAsync(zipStream, settings);
	/// <inheritdoc/>
	public static async Task<Level> FromZipAsync(Stream zipStream, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadConfig();
		DirectoryInfo tempDirectory = new(Path.Combine(
			Config.CachePath, Config.CacheDirectoryPrefix + Path.GetRandomFileName()));
		tempDirectory.Create();
		try
		{
#if NET8_0_OR_GREATER
			ZipFile.ExtractToDirectory(zipStream, tempDirectory.FullName, overwriteFiles: true);
#else
			using (ZipArchive archive = new(zipStream, ZipArchiveMode.Read))
			{
				foreach (ZipArchiveEntry entry in archive.Entries)
				{
					string entryPath = Path.Combine(tempDirectory.FullName, entry.FullName);
					string? entryDir = Path.GetDirectoryName(entryPath);
					if (!string.IsNullOrEmpty(entryDir))
						Directory.CreateDirectory(entryDir);
					if (!string.IsNullOrEmpty(entry.Name))
						entry.ExtractToFile(entryPath, overwrite: true);
				}
			}
#endif
			Level level = await FromDirectoryAsync(tempDirectory.FullName, settings, cancellationToken);
			level.ResolvedDirectory = tempDirectory.FullName;
			level.isZip = true;
			level.isExtracted = true;
			return level;
		}
		catch
		{
			tempDirectory.Delete(true);
			throw;
		}
	}
	/// <inheritdoc/>
	public void SaveToZip(string filepath, LevelWriteConfig? settings = null)
			=> SaveToZipAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToZipAsync(string filepath, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
		if (!directory.Exists)
			directory.Create();
		using FileStream stream = new(filepath, FileMode.Create, FileAccess.Write);
		await SaveToZipAsync(stream, settings, cancellationToken);
	}
	/// <inheritdoc/>
	public void SaveToZip(Stream zipStream, LevelWriteConfig? settings = null)
			=> SaveToZipAsync(zipStream, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToZipAsync(Stream zipStream, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteConfig();
		string tempDir = Path.Combine(
			Config.CachePath, Config.CacheDirectoryPrefix + Path.GetRandomFileName());
		await SaveToDirectoryAsync(tempDir, settings, cancellationToken);
		try
		{
			using ZipArchive archive = new(zipStream, ZipArchiveMode.Create, leaveOpen: true);
			foreach (string file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
			{
#if NET8_0_OR_GREATER
				string entryName = Path.GetRelativePath(tempDir, file).Replace('\\', '/');
#else
				string entryName = file.Substring(tempDir.Length + 1).Replace('\\', '/');
#endif
				archive.CreateEntryFromFile(file, entryName);
			}
		}
		finally
		{
			Directory.Delete(tempDir, true);
		}
	}
	#endregion
}
