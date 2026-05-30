using System.Diagnostics.CodeAnalysis;

namespace RhythmBase.Global.Components;

/// <summary>
/// Represents a reference to a file by its path and provides utility members for file-related queries.
/// </summary>
/// <remarks>
/// The <see cref="FileReference"/> struct encapsulates a file path and offers utility members for working
/// with file references, such as checking existence, retrieving the file extension, and determining if the
/// reference is empty. Implicit conversions to and from <see cref="string"/> are provided for convenience.
/// </remarks>
public struct FileReference : IEqualityComparer<FileReference>, IEquatable<FileReference>
{
	/// <summary>
	/// Gets or sets the file path for this reference.
	/// </summary>
	/// <remarks>
	/// The path may be absolute or relative. Relative paths are interpreted relative to a provided base file
	/// when calling methods such as <see cref="IsExist(string)"/>.
	/// </remarks>
	public string Path { get; set; }

    /// <summary>
    /// Gets the file extension of the referenced path, including the leading period ('.').
    /// </summary>
    /// <value>
    /// The extension of <see cref="Path"/> as returned by <see cref="Path.GetExtension(string)"/>.
    /// If <see cref="Path"/> is null or has no extension, an empty string is returned.
    /// </value>
    public readonly string Extension => System.IO.Path.GetExtension(Path);
	/// <summary>
	/// Gets a static instance of the FileReference class that represents an empty file reference.
	/// </summary>
	/// <remarks>This property can be used to represent a non-existent or uninitialized file reference, providing
	/// a consistent way to handle cases where a file reference is required but no valid path is available.</remarks>
	public static FileReference Empty => new() { Path = "" };
	/// <summary>
	/// Determines whether the file referenced by <see cref="Path"/> exists.
	/// </summary>
	/// <param name="parentFile">
	/// A base file path used to resolve relative <see cref="Path"/> values. If <see cref="Path"/> is absolute,
	/// <paramref name="parentFile"/> is ignored.
	/// </param>
	/// <returns>
	/// <c>true</c> if the resolved file path exists on disk; otherwise, <c>false</c>.
	/// </returns>
	public readonly bool IsExist(string parentFile) => File.Exists(System.IO.Path.GetFullPath(Path, parentFile));

	/// <summary>
	/// Gets a value indicating whether this file reference has no path.
	/// </summary>
	/// <value>
	/// <c>true</c> if <see cref="Path"/> is null or an empty string; otherwise, <c>false</c>.
	/// </value>
	public readonly bool IsEmpty => string.IsNullOrEmpty(Path);

	/// <summary>
	/// Implicitly converts a <see cref="FileReference"/> to a <see cref="string"/> by returning its <see cref="Path"/>.
	/// </summary>
	/// <param name="fileReference">The file reference to convert.</param>
	/// <returns>The <see cref="Path"/> value of <paramref name="fileReference"/>.</returns>
	public static implicit operator string(FileReference fileReference) => fileReference.Path;

	/// <summary>
	/// Implicitly converts a <see cref="string"/> path to a <see cref="FileReference"/>.
	/// </summary>
	/// <param name="path">The path to wrap as a <see cref="FileReference"/>.</param>
	/// <returns>A new <see cref="FileReference"/> instance with <see cref="Path"/> set to <paramref name="path"/>.</returns>
	public static implicit operator FileReference(string path) => new() { Path = path };
	/// <summary>
	/// Determines whether two specified FileReference instances have the same file path.
	/// </summary>
	/// <remarks>The comparison is case-sensitive and compares the Path properties of the two FileReference
	/// instances. Both operands can be null; two null references are considered equal.</remarks>
	/// <param name="left">The first FileReference to compare.</param>
	/// <param name="right">The second FileReference to compare.</param>
	/// <returns>true if the file paths of left and right are equal; otherwise, false.</returns>
	public static bool operator ==(FileReference left, FileReference right) => left.Path == right.Path;
	/// <summary>
	/// Determines whether two specified FileReference instances have different file paths.
	/// </summary>
	/// <remarks>This operator compares the Path properties of the two FileReference instances to determine
	/// inequality.</remarks>
	/// <param name="left">The first FileReference to compare.</param>
	/// <param name="right">The second FileReference to compare.</param>
	/// <returns>true if the file paths of left and right are not equal; otherwise, false.</returns>
	public static bool operator !=(FileReference left, FileReference right) => left.Path != right.Path;
	///<inheritdoc/>
	public readonly override bool Equals(object? obj) => obj is FileReference other && this == other;
	///<inheritdoc/>
	public readonly override int GetHashCode() => Path.GetHashCode();
	///<inheritdoc/>
	public readonly override string ToString() => Path;
	///<inheritdoc/>
	public readonly bool Equals(FileReference other) => Path == other.Path;
	///<inheritdoc/>
	public readonly bool Equals(FileReference x, FileReference y) => x.Path == y.Path;
	///<inheritdoc/>
	public readonly int GetHashCode([NotNull] FileReference obj) => obj.Path.GetHashCode();
}
