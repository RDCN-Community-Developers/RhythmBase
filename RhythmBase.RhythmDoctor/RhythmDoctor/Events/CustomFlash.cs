using RhythmBase.Global.Components.Easing;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a custom flash event.
/// </summary>
[JsonObjectSerializable]
public record class CustomFlash : BaseEvent, IEaseEvent, IRoomEvent, IColorEvent
{
	/// <inheritdoc />
	public Room Rooms { get; set; } = new Room([0]);
	/// <inheritdoc />
	public EaseType Ease { get; set; } = EaseType.Linear;
	/// <summary>
	/// Gets or sets the start color of the flash.
	/// </summary>
	public PaletteColor? StartColor { get; set; }
	/// <summary>
	/// Gets or sets the end color of the flash.
	/// </summary>
	[Tween]
	public PaletteColor? EndColor { get; set; }
	/// <inheritdoc />
	public float Duration { get; set; }
	/// <summary>
	/// Gets or sets the start opacity of the flash.
	/// <remark>
	/// Must be a value between 0 and 100, inclusive, if specified. 0 and 100 are considered as fully transparent and fully opaque, respectively.
	/// </remark>
	/// </summary>
	public int? StartOpacity { get; set; }
	/// <summary>
	/// Gets or sets the end opacity of the flash.
	/// <remark>
	/// Must be a value between 0 and 100, inclusive, if specified. 0 and 100 are considered as fully transparent and fully opaque, respectively.
	/// </remark>
	/// </summary>
	[Tween]
	public int? EndOpacity { get; set; }
	/// <summary>
	/// Gets or sets a value indicating the event affect background or foreground. If true, the flash will affect the background instead of the foreground.
	/// </summary>
	public bool Background { get; set; } = false;
	/// <summary>
	/// Gets or sets the reduced strength value, which is used to determine the intensity of the flash effect.
	/// </summary>
	/// <remarks> The value must be a non-negative integer if specified. 
	/// This property is optional, used to avoid the flash effect being too strong.
	/// </remarks>
	public int? ReducedStrength { get; set; }
	/// <inheritdoc />
	public override EventType Type => EventType.CustomFlash;

	/// <inheritdoc />
	public override Tab Tab => Tab.Actions;

	/// <inheritdoc />
	public override string ToString() => base.ToString() + $" {StartColor} {StartOpacity}%=>{EndColor} {EndOpacity}%";
}
