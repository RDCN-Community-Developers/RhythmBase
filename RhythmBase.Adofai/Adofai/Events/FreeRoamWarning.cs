using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents a warning marker for free roam mode on a specific tile.
/// </summary>
/// <remarks>
/// This event is a tile-scoped event used by the ADOFAI editor to indicate a free-roam warning
/// at a particular position relative to the tile. It inherits common event behavior from
/// <see cref="BaseTileEvent"/> and its <see cref="Type"/> identifies it as <see cref="EventType.FreeRoamWarning"/>.
/// </remarks>
[JsonObjectSerializable]
public class FreeRoamWarning : BaseTileEvent
{
	/// <summary>
	/// Gets the type of this event.
	/// </summary>
	/// <value>
	/// Always returns <see cref="EventType.FreeRoamWarning"/> to identify this event kind.
	/// </value>
	public override EventType Type => EventType.FreeRoamWarning;

	/// <summary>
	/// Gets or sets the position of the warning relative to the tile.
	/// </summary>
	/// <value>
	/// A non-nullable <see cref="PointN"/> that specifies the horizontal (X) and vertical (Y)
	/// offset of the warning marker. The default value is <c>new RDPointN(1, 0)</c>.
	/// </value>
	/// <remarks>
	/// The meaning of the coordinates depends on the editor's coordinate system for tile events.
	/// Consumers can read or modify this property to change where the warning is displayed on the tile.
	/// </remarks>
	public PointN Position { get; set; } = new(1, 0);
}