using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that changes the CPU type of rows.
/// </summary>
[JsonObjectSerializable]
public record class ChangePlayersRows : BaseEvent
{
	/// <summary>
	/// Initializes a new instance of the ChangePlayersRows class.
	/// </summary>
	public ChangePlayersRows()
	{
		Array.Fill(CpuMarkers, GameCharacter.Otto);
	}
	/// <summary>
	/// Gets or sets the list of players.
	/// </summary>
	public PlayerTypeGroup Players { get; set; } = new(PlayerType.NoChange);
	/// <summary>
	/// Gets or sets the player mode.
	/// </summary>
	public PlayingMode PlayerMode { get; set; } = PlayingMode.OnePlayer;
	/// <summary>
	/// Gets or sets the list of CPU markers.
	/// </summary>
	public GameCharacter[] CpuMarkers { get; set; } = new GameCharacter[16];
	/// <summary>
	/// Gets or sets whether the row flash animation triggers on the beat.
	/// </summary>
	[JsonAlias("flashingOnBeat")]
	[JsonCondition($"!$&.{nameof(FlashOnBeat)}")]
	public bool FlashOnBeat { get; set; } = true;
	/// <inheritdoc />
	public override EventType Type => EventType.ChangePlayersRows;

	/// <inheritdoc />
	public override Tab Tab => Tab.Actions;
}