namespace RhythmBase.Global.Settings;

/// <summary>
/// Provides global configuration settings for the application.
/// </summary>
/// <remarks>
/// This static class contains application-wide configuration values and defaults.
/// Settings declared here are intended to be globally accessible and stable across the runtime.
/// </remarks>
public static class GlobalSettings
{
	/// <summary>
	/// Gets or sets the prefix used for naming temporary cache directories created by the application.
	/// </summary>
	/// <remarks>Customize this value to avoid naming conflicts with other applications that may use similar
	/// directory naming conventions.</remarks>
	public static string CacheDirectoryPrefix
	{
		get; set
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("CacheDirectoryPrefix cannot be null, empty, or whitespace.", nameof(value));
			if (Path.GetInvalidFileNameChars().Any(value.Contains))
				throw new ArgumentException("CacheDirectoryPrefix cannot contain invalid path characters.", nameof(value));
			field = value;
		}
	} = "RhythmBaseTemp_Zip_";
	/// <summary>
	/// Gets or sets the path to the directory used for caching temporary files.
	/// </summary>
	/// <remarks>
	/// Defaults to the operating system's temporary directory as returned by <see cref="Path.GetTempPath"/>.
	/// Consumers may override this path to direct cache files to a specific location; ensure the
	/// configured directory exists and the application has appropriate read/write permissions.
	/// </remarks>
	/// <value>
	/// A string representing an absolute or relative filesystem path used for temporary cache files.
	/// </value>
	public static string CachePath { get; set; } = Path.GetTempPath();
	internal static DirectoryInfo GetTempDirectory() => new(Path.Combine(CachePath, CacheDirectoryPrefix + Guid.NewGuid().ToString("N")));
	/// <summary>
	/// Deletes all cached directories that match the specified prefix within the cache path.
	/// </summary>
	/// <remarks>If an error occurs while deleting any directory, the method ignores the error and continues
	/// processing. This method is typically used to clear all cached data managed by the application.</remarks>
	public static void ClearCache()
	{
		try
		{
			foreach (string dir in Directory.GetDirectories(CachePath, CacheDirectoryPrefix))
				Directory.Delete(dir, true);
		}
		catch { }
	}
}
