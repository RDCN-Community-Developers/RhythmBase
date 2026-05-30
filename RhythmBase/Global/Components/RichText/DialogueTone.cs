using System.Diagnostics.CodeAnalysis;

namespace RhythmBase.Global.Components.RichText;

/// <summary>
/// Enum representing the different types of rich string events.
/// </summary>
public enum DialogueToneType
{
	/// <summary>
	/// Static event type.
	/// </summary>
	Static,
	/// <summary>
	/// Flash event type.
	/// </summary>
	Flash,
	/// <summary>
	/// Very slow event type.
	/// </summary>
	VerySlow,
	/// <summary>
	/// Slow event type.
	/// </summary>
	Slow,
	/// <summary>
	/// Normal event type.
	/// </summary>
	Normal,
	/// <summary>
	/// Fast event type.
	/// </summary>
	Fast,
	/// <summary>
	/// Very fast event type.
	/// </summary>
	VeryFast,
	/// <summary>
	/// Very very fast event type.
	/// </summary>
	VeryVeryFast,
	/// <summary>
	/// Excited event type.
	/// </summary>
	Excited,
	/// <summary>
	/// Shout event type.
	/// </summary>
	Shout,
	/// <summary>
	/// Shake event type.
	/// </summary>
	Shake,
	/// <summary>
	/// Pause event type.
	/// </summary>
	Pause,
}
/// <summary>
/// Class representing a rich string event.
/// </summary>
/// <param name="Type">Rich string event type.</param>
/// <param name="Index">Rich string event index.</param>
public record struct DialogueTone(DialogueToneType Type, int Index)
{
	/// <summary>
	/// Gets the pause duration for the dialogue event.
	/// </summary>
	public int? Pause { get; init; }
	/// <summary>
	/// Serializes the rich string event type to its corresponding string representation.
	/// </summary>
	/// <returns>A string representation of the rich string event type.</returns>
	/// <exception cref="NotImplementedException">Thrown when the event type is not implemented.</exception>
	public readonly string Serialize() => "[" + Type switch
	{
		DialogueToneType.Static => "static",
		DialogueToneType.Flash => "flash",
		DialogueToneType.VerySlow => "vslow",
		DialogueToneType.Slow => "slow",
		DialogueToneType.Normal => "normal",
		DialogueToneType.Fast => "fast",
		DialogueToneType.VeryFast => "vfast",
		DialogueToneType.VeryVeryFast => "vvvfast",
		DialogueToneType.Excited => "excited",
		DialogueToneType.Shout => "shout",
		DialogueToneType.Shake => "shake",
		DialogueToneType.Pause => Pause?.ToString(),
		_ => throw new NotImplementedException(),
	} + "]";
	/// <summary>
	/// Creates a new instance of <see cref="DialogueTone"/> based on the provided type and index.
	/// </summary>
	/// <param name="type">The string representation of the event type.</param>
	/// <param name="index">The index of the event.</param>
	/// <param name="result">The created <see cref="DialogueTone"/> instance if successful, otherwise null.</param>
	/// <returns>True if the event was successfully created, otherwise false.</returns>
	public static bool Create(string type, int index, [NotNullWhen(true)] out DialogueTone? result)
	{
		DialogueToneType? eventType = type switch
		{
			"static" => DialogueToneType.Static,
			"flash" => DialogueToneType.Flash,
			"vslow" => DialogueToneType.VerySlow,
			"slow" => DialogueToneType.Slow,
			"normal" => DialogueToneType.Normal,
			"fast" => DialogueToneType.Fast,
			"vfast" => DialogueToneType.VeryFast,
			"vvvfast" => DialogueToneType.VeryVeryFast,
			"excited" => DialogueToneType.Excited,
			"shout" => DialogueToneType.Shout,
			"shake" => DialogueToneType.Shake,
			_ => null,
		};
		if (eventType is null)
		{
			if (int.TryParse(type, out int pause))
			{
				result = new(DialogueToneType.Pause, index) { Pause = pause };
				return true;
			}
			result = null;
			return false;
		}
		result = new(eventType.Value, index);
		return true;
	}
}