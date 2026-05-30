using RhythmBase.Global.Components;
using RhythmBase.Global.Converters;
using RhythmBase.Global.Converters.JsonSerialization;
using RhythmBase.Global.Exceptions;
using RhythmBase.Global.Settings;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Converters;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Components;

partial class Level
{
	public static LevelType LevelType => LevelType.RhythmDoctor;
	#region file
	/// <inheritdoc/>
	public static Level FromFile(string filepath, LevelReadSettings? settings = null)
	{
		string extension = Path.GetExtension(filepath);
		if (extension is not ".rdlevel" and not ".json")
		{
			if (extension is ".rdzip" or ".zip")
				throw new NotSupportedException($"File type '{extension}' is not supported. Use {nameof(FromZip)} instead.");
			throw new NotSupportedException($"File type '{extension}' is not supported.");
		}
		return FromFileAsync(filepath, settings).GetAwaiter().GetResult();
	}
	/// <inheritdoc/>
	public static async Task<Level> FromFileAsync(string filepath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadSettings();
		string extension = Path.GetExtension(filepath);
		Level? level;
		if (extension is not ".rdlevel" and not ".json")
		{
			if (extension is ".rdzip" or ".zip")
				throw new NotSupportedException($"File type '{extension}' is not supported. Use {nameof(FromZipAsync)} instead.");
			throw new NotSupportedException($"File type '{extension}' is not supported.");
		}
		using FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
		level = await FromStreamAsync(stream, settings, cancellationToken);
		level.Filepath = level.ResolvedPath = Path.GetFullPath(filepath);
		return level;
	}
	/// <inheritdoc/>
	public void SaveToFile(string filepath, LevelWriteSettings? settings = null)
			=> SaveToFileAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToFileAsync(string filepath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteSettings();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForWrite(LevelType.RhythmDoctor, settings);
		DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
		if (!directory.Exists)
			directory.Create();
		using FileStream stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write);
		stream.SetLength(0);
		await SaveToStreamAsync(stream, settings, cancellationToken);
	}
	#endregion
	#region stream
	/// <inheritdoc/>
	public static Level FromStream(Stream rdlevelStream, LevelReadSettings? settings = null)
			=> FromStreamAsync(rdlevelStream, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromStreamAsync(Stream rdlevelStream, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadSettings();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(LevelType.RhythmDoctor, settings);
		Level? level;
		level = FileMainEntryConverter.DeserializeMainEntry<Level>(new StreamDataSource(rdlevelStream), options);
		// await FileMainEntryConverter.DeserializeMainEntryAsync<Level>(new StreamDataSource(rdlevelStream), options, cancellationToken);
		return level ?? [];
	}
	/// <inheritdoc/>
	public void SaveToStream(Stream stream, LevelWriteSettings? settings = null)
			=> SaveToStreamAsync(stream, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToStreamAsync(Stream stream, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteSettings();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForWrite(LevelType.RhythmDoctor, settings);
		FileMainEntryConverter.SerializeMainEntry(this, stream, options);
	}
	#endregion
	#region zip
	/// <inheritdoc/>
	public static Level FromZip(string filepath, LevelReadSettings? settings = null)
	{
		string extension = Path.GetExtension(filepath);
		if (extension is not ".rdzip" and not ".zip")
		{
			if (extension is ".rdlevel" or ".json")
				throw new NotSupportedException($"File type '{extension}' is not supported. Use {nameof(FromFile)} instead.");
			throw new RhythmBaseException($"File type '{extension}' is not supported.");
		}
		return FromZipAsync(filepath, settings).GetAwaiter().GetResult();
	}

	/// <inheritdoc/>
	public static async Task<Level> FromZipAsync(string filepath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadSettings();
		string extension = Path.GetExtension(filepath);
		Level? level;
		if (extension is not ".rdzip" and not ".zip")
		{
			if (extension is ".rdlevel" or ".json")
				throw new NotSupportedException($"File type '{extension}' is not supported. Use {nameof(FromFileAsync)} instead.");
			throw new RhythmBaseException($"File type '{extension}' is not supported.");
		}
		switch (settings.ZipFileProcessMethod)
		{
			case ZipFileProcessMethod.AllFiles:
				DirectoryInfo tempDirectory = new(Path.Combine(GlobalSettings.CachePath, "RhythmBaseTemp_Zip_" + Path.GetRandomFileName()));
				tempDirectory.Create();
				try
				{
#if NET8_0_OR_GREATER
					using Stream stream = File.OpenRead(filepath);
					ZipFile.ExtractToDirectory(stream, tempDirectory.FullName, overwriteFiles: true);
#elif NETSTANDARD2_0_OR_GREATER
                    ZipFile.ExtractToDirectory(filepath, tempDirectory.FullName);
#endif
					string? rdlevelPath = null;
					foreach (FileInfo? file in tempDirectory.GetFiles())
					{
						if (file.Name == "main.rdlevel")
						{
							rdlevelPath = file.FullName;
							break;
						}
					}
					if (rdlevelPath == null)
						throw new RhythmBaseException("No RDLevel file has been found.");
					level = await FromFileAsync(rdlevelPath, settings, cancellationToken);
					level.ResolvedPath = Path.GetFullPath(rdlevelPath);
					level.Filepath = Path.GetFullPath(filepath);
					level.isZip = true;
					level.isExtracted = true;
				}
				catch (Exception ex2)
				{
					tempDirectory.Delete(true);
#if DEBUG
					throw;
#else
                    throw new RhythmBaseException("Cannot extract the file.", ex2);
#endif
				}
				break;
			case ZipFileProcessMethod.LevelFileOnly:
				try
				{
					using FileStream zipStream = new(filepath, FileMode.Open, FileAccess.Read);
					using ZipArchive archive = new(zipStream, ZipArchiveMode.Read);
					ZipArchiveEntry? entry = archive.GetEntry("main.rdlevel") ?? throw new RhythmBaseException("Cannot find the level file.");
					using Stream stream = entry.Open();
					level = await FromStreamAsync(stream, settings, cancellationToken);
					level.Filepath = Path.GetFullPath(filepath);
					level.isZip = true;
					level.isExtracted = false;
				}
				catch (Exception ex2)
				{
					throw new RhythmBaseException("Cannot extract the file.", ex2);
				}
				break;
			default:
				throw new RhythmBaseException(extension + " is not supported.");
		}
		return level;
	}
	/// <inheritdoc/>
	public void SaveToZip(string filepath, LevelWriteSettings? settings = null)
	{
		SaveToZipAsync(filepath, settings).GetAwaiter().GetResult();
	}
	/// <inheritdoc/>
	public async Task SaveToZipAsync(string filepath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrEmpty(this.ResolvedDirectory))
			throw new NotImplementedException();
		settings ??= new LevelWriteSettings();
		HashSet<FileReference> fileReferences = [];
		void referenceDelegate(object? level, FileReferenceArgs args)
		{
			fileReferences.Add(args.Reference);
		}
		settings.FileReferenceEncountered += referenceDelegate;
		bool loadAssets = settings.LoadAssets;
		settings.LoadAssets = true;
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForWrite(LevelType.RhythmDoctor, settings);
		DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
		if (!directory.Exists)
			directory.Create();
		using Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
		ZipArchive archive = new(stream, ZipArchiveMode.Create);
		ZipArchiveEntry entry = archive.CreateEntry("main.rdlevel");
		using (Stream rdlevelStream = entry.Open())
		{
			await Task.Run(() => SaveToStream(rdlevelStream, settings), cancellationToken);
		}
		settings.FileReferenceEncountered -= referenceDelegate;
		foreach (var file in fileReferences)
		{
			archive.CreateEntryFromFile(Path.Combine(ResolvedDirectory, file.Path), Path.GetFileName(file.Path));
		}
		archive.Dispose();
		settings.LoadAssets = loadAssets;
	}
	#endregion zip
	#region json
	/// <inheritdoc/>
	public static Level FromJsonString(string json, LevelReadSettings? settings = null)
	{
		settings ??= new LevelReadSettings();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(LevelType.RhythmDoctor, settings);
		Level? level;
		level = FileMainEntryConverter.DeserializeMainEntry<Level>(new ReadOnlyMemoryDataSource(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(json))), options);
		return level ?? [];
	}
	/// <inheritdoc/>
	public static Level FromJsonDocument(JsonDocument jsonDocument, LevelReadSettings? settings = null)
	{
		settings ??= new LevelReadSettings();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForRead(LevelType.RhythmDoctor, settings);
		Level? level;
		level = FileMainEntryConverter.DeserializeMainEntry<Level>(new JsonDocumentDataSource(jsonDocument), options);
		return level ?? [];
	}
	/// <summary>
	/// Serializes the current level to a JSON string.
	/// </summary>
	/// <param name="settings">Optional settings for writing the level. If null, default settings are used.</param>
	/// <returns>A JSON string representing the current level.</returns>
	public string ToJsonString(LevelWriteSettings? settings = null)
	{
		settings ??= new LevelWriteSettings();
		MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForWrite(LevelType.RhythmDoctor, settings);
		string json;
		using (MemoryStream stream = new())
		{
			SaveToStream(stream, settings);
			stream.Seek(0, SeekOrigin.Begin);
			json = Encoding.UTF8.GetString(stream.ToArray());
		}
		return json;
	}
	/// <summary>
	/// Serializes the current level to a <see cref="JsonDocument"/>.
	/// </summary>
	/// <param name="settings">
	/// Optional settings for writing the level. If <c>null</c>, default settings are used.
	/// </param>
	/// <returns>
	/// A <see cref="JsonDocument"/> representing the current level in JSON format.
	/// </returns>
	public JsonDocument ToJsonDocument(LevelWriteSettings? settings = null)
	{
		settings ??= new LevelWriteSettings();
		string json;
		MemoryStream stream = new();
		SaveToStream(stream, settings);
		stream.Seek(0, SeekOrigin.Begin);
		json = Encoding.UTF8.GetString(stream.ToArray());
		return JsonDocument.Parse(json);
	}
	#endregion
}
