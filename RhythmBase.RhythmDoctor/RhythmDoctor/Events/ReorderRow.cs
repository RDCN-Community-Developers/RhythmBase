using RhythmBase.RhythmDoctor.Converters;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>  
/// Represents an action to reorder a row in the Rhythm Doctor editor.  
/// </summary>  
[JsonObjectSerializable]
public record class ReorderRow : BaseRowAction
{
	/// <inheritdoc/>  
	public override EventType Type => EventType.ReorderRow;
	/// <inheritdoc/>  
	public override Tab Tab => Tab.Actions;
	/// <summary>  
	/// Gets or sets the new room to which the row will be moved.  
	/// </summary>  
	[JsonDefaultSerializer]
	[JsonConverter(typeof(RoomIndexConverter))]
	public RoomIndex? NewRoom { get; set; }
	/// <summary>
	/// Gets or sets the target layer for the row reorder.
	/// </summary>
	[JsonAlias("sortingLayerName")]
	public LayerType Layer { get; set; } = LayerType.Default;
	/// <summary>
	/// Gets or sets the order of the room.
	/// </summary>
	/// <remarks>
	/// Leave it null to keep the original order.
	/// </remarks>
	public int? Order { get; set; }
	/// <summary>
	/// Gets or sets the sorting order for the row.
	/// </summary>
	public int? SortingOrder { get; set; }
	/// <summary>  
	/// Gets or sets the transition type for reordering the row.  
	/// </summary>  
	public Transition Transition { get; set; } = Transition.Smooth;
}
