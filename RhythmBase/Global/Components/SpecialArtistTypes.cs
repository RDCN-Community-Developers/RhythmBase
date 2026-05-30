namespace RhythmBase.Global.Components;

/// <summary>
/// Enum representing special types of artists.
/// </summary>
[JsonEnumSerializable]
public enum SpecialArtistTypes
{
	/// <summary>
	/// No special artist type.
	/// </summary>
	None,
	/// <summary>
	/// The author is also the artist.
	/// </summary>
	AuthorIsArtist,
	/// <summary>
	/// The artist's work is under a public license.
	/// </summary>
	PublicLicense
}
