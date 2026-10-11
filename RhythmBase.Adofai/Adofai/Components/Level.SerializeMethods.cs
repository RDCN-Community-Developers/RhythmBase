using System.IO.Compression;
using System.Text;
using System.Text.Json;
using RhythmBase.Adofai.Serialization;

namespace RhythmBase.Adofai.Components;

partial class Level
{
	#region zip
	/// <inheritdoc/>
	public static Level FromZip(string filepath, LevelReadConfig? settings = null)
			=> FromZipAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static Task<Level> FromZipAsync(string filepath, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
	/// <inheritdoc/>
	public static Task<Level> FromZip(Stream zipStream, LevelReadConfig? settings = null)
			=> FromZipAsync(zipStream, settings);
	/// <inheritdoc/>
	public static async Task<Level> FromZipAsync(Stream zipStream, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadConfig();
		using ZipArchive archive = new(zipStream, ZipArchiveMode.Read);
		ZipArchiveEntry? entry = null;
		foreach (ZipArchiveEntry e in archive.Entries)
		{
			if (e.FullName.EndsWith(".adofai"))
			{
				entry = e;
				break;
			}
		}
		if (entry is null)
			throw new FileNotFoundException("No Adofai file has been found.");
		using Stream stream = entry.Open();
		Level level = await FromStreamAsync(stream, settings, cancellationToken);
		level.isZip = true;
		level.isExtracted = false;
		return level;
	}
	/// <inheritdoc/>
	public void SaveToZip(string filepath, LevelWriteConfig? settings = null)
			=> SaveToZipAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToZipAsync(string filepath, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteConfig();
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
		string directoryPath = !string.IsNullOrWhiteSpace(settings.ResolvedDirectory)
			? settings.ResolvedDirectory
			: !string.IsNullOrWhiteSpace(ResolvedDirectory) ? ResolvedDirectory : "";
		if (string.IsNullOrWhiteSpace(directoryPath))
			throw new InvalidOperationException($"Cannot save to zip because the level has no resolved directory and no directory is specified in the {nameof(settings)}.{nameof(LevelWriteConfig.ResolvedDirectory)}.");
		DirectoryInfo directory = new(directoryPath);
		HashSet<FileReference> files = [];
		void AddFileReference(object? sender, FileReferenceArgs args) => files.Add(args.Reference);
		settings.FileReferenceEncountered += AddFileReference;
		bool loadAssets = settings.LoadAssets;
		settings.LoadAssets = true;
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(directory.FullName, settings);
		using ZipArchive archive = new(zipStream, ZipArchiveMode.Create, leaveOpen: true);
		ZipArchiveEntry entry = archive.CreateEntry("main.adofai");
		using (Stream adofaiStream = entry.Open())
			await Task.Run(() => FileMainEntryConverter.SerializeMainEntry(this, adofaiStream, options), cancellationToken);
		foreach (var file in files)
		{
			string fullPath = Path.Combine(directory.FullName, file.Path);
			if (!File.Exists(fullPath))
				throw new FileNotFoundException($"Referenced file '{file.Path}' not found in the resolved directory.");
			archive.CreateEntryFromFile(fullPath, Path.GetFileName(file.Path));
		}
		settings.FileReferenceEncountered -= AddFileReference;
		settings.LoadAssets = loadAssets;
	}
	#endregion
	#region directory
	/// <inheritdoc/>
	public static Level FromDirectory(string directoryPath, LevelReadConfig? settings = null)
		=> FromFile(Path.Combine(directoryPath, ChartNaming.Instance.GetFileName("main")), settings);
	/// <inheritdoc/>
	public static async Task<Level> FromDirectoryAsync(string directoryPath, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
		=> await FromFileAsync(Path.Combine(directoryPath, ChartNaming.Instance.GetFileName("main")), settings, cancellationToken);
	/// <inheritdoc/>
	public void SaveToDirectory(string directoryPath, LevelWriteConfig? settings = null)
		=> SaveToFile(Path.Combine(directoryPath, ChartNaming.Instance.GetFileName("main")), settings);
	/// <inheritdoc/>
	public Task SaveToDirectoryAsync(string directoryPath, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
		=> SaveToFileAsync(Path.Combine(directoryPath, ChartNaming.Instance.GetFileName("main")), settings, cancellationToken);
	#endregion
	#region file
	/// <inheritdoc/>
	public static Level FromFile(string filepath, LevelReadConfig? settings = null)
			=> FromFileAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromFileAsync(string filepath, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadConfig();
		string extension = Path.GetExtension(filepath);
		Level? level;
		if (extension != ".zip")
		{
			if (extension != ".adofai")
				throw new NotSupportedException("File not supported.");
			using FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
			MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(Path.GetDirectoryName(Path.GetFullPath(filepath)), settings);
			level = FileMainEntryConverter.DeserializeMainEntry<Level>(AdofaiUpgrader.Wrap(new StreamDataSource(stream), options), options);
			level.Filepath = level.ResolvedPath = Path.GetFullPath(filepath);
			if (ChartNaming.Instance.TryGetChartName(Path.GetFileName(filepath), out string chartName))
				level.Name = chartName;
			return level;
		}
		switch (settings.ZipProcessingMode)
		{
			case ZipProcessingMode.AllEntries:
				DirectoryInfo tempDirectory = new(Path.Combine(Global.Config.CachePath, "RhythmBaseTemp_" + Path.GetRandomFileName()));
				tempDirectory.Create();
				try
				{
#if NET8_0_OR_GREATER
					using Stream stream = File.OpenRead(filepath);
					ZipFile.ExtractToDirectory(stream, tempDirectory.FullName, overwriteFiles: true);
#elif NETSTANDARD2_0_OR_GREATER
                    ZipFile.ExtractToDirectory(filepath, tempDirectory.FullName);
#endif
					string? adlevelPath = null;
					foreach (FileInfo file in tempDirectory.GetFiles())
					{
						if (file.Extension == ".adofai")
						{
							adlevelPath = file.FullName;
							break;
						}
					}
					if (adlevelPath == null)
						throw new FileNotFoundException("No Adofai file has been found.");
					level = FromFile(adlevelPath, settings);
					level.ResolvedPath = Path.GetFullPath(adlevelPath);
					level.Filepath = Path.GetFullPath(filepath);
					level.isZip = true;
					level.isExtracted = true;
				}
				catch (Exception ex2)
				{
					tempDirectory.Delete(true);
					throw new InvalidDataException("Cannot extract the file.", ex2);
				}
				break;
			case ZipProcessingMode.RootEntriesOnly:
				try
				{
					using FileStream zipStream = new(filepath, FileMode.Open, FileAccess.Read);
					using ZipArchive archive = new(zipStream, ZipArchiveMode.Read);
					ZipArchiveEntry? entry = archive.GetEntry("main.rdlevel") ?? throw new FileNotFoundException("Cannot find the level file.");
					using Stream stream = entry.Open();
					level = await FromStreamAsync(stream, settings, cancellationToken);
					level.Filepath = Path.GetFullPath(filepath);
					level.isZip = true;
					level.isExtracted = false;
				}
				catch (Exception ex2)
				{
					throw new InvalidDataException("Cannot extract the file.", ex2);
				}
				break;
			default:
				throw new NotSupportedException(extension + " is not supported.");
		}
		return level;
	}
	/// <inheritdoc/>
	public void SaveToFile(string filepath, LevelWriteConfig? settings = null)
			=> SaveToFileAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToFileAsync(string filepath, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteConfig();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(Path.GetDirectoryName(filepath), settings);
		using FileStream stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write);
		stream.SetLength(0);
		await Task.Run(() => FileMainEntryConverter.SerializeMainEntry(this, stream, options), cancellationToken);
	}
	#endregion
	#region stream
	/// <inheritdoc/>
	public static Level FromStream(Stream adlevelStream, LevelReadConfig? settings = null)
			=> FromStreamAsync(adlevelStream, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromStreamAsync(Stream stream, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadConfig();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
		Level? level;
		level = await FileMainEntryConverter.DeserializeMainEntryAsync<Level>(AdofaiUpgrader.Wrap(new StreamDataSource(stream), options), options, cancellationToken);
		return level ?? [];
	}
	/// <inheritdoc/>
	public void SaveToStream(Stream stream, LevelWriteConfig? settings = null)
			=> SaveToStreamAsync(stream, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToStreamAsync(Stream stream, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteConfig();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
		await Task.Run(() => FileMainEntryConverter.SerializeMainEntry(this, stream, options), cancellationToken);
	}
	#endregion
	#region json
	/// <inheritdoc/>
	public static Level FromJsonString(string json, LevelReadConfig? settings = null)
	{
		settings ??= new LevelReadConfig();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
		Level? level;
		level = FileMainEntryConverter.DeserializeMainEntry<Level>(AdofaiUpgrader.Wrap(new ReadOnlyMemoryDataSource(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(json))), options), options);
		return level ?? [];
	}
	/// <inheritdoc/>
	public string ToJsonString(LevelWriteConfig? settings = null)
	{
		settings ??= new LevelWriteConfig();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
		string json;
		using (MemoryStream stream = new())
		{
			FileMainEntryConverter.SerializeMainEntry(this, stream, options);
			stream.Seek(0, SeekOrigin.Begin);
			json = Encoding.UTF8.GetString(stream.ToArray());
		}
		return json;
	}
	/// <inheritdoc/>
	public static Level FromJsonDocument(JsonDocument jsonDocument, LevelReadConfig? settings = null)
	{
		settings ??= new LevelReadConfig();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
		Level? level;
		level = FileMainEntryConverter.DeserializeMainEntry<Level>(AdofaiUpgrader.Wrap(new JsonDocumentDataSource(jsonDocument), options), options);
		return level ?? [];
	}
	/// <inheritdoc/>
	public JsonDocument ToJsonDocument(LevelWriteConfig? settings = null)
	{
		settings ??= new LevelWriteConfig();
		string json;
		MemoryStream stream = new();
		SaveToStream(stream, settings);
		stream.Seek(0, SeekOrigin.Begin);
		json = Encoding.UTF8.GetString(stream.ToArray());
		return JsonDocument.Parse(json);
	}
	#endregion
}