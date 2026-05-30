using RhythmBase.Global.Components;
using RhythmBase.Global.Events;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Converters;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a room mask event.
/// </summary>
[JsonObjectSerializable]
public record class MaskRoom : BaseEvent, IColorEvent, IImageFileEvent
{
	/// <summary>
	/// Gets or sets the type of the mask.
	/// </summary>
	public RoomMaskType MaskType { get; set; } = RoomMaskType.None;
	/// <summary>
	/// Gets or sets the alpha mode.
	/// </summary>
	[JsonCondition($"$&.{nameof(MaskType)} != RhythmBase.RhythmDoctor.{nameof(RoomMaskType)}.{nameof(RoomMaskType.None)}")]
	public MaskAlphaMode AlphaMode { get; set; } = MaskAlphaMode.Normal;
	/// <summary>
	/// Gets or sets the source room.
	/// </summary>
	[JsonConverter(typeof(RoomIndexConverter))]
	[JsonCondition($"$&.{nameof(MaskType)} == RhythmBase.RhythmDoctor.{nameof(RoomMaskType)}.{nameof(RoomMaskType.Room)}")]
	public RoomIndex SourceRoom { get; set; } = RoomIndex.Room1;
	/// <summary>
	/// Gets or sets the list of image assets.
	/// </summary>
	[JsonCondition($"$&.{nameof(MaskType)} == RhythmBase.RhythmDoctor.{nameof(RoomMaskType)}.{nameof(RoomMaskType.Image)}")]
	public List<FileReference> Image { get; set; } = [];
	/// <summary>
	/// Gets or sets the frames per second.
	/// </summary>
	[JsonCondition($"$&.{nameof(Image)}.Count > 1")]
	public float Fps { get; set; } = 30;
	/// <summary>
	/// Gets or sets the key color.
	/// </summary>
	[JsonCondition($"$&.{nameof(MaskType)} == RhythmBase.RhythmDoctor.{nameof(RoomMaskType)}.{nameof(RoomMaskType.Color)}")]
	public PaletteColorWithAlpha KeyColor { get; set; } = Color.White;
	/// <summary>
	/// Gets or sets the color cutoff value.
	/// </summary>
	[JsonCondition($"$&.{nameof(MaskType)} == RhythmBase.RhythmDoctor.{nameof(RoomMaskType)}.{nameof(RoomMaskType.Color)}")]
	public int ColorCutoff { get; set; } = 0;
	/// <summary>
	/// Gets or sets the color feathering value.
	/// </summary>
	[JsonCondition($"$&.{nameof(MaskType)} == RhythmBase.RhythmDoctor.{nameof(RoomMaskType)}.{nameof(RoomMaskType.Color)}")]
	public int ColorFeathering { get; set; } = 0;
	///<inheritdoc/>
	public override EventType Type => EventType.MaskRoom;
	///<inheritdoc/>
	public override Tab Tab => Tab.Rooms;

	/// <summary>
	/// Gets the room associated with the event.
	/// </summary>
	public Room Room => new SingleRoom(checked((byte)Y));
	IEnumerable<FileReference> IImageFileEvent.ImageFiles => [.. Image];
	IEnumerable<FileReference> IFileEvent.Files => [.. Image];
}
