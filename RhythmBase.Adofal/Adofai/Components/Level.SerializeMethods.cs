using System.IO.Compression;
using System.Text;
using System.Text.Json;
using RhythmBase.Adofai.Converters;

namespace RhythmBase.Adofai.Components;

partial class Level
{
	public static LevelType LevelType => LevelType.Adofai;
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
	public async Task SaveToZipAsync(string filepath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrEmpty(this.ResolvedDirectory))
			throw new NotImplementedException();
		settings ??= new LevelWriteSettings();
		HashSet<FileReference> files = new HashSet<FileReference>();
		void AddFileReference(object? sender, FileReferenceArgs args) => files.Add(args.Reference);
		settings.FileReferenceEncountered += AddFileReference;
		bool loadAssets = settings.LoadAssets;
		settings.LoadAssets = true;
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(Path.GetDirectoryName(Filepath) ?? "", settings);
		DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
		if (!directory.Exists)
			directory.Create();
		using Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
		ZipArchive archive = new(stream, ZipArchiveMode.Create);
		ZipArchiveEntry entry = archive.CreateEntry("main.adofai");
		using (Stream rdlevelStream = entry.Open())
			await SaveToStreamAsync(rdlevelStream, settings, cancellationToken);
		foreach (var file in files)
			archive.CreateEntryFromFile(Path.Combine(ResolvedDirectory, file.Path), Path.GetFileName(file.Path));
		archive.Dispose();
		settings.FileReferenceEncountered -= AddFileReference;
		settings.LoadAssets = loadAssets;
	}
	#endregion
	#region file
	/// <inheritdoc/>
	public static Level FromFile(string filepath, LevelReadSettings? settings = null)
			=> FromFileAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromFileAsync(string filepath, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadSettings();
		string extension = Path.GetExtension(filepath);
		Level? level;
		if (extension != ".zip")
		{
			if (extension != ".adofai")
				throw new RhythmBaseException("File not supported.");
			using FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);
			level = FromStream(stream, settings);
			level.Filepath = level.ResolvedPath = Path.GetFullPath(filepath);
			return level;
		}
		switch (settings.ZipFileProcessMethod)
		{
			case ZipFileProcessMethod.AllFiles:
				DirectoryInfo tempDirectory = new(Path.Combine(GlobalSettings.CachePath, "RhythmBaseTemp_" + Path.GetRandomFileName()));
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
						throw new RhythmBaseException("No Adofai file has been found.");
					level = FromFile(adlevelPath, settings);
					level.ResolvedPath = Path.GetFullPath(adlevelPath);
					level.Filepath = Path.GetFullPath(filepath);
					level.isZip = true;
					level.isExtracted = true;
				}
				catch (Exception ex2)
				{
					tempDirectory.Delete(true);
					throw new RhythmBaseException("Cannot extract the file.", ex2);
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
	public void SaveToFile(string filepath, LevelWriteSettings? settings = null)
			=> SaveToFileAsync(filepath, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToFileAsync(string filepath, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteSettings();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(filepath, settings);
		using FileStream stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write);
		stream.SetLength(0);
		await Task.Run(() => FileMainEntryConverter.SerializeMainEntry(this, stream, options), cancellationToken);
	}
	#endregion
	#region stream
	/// <inheritdoc/>
	public static Level FromStream(Stream adlevelStream, LevelReadSettings? settings = null)
			=> FromStreamAsync(adlevelStream, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public static async Task<Level> FromStreamAsync(Stream stream, LevelReadSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadSettings();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
		Level? level;
		level = await FileMainEntryConverter.DeserializeMainEntryAsync<Level>(new StreamDataSource(stream), options, cancellationToken);
		return level ?? [];
	}
	/// <inheritdoc/>
	public void SaveToStream(Stream stream, LevelWriteSettings? settings = null)
			=> SaveToStreamAsync(stream, settings).GetAwaiter().GetResult();
	/// <inheritdoc/>
	public async Task SaveToStreamAsync(Stream stream, LevelWriteSettings? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteSettings();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
		await Task.Run(() => FileMainEntryConverter.SerializeMainEntry(this, stream, options), cancellationToken);
	}
	#endregion
	#region json
	/// <inheritdoc/>
	public static Level FromJsonString(string json, LevelReadSettings? settings = null)
	{
		settings ??= new LevelReadSettings();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
		Level? level;
		level = FileMainEntryConverter.DeserializeMainEntry<Level>(new ReadOnlyMemoryDataSource(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(json))), options);
		return level ?? [];
	}
	/// <inheritdoc/>
	public string ToJsonString(LevelWriteSettings? settings = null)
	{
		settings ??= new LevelWriteSettings();
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
	public static Level FromJsonDocument(JsonDocument jsonDocument, LevelReadSettings? settings = null)
	{
		settings ??= new LevelReadSettings();
		MetadataJsonSerializerOptions options = Utils.Utils.GetJsonSerializerOptions(settings: settings);
		Level? level;
		level = FileMainEntryConverter.DeserializeMainEntry<Level>(new JsonDocumentDataSource(jsonDocument), options);
		return level ?? [];
	}
	/// <inheritdoc/>
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