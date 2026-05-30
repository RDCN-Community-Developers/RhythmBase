using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Converters;
namespace RhythmBase.RhythmDoctor.Events;


/// <summary>
/// Represents an event to set a VFX preset.
/// </summary>
[JsonObjectHasSerializer(typeof(RDInstanceConverter.SetVFXPreset))]
public record class SetVFXPreset : BaseEvent, IEaseEvent, IRoomEvent, IColorEvent
{
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets the VFX preset.
	/// </summary>
	public VfxPreset Preset { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the VFX is enabled.
	/// </summary>
	public bool Enable { get; set; } = true;
	/// <summary>
	/// Gets or sets the threshold value for the VFX.
	/// </summary>
	[Tween]
	public float? Threshold { get; set; }
	/// <summary>
	/// Gets or sets the intensity of the VFX.
	/// </summary>
	[Tween]
	public float? Intensity { get; set; }
	/// <summary>
	/// Gets or sets the color of the VFX.
	/// </summary>
	[Tween]
	public PaletteColor? Color { get; set; } = Global.Components.Color.White;
	/// <summary>
	/// Gets or sets the amount by which the screen is scrolled or tiled when the effect is active.
	/// </summary>
	public Point? Amount { get; set; } = new(1, 1);
	/// <summary>
	/// Gets or sets the speed percentage for the effect.
	/// </summary>
	[Tween]
	public float? SpeedPercentage { get; set; } = 100f;
	///<inheritdoc/>
	public EaseType Ease { get; set; } = EaseType.Linear;
	///<inheritdoc/>
	public float Duration { get; set; } = 0f;
	///<inheritdoc/>
	public override EventType Type => EventType.SetVFXPreset;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
	///<inheritdoc/>
	public override string ToString() => $"{base.ToString()} {Preset}";
}
