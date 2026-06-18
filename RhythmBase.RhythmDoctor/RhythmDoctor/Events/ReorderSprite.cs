using RhythmBase.RhythmDoctor.Converters;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an action to reorder a sprite in the Rhythm Doctor game.
/// </summary>
[JsonObjectSerializable]
public record class ReorderSprite : BaseDecorationAction
{
	///<inheritdoc/>
	public override EventType Type => EventType.ReorderSprite;
	///<inheritdoc/>
	public override Tab Tab => Tab.Decorations;
	/// <summary>
	/// Gets or sets the new room to which the sprite will be moved.
	/// </summary>
	[JsonConverter(typeof(RoomIndexConverter))]
	public RoomIndex? NewRoom { get; set; }
	/// <summary>
	/// Gets or sets the target layer for the sprite reorder.
	/// </summary>
	[JsonAlias("sortingLayerName")]
	public LayerType Layer { get; set; } = LayerType.Default;
	/// <summary>
	/// Gets or sets the depth level of the object.
	/// </summary>
	/// <remarks>
	/// Leave it null to keep the original depth.
	/// </remarks>
	public int? Depth { get; set; }
	/// <summary>
	/// Gets or sets the type of layer used for sorting or rendering purposes.
	/// </summary>
	[JsonAlias("sortingLayerName")]
	public LayerType LayerType { get; set; } = LayerType.Default;
}