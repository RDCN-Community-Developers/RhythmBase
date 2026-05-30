namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents the status of a classic beat.
/// </summary>
public struct ClassicBeatStatus
{
	/// <summary>
	/// Gets or sets the status type of the beat.
	/// </summary>
	public StatusType Status;
	/// <summary>
	/// Gets or sets the beat count.
	/// </summary>
	public ushort BeatCount;
	/// <summary>
	/// Defines the various status types for a classic beat.
	/// </summary>
	public enum StatusType
	{
#pragma warning disable CS1591
		Unset = -1,
		None,
		Synco,
		Beat_Open,
		Beat_Flash,
		Beat_Double_Flash,
		Beat_Triple_Flash,
		Beat_Close,
		X_Open,
		X_Flash,
		X_Close,
		X_Synco_Open,
		X_Synco_Flash,
		X_Synco_Close,
		Up_Open,
		Up_Close,
		Down_Open,
		Down_Close,
		Swing_Left,
		Swing_Right,
		Swing_Bounce,
		Held_Start,
		Held_End
#pragma warning restore CS1591
	}
}
