using RhythmBase.Global.Serialization;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Serialization;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Components;

partial class Level
{
	#region directory
	/// <summary>
	/// Loads a level from a directory containing <c>.rdlevel</c> chart files. The <c>main.rdlevel</c>
	/// file is loaded as the main chart and all other <c>.rdlevel</c> files are loaded as additional charts.
	/// </summary>
	public static Level FromDirectory(string directoryPath, LevelReadConfig? settings = null)
		=> FromDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
	/// <summary>
	/// Asynchronously loads a level from a directory containing <c>.rdlevel</c> chart files.
	/// </summary>
	public static async Task<Level> FromDirectoryAsync(string directoryPath, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadConfig();
		string mainPath = Path.Combine(directoryPath, ChartNaming.Instance.GetFileName(DefaultChartName));
		if (!File.Exists(mainPath))
			throw new FileNotFoundException($"The main chart '{mainPath}' was not found.");
		Chart main = await Chart.FromFileAsync(mainPath, settings, cancellationToken);
		Dictionary<string, Chart> extras = [];
		foreach (string file in Directory.GetFiles(directoryPath, "*.rdlevel"))
		{
			if (Path.GetFileName(file) == ChartNaming.Instance.GetFileName(DefaultChartName))
				continue;
			Chart extra = await Chart.FromFileAsync(file, settings, cancellationToken);
			if (ChartNaming.Instance.TryGetChartName(Path.GetFileName(file), out string chartName))
				extras[chartName] = extra;
		}
		Level level = new(main, extras)
		{
			Filepath = main.Filepath,
			ResolvedPath = main.ResolvedPath,
		};
		if (settings.LoadReferencedCharts)
			LoadReferencedCharts(level, settings);
		return level;
	}
	/// <summary>
	/// Saves all charts contained in this level to the specified directory.
	/// </summary>
	public void SaveToDirectory(string directoryPath, LevelWriteConfig? settings = null)
		=> SaveToDirectoryAsync(directoryPath, settings).GetAwaiter().GetResult();
	/// <summary>
	/// Asynchronously saves all charts contained in this level to the specified directory.
	/// </summary>
	public async Task SaveToDirectoryAsync(string directoryPath, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteConfig();
		Directory.CreateDirectory(directoryPath);
		foreach (var pair in _charts)
			await pair.Value.SaveToFileAsync(Path.Combine(directoryPath, ChartNaming.Instance.GetFileName(pair.Value.Name)), settings, cancellationToken);
	}
	#endregion
	#region zip
	/// <summary>
	/// Loads a level from a ZIP archive containing the main chart and its assets.
	/// </summary>
	public static Level FromZip(string filepath, LevelReadConfig? settings = null) => FromZipAsync(filepath, settings).GetAwaiter().GetResult();
	/// <summary>
	/// Asynchronously loads a level from a ZIP archive containing the main chart and its assets.
	/// </summary>
	public static async Task<Level> FromZipAsync(string filepath, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadConfig();
		string extension = Path.GetExtension(filepath);
		Chart main;
		Dictionary<string, Chart> extras = [];
		if (extension is not ".rdzip" and not ".zip")
		{
			if (extension is ".rdlevel" or ".json")
				throw new NotSupportedException($"File type '{extension}' is not supported. Use {nameof(Chart)}{nameof(Chart.FromFileAsync)} instead.");
			throw new NotSupportedException($"File type '{extension}' is not supported.");
		}
		switch (settings.ZipProcessingMode)
		{
			case ZipProcessingMode.AllEntries:
				DirectoryInfo tempDirectory = new(Path.Combine(Global.Config.CachePath, "RhythmBaseTemp_Zip_" + Path.GetRandomFileName()));
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
						if (file.Name == ChartNaming.Instance.GetFileName(DefaultChartName))
						{
							rdlevelPath = file.FullName;
							break;
						}
					}
					if (rdlevelPath == null)
						throw new FileNotFoundException("No RDLevel file has been found.");
					main = await Chart.FromFileAsync(rdlevelPath, settings, cancellationToken);
					main.ResolvedPath = Path.GetFullPath(rdlevelPath);
					main.Filepath = Path.GetFullPath(filepath);
					main.isZip = true;
					main.isExtracted = true;
					foreach (FileInfo file in tempDirectory.GetFiles("*.rdlevel"))
					{
						if (file.Name == ChartNaming.Instance.GetFileName(DefaultChartName))
							continue;
						Chart extra = await Chart.FromFileAsync(file.FullName, settings, cancellationToken);
						if (ChartNaming.Instance.TryGetChartName(file.Name, out string chartName))
							extras[chartName] = extra;
					}
				}
				catch (Exception)
				{
					tempDirectory.Delete(true);
					throw;
				}
				break;
			case ZipProcessingMode.RootEntriesOnly:
				using (FileStream zipStream = new(filepath, FileMode.Open, FileAccess.Read))
				{
					using ZipArchive archive = new(zipStream, ZipArchiveMode.Read);
					ZipArchiveEntry entry = archive.GetEntry(ChartNaming.Instance.GetFileName(DefaultChartName)) ?? throw new FileNotFoundException("Cannot find the level file.");
					using Stream entryStream = entry.Open();
					main = await Chart.FromStreamAsync(entryStream, settings, cancellationToken);
				}
				main.Filepath = Path.GetFullPath(filepath);
				main.isZip = true;
				main.isExtracted = false;
				break;
			default:
				throw new NotSupportedException(extension + " is not supported.");
		}
		Level level = new(main, extras)
		{
			Filepath = main.Filepath,
			ResolvedPath = main.ResolvedPath,
		};
		if (settings.LoadReferencedCharts)
			LoadReferencedCharts(level, settings);
		return level;
	}
	/// <summary>
	/// Asynchronously loads a level from a ZIP stream containing the main chart.
	/// </summary>
	public static Task<Level> FromZip(Stream zipStream, LevelReadConfig? settings = null)
		=> FromZipAsync(zipStream, settings);
	/// <summary>
	/// Asynchronously loads a level from a ZIP stream containing the main chart.
	/// </summary>
	public static async Task<Level> FromZipAsync(Stream zipStream, LevelReadConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelReadConfig();
		Chart main;
		using (ZipArchive archive = new(zipStream, ZipArchiveMode.Read))
		{
			ZipArchiveEntry entry = archive.GetEntry(ChartNaming.Instance.GetFileName(DefaultChartName)) ?? throw new FileNotFoundException("Cannot find the level file.");
			using Stream stream = entry.Open();
			main = await Chart.FromStreamAsync(stream, settings, cancellationToken);
		}
		main.isZip = true;
		main.isExtracted = false;
		return new Level(main);
	}
	/// <summary>
	/// Saves all charts contained in this level to a ZIP archive at the specified path. Referenced
	/// charts' assets are additionally packed when <see cref="LevelWriteConfig.PackReferencedCharts"/> is enabled.
	/// </summary>
	public void SaveToZip(string filepath, LevelWriteConfig? settings = null)
		=> SaveToZipAsync(filepath, settings).GetAwaiter().GetResult();
	/// <summary>
	/// Asynchronously saves the level to a ZIP archive at the specified path.
	/// </summary>
	public async Task SaveToZipAsync(string filepath, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteConfig();
		DirectoryInfo directory = new FileInfo(filepath).Directory ?? new("");
		if (!directory.Exists)
			directory.Create();
		using FileStream stream = new(filepath, FileMode.Create, FileAccess.Write);
		await SaveToZipAsync(stream, settings, cancellationToken);
	}
	/// <summary>
	/// Saves the level to the specified ZIP stream.
	/// </summary>
	public void SaveToZip(Stream zipStream, LevelWriteConfig? settings = null)
		=> SaveToZipAsync(zipStream, settings).GetAwaiter().GetResult();
	/// <summary>
	/// Asynchronously saves the level to the specified ZIP stream. All charts contained in the level are
	/// written as entries. Referenced charts' assets are additionally packed only when
	/// <see cref="LevelWriteConfig.PackReferencedCharts"/> is enabled.
	/// </summary>
	public async Task SaveToZipAsync(Stream zipStream, LevelWriteConfig? settings = null, CancellationToken cancellationToken = default)
	{
		settings ??= new LevelWriteConfig();
		using ZipArchive archive = new(zipStream, ZipArchiveMode.Create, leaveOpen: true);
		foreach (var pair in _charts)
		{
			string name = ChartNaming.Instance.GetFileName(pair.Value.Name);
			Chart chart = pair.Value;
			bool packAssets = chart == MainChart || settings.PackReferencedCharts;
			string directory = ResolveChartDirectory(chart, settings);
			HashSet<FileReference> fileReferences = [];
			void referenceDelegate(object? sender, FileReferenceArgs args) => fileReferences.Add(args.Reference);
			bool loadAssets = settings.LoadAssets;
			if (packAssets)
			{
				settings.FileReferenceEncountered += referenceDelegate;
				settings.LoadAssets = true;
			}
			ZipArchiveEntry entry = archive.CreateEntry(name);
			using (Stream chartStream = entry.Open())
			{
				MetadataJsonSerializerOptions options = JsonSerializerOptionsUtils.GetJsonSerializerOptionsForWrite(settings);
				options.DirectoryName = directory;
				await Task.Run(() => FileMainEntryConverter.SerializeMainEntry(chart, chartStream, options), cancellationToken);
			}
			if (packAssets)
			{
				settings.FileReferenceEncountered -= referenceDelegate;
				settings.LoadAssets = loadAssets;
				foreach (var file in fileReferences)
				{
					if (string.IsNullOrEmpty(directory))
						continue;
					string fullPath = Path.Combine(directory, file.Path);
					if (!File.Exists(fullPath))
						throw new FileNotFoundException($"Referenced file '{file.Path}' not found in the resolved directory.");
					archive.CreateEntryFromFile(fullPath, Path.GetFileName(file.Path));
				}
			}
		}
	}
	#endregion
	/// <summary>
	/// Resolves the directory used to locate a chart's referenced assets, giving precedence to
	/// <paramref name="settings"/>.<see cref="LevelWriteConfig.ResolvedDirectory"/> over the chart's
	/// own <see cref="Chart.ResolvedDirectory"/>.
	/// </summary>
	private static string ResolveChartDirectory(Chart chart, LevelWriteConfig settings) =>
		string.IsNullOrWhiteSpace(settings.ResolvedDirectory)
		? chart.ResolvedDirectory
		: settings.ResolvedDirectory;
	/// <summary>
	/// Recursively resolves charts referenced by <see cref="GoToLevel"/> events into the level's chart
	/// collection. Each referenced file is loaded only once and shared by all <see cref="GoToLevel"/>
	/// events, which makes cyclic references between charts safe.
	/// </summary>
	private static void LoadReferencedCharts(Level level, LevelReadConfig settings)
	{
		Queue<Chart> pending = new();
		pending.Enqueue(level.MainChart);
		while (pending.Count > 0)
		{
			Chart chart = pending.Dequeue();
			foreach (GoToLevel goTo in chart.OfType<GoToLevel>())
			{
				if (goTo.Chart.IsEmpty)
					continue;
				if (!ChartNaming.Instance.TryGetChartName(Path.GetFileName(goTo.Chart.Path), out string key))
					continue;
				if (string.IsNullOrEmpty(key))
					continue;
				if (level.TryGetChart(key, out Chart? existing))
				{
					goTo.ResolvedLevel = existing;
					continue;
				}
				string baseDir = string.IsNullOrEmpty(chart.ResolvedDirectory) ? Directory.GetCurrentDirectory() : chart.ResolvedDirectory;
				string fullPath = Path.GetFullPath(goTo.Chart.Path, baseDir);
				if (!File.Exists(fullPath))
					continue;
				Chart refChart = Chart.FromFile(fullPath, settings);
				level.RegisterChart(key, refChart);
				goTo.ResolvedLevel = refChart;
				pending.Enqueue(refChart);
			}
		}
	}
}
