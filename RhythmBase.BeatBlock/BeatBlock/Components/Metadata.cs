using RhythmBase.BeatBlock.Converters;
using RhythmBase.BeatBlock.Events;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using static RhythmBase.BeatBlock.Constants;

namespace RhythmBase.BeatBlock.Components;
/// <summary>
/// Represents the metadata of a BeatBlock level.
/// </summary>
public record class Metadata
{
	/// <summary>
	/// Gets or sets the name of the song.
	/// </summary>
	public string SongName { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the artist of the song.
	/// </summary>
	public string Artist { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the artist link.
	/// </summary>
	public string ArtistLink { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the beats per minute of the song.
	/// </summary>
	public float Bpm { get; set; }
	/// <summary>
	/// Gets or sets the description of the level.
	/// </summary>
	public string Description { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the charter of the level.
	/// </summary>
	public string Charter { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the difficulty of the level.
	/// </summary>
	public int Difficulty { get; set; } = 0;
	/// <summary>
	/// Gets or sets a value indicating whether a light warning is enabled.
	/// </summary>
	public bool LightWarning { get; set; } = false;
	/// <summary>
	/// Gets or sets a value indicating whether loop points are enabled.
	/// </summary>
	public bool LoopPointsEnable { get; set; } = false;
	/// <summary>
	/// Gets or sets a value indicating whether a lyrics warning is enabled.
	/// </summary>
	public bool LyricsWarning { get; set; } = false;
	/// <summary>
	/// Gets or sets the source of the level.
	/// </summary>
	public string Source { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets a value indicating whether the background is enabled.
	/// </summary>
	public bool IsBackgroundEnabled { get; set; } = true;
	/// <summary>
	/// Gets or sets the start loop point.
	/// </summary>
	public float StartLoop { get; set; } = 0;
	/// <summary>
	/// Gets or sets the end loop point.
	/// </summary>
	public float EndLoop { get; set; } = 0;
	/// <summary>
	/// Gets or sets the background data.
	/// </summary>
	public BackgroundData BackgroundData { get; set; } = new BackgroundData();
	/// <summary>
	/// Initializes a new instance of the <see cref="Metadata"/> class.
	/// </summary>
	public Metadata()
	{

	}
}

/// <summary>
/// Represents the background data for a BeatBlock level.
/// </summary>
public record class BackgroundData
{
	/// <summary>
	/// Gets or sets the red channel color.
	/// </summary>
	public Color RedChannel { get; set; } = Color.Red;
	/// <summary>
	/// Gets or sets the green channel color.
	/// </summary>
	public Color GreenChannel { get; set; } = Color.Green;
	/// <summary>
	/// Gets or sets the blue channel color.
	/// </summary>
	public Color BlueChannel { get; set; } = Color.Blue;
	/// <summary>
	/// Gets or sets the yellow channel color.
	/// </summary>
	public Color YellowChannel { get; set; } = Color.Yellow;
	/// <summary>
	/// Gets or sets the magenta channel color.
	/// </summary>
	public Color MagentaChannel { get; set; } = Color.Magenta;
	/// <summary>
	/// Gets or sets the cyan channel color.
	/// </summary>
	public Color CyanChannel { get; set; } = Color.Cyan;
	/// <summary>
	/// Gets or sets a value indicating whether the Cranky effect is hidden.
	/// </summary>
	public bool HideCranky { get; set; } = false;
	/// <summary>
	/// Gets or sets the background image reference.
	/// </summary>
	public FileReference Image { get; set; } = new FileReference();
	/// <summary>
	/// Gets or sets the results image reference.
	/// </summary>
	public FileReference ResultsImage { get; set; } = new FileReference();
	/// <summary>
	/// Initializes a new instance of the <see cref="BackgroundData"/> class.
	/// </summary>
	public BackgroundData()
	{
	}
}
