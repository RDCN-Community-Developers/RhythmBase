using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

/// <summary>
/// Options that carry additional metadata alongside standard <see cref="JsonSerializerOptions"/> for level serialization.
/// </summary>
public record class MetadataJsonSerializerOptions
{
	/// <summary>
	/// Gets or sets the underlying <see cref="JsonSerializerOptions"/> used for JSON serialization.
	/// </summary>
	public required JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		WriteIndented = true,
	};
	/// <summary>
	/// Gets a value indicating whether array elements should be aligned vertically when writing.
	/// </summary>
	public bool WriteAligned { get; init; }
	/// <summary>
	/// Gets or sets a value indicating whether the JSON output should be indented.
	/// </summary>
	public bool WriteIndented { get => JsonSerializerOptions.WriteIndented; init => JsonSerializerOptions.WriteIndented = value; }
	public JsonStrictness Strictness { get; init; } = JsonStrictness.Strict;
}
public enum JsonStrictness
{
	Strict,
	Relaxed,
}