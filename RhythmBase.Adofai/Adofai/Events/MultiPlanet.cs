namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents an event that enables multi-planet mode in the level.
/// </summary>
[JsonObjectSerializable]
public class MultiPlanet : BaseTileEvent, ISingleEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.MultiPlanet;
	/// <summary>
	/// Gets or sets the planets associated with this event.
	/// </summary>
	public Planets Planets { get; set; } = Planets.TwoPlanets;
}