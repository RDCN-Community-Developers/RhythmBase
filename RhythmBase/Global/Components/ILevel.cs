using System.Text.Json;

namespace RhythmBase.Global.Components
{
    internal interface ILevel<TSelf, TEvent, TType, TBeat> : IDisposable
        where TSelf : ILevel<TSelf, TEvent, TType, TBeat>
        where TEvent : IEvent<TType, TBeat>
        where TType : struct, Enum
        where TBeat : struct, IBeat<TBeat>
    {
        public int Count { get; }
        /// <summary>
        /// The resolved file path for reading. Points to the extracted temporary file if the source is an archive; otherwise identical to <see cref="Filepath"/>.
        /// </summary>
        string ResolvedPath { get; }
        /// <summary>
        /// The original file path as provided (e.g., archive path or direct file path).
        /// </summary>
        string? Filepath { get; }
        /// <summary>
        /// The directory containing the resolved file. Points to the temporary extraction directory if the source is an archive; otherwise the directory of <see cref="Filepath"/>.
        /// </summary>
        string? ResolvedDirectory { get; }
        IBeatCalculator<TBeat> Calculator { get; }
#if NET8_0_OR_GREATER
        /// <summary>
        /// The default level within the game.
        /// </summary>
        static abstract TSelf Default { get; }
        /// <summary>
        /// Creates an <typeparamref name="TSelf"/> instance by reading data from the specified file.
        /// </summary>
        /// <remarks>This method supports both plain level files and compressed
        /// archives. If the file is a compressed archive, it is extracted to a temporary
        /// directory to locate the level file within the archive.</remarks>
        /// <param name="filepath">The path to the file to read.</param>
        /// <param name="settings">Optional settings that control how the level is read. If not provided, default settings are used.</param>
        /// <returns>An <typeparamref name="TSelf"/> instance representing the data read from the file.</returns>
        /// <exception cref="RhythmBaseException">Thrown if the file format is not supported, if no level file is found in the archive, or if an error occurs during
        /// file extraction.</exception>
        static abstract TSelf FromFile(string filepath, ILevelReadSettings<TEvent, TType, TBeat>? settings = null);
        /// <summary>
        /// Asynchronously loads an <typeparamref name="TSelf"/> instance from a file.
        /// </summary>
        /// <remarks>If the file is a compressed archive, the method extracts its
        /// contents to a temporary directory and searches for a file with the extension. If no such file is
        /// found, an exception is thrown. The temporary directory is automatically cleaned up after the operation
        /// completes.</remarks>
        /// <param name="filepath">The path to the file to load.</param>
        /// <param name="settings">Optional settings that control how the level is read. If <see langword="null"/>, default settings are used.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <typeparamref name="TSelf"/> instance. 
        /// If the file contains no data or deserialization results in null, the task result will be an empty <typeparamref name="TSelf"/>
        /// instance.</returns>
        /// <exception cref="RhythmBaseException">Thrown if the file format is unsupported, if no file is found in a compressed archive, or if an
        /// error occurs during extraction.</exception>
        static abstract Task<TSelf> FromFileAsync(string filepath, ILevelReadSettings<TEvent, TType, TBeat>? settings = null, CancellationToken cancellationToken = default);

#endif
        /// <summary>
        /// Saves the current level to a file in JSON format.
        /// </summary>
        /// <param name="filepath">The file path where the level will be saved.</param>
        /// <param name="settings">Optional settings for writing the level. If null, default settings are used.</param>
        void SaveToFile(string filepath, ILevelWriteSettings<TEvent, TType, TBeat>? settings = null);
        /// <summary>
        /// Asynchronously saves the current level to a file in JSON format.
        /// </summary>
        /// <param name="filepath">The file path where the level will be saved.</param>
        /// <param name="settings">Optional settings for writing the level. If null, default settings are used.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        void SaveToFileAsync(string filepath, ILevelWriteSettings<TEvent, TType, TBeat>? settings = null, CancellationToken cancellationToken = default);
    }
    internal interface ISingleFileLevel<TSelf, TEvent, TType, TBeat> : ILevel<TSelf, TEvent, TType, TBeat>
        where TSelf : ILevel<TSelf, TEvent, TType, TBeat>
        where TEvent : IEvent<TType, TBeat>
        where TType : struct, Enum
        where TBeat : struct, IBeat<TBeat>
    {
#if NET8_0_OR_GREATER
        /// <summary>
        /// Deserializes an <typeparamref name="TSelf"/> object from the specified stream using the provided settings.
        /// </summary>
        /// <remarks>The method invokes the <see cref="ILevelReadSettings{TEvent, TType, TBeat}.OnBeforeReading"/> and <see cref="ILevelReadSettings{TEvent, TType, TBeat}.OnAfterReading"/> callbacks on the provided settings,
        /// allowing for custom pre- and post-processing during deserialization.</remarks>
        /// <param name="stream">The stream containing the serialized <typeparamref name="TSelf"/> data. The stream must be readable and positioned at the start of
        /// the data.</param>
        /// <param name="settings">Optional settings that control the deserialization process. If not specified, default settings are used.</param>
        /// <returns>An <typeparamref name="TSelf"/> object representing the deserialized data. Returns an empty <typeparamref name="TSelf"/> instance if the stream contains
        /// no data or deserialization results in null.</returns>
        static abstract TSelf FromStream(Stream stream, ILevelReadSettings<TEvent, TType, TBeat>? settings = null);
        /// <summary>
        /// Asynchronously reads a level from a stream.
        /// </summary>
        /// <param name="stream">The stream containing the level data.</param>
        /// <param name="settings">Optional settings for reading the level.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task{TLevel}"/> representing the asynchronous operation, with an <typeparamref name="TSelf"/> instance loaded from the stream.</returns>
        static abstract Task<TSelf> FromStreamAsync(Stream stream, ILevelReadSettings<TEvent, TType, TBeat>? settings = null, CancellationToken cancellationToken = default);
#endif
        /// <summary>
        /// Saves the current level to the specified stream in JSON format.
        /// </summary>
        /// <param name="stream">The stream to which the level will be saved.</param>
        /// <param name="settings">Optional settings for writing the level. If null, default settings are used.</param>
        void SaveToStream(Stream stream, ILevelWriteSettings<TEvent, TType, TBeat>? settings = null);
        /// <summary>
        /// Asynchronously saves the current level to the specified stream in JSON format.
        /// </summary>
        /// <param name="stream">The stream to which the level will be saved.</param>
        /// <param name="settings">Optional settings for writing the level. If null, default settings are used.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        void SaveToStreamAsync(Stream stream, ILevelWriteSettings<TEvent, TType, TBeat>? settings = null, CancellationToken cancellationToken = default);
    }
    internal interface IArchiveLevel<TSelf, TEvent, TType, TBeat> : ILevel<TSelf, TEvent, TType, TBeat>
        where TSelf : ILevel<TSelf, TEvent, TType, TBeat>
        where TEvent : IEvent<TType, TBeat>
        where TType : struct, Enum
        where TBeat : struct, IBeat<TBeat>
    {
        /// <summary>
        /// Saves the current level data to a file in packed (ZIP) format at the specified path. (This method is not fully implemented yet.)
        /// </summary>
        /// <param name="filepath">The path of the file to create and write the packed level data to. Must not be null or empty.</param>
        /// <param name="settings">Optional settings that control how the level data is written. If null, default settings are used.</param>
        void SaveToZip(string filepath, ILevelWriteSettings<TEvent, TType, TBeat>? settings = null);
        /// <summary>
        /// Asynchronously saves the current level and its associated assets to a ZIP archive at the specified file path.
        /// </summary>
        /// <remarks>The resulting ZIP archive will contain the main level data as a JSON file and any referenced
        /// asset files. This method is asynchronous but returns void; exceptions will be thrown on the calling thread if the
        /// operation fails.</remarks>
        /// <param name="filepath">The full path to the ZIP file to create. If the file exists, it will be overwritten.</param>
        /// <param name="settings">Optional settings that control how the level and its assets are serialized. If null, default settings are used.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the save operation.</param>
        /// <exception cref="NotImplementedException">Thrown if the level's directory is not set.</exception>
        void SaveToZipAsync(string filepath, ILevelWriteSettings<TEvent, TType, TBeat>? settings = null, CancellationToken cancellationToken = default);
    }
    internal interface IJsonLevel<TSelf, TEvent, TType, TBeat> : ILevel<TSelf, TEvent, TType, TBeat>
        where TSelf : ILevel<TSelf, TEvent, TType, TBeat>
        where TEvent : IEvent<TType, TBeat>
        where TType : struct, Enum
        where TBeat : struct, IBeat<TBeat>
    {
#if NET8_0_OR_GREATER
        /// <summary>
        /// Deserializes a JSON document into an instance of the RDLevel class using the specified settings.
        /// </summary>
        /// <remarks>This method invokes the OnBeforeReading and OnAfterReading callbacks from the provided settings to
        /// allow for custom pre- and post-processing during deserialization.</remarks>
        /// <param name="jsonDocument">The JSON document to deserialize. This parameter cannot be null.</param>
        /// <param name="settings">Optional settings that control the deserialization process. If not specified, default settings are used.</param>
        /// <returns>An instance of RDLevel representing the deserialized data. Returns an empty array if deserialization fails.</returns>
        static abstract TSelf FromJsonDocument(JsonDocument jsonDocument, ILevelReadSettings<TEvent, TType, TBeat>? settings = null);
        /// <summary>
        /// Reads a level from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string containing the level data.</param>
        /// <param name="settings">Optional settings for reading the level.</param>
        /// <returns>An <typeparamref name="TSelf"/> instance loaded from the JSON string.</returns>
        static abstract TSelf FromJsonString(string json, ILevelReadSettings<TEvent, TType, TBeat>? settings = null);
#endif
        /// <summary>
        /// Converts the current level instance into a <see cref="JsonDocument"/>.
        /// </summary>
        /// <param name="settings">Optional settings to control the JSON serialization process. If <see langword="null"/>, default settings are used.</param>
        /// <returns>A <see cref="JsonDocument"/> representing the serialized level data.</returns>
        JsonDocument ToJsonDocument(ILevelWriteSettings<TEvent, TType, TBeat>? settings = null);

        /// <summary>
        /// Serializes the current level instance into a JSON formatted string.
        /// </summary>
        /// <param name="settings">Optional settings to control the JSON serialization process. If <see langword="null"/>, default settings are used.</param>
        /// <returns>A string containing the JSON representation of the level.</returns>
        string ToJsonString(ILevelWriteSettings<TEvent, TType, TBeat>? settings = null);
    }
}