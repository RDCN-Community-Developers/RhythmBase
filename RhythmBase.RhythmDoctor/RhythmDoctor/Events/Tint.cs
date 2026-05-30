using RhythmBase.Global.Components.Easing;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a Tint event which is a type of BaseDecorationAction and implements IEaseEvent.
/// </summary>
[JsonObjectSerializable]
public record class Tint : BaseDecorationAction, IEaseEvent, IColorEvent, ITintEvent
{
	///<inheritdoc/>
	public EaseType Ease { get; set; }
	/// <summary>
	/// Gets or sets the border type for the tint event.
	/// </summary>
	/// <remarks>
	/// Leave it null to keep the original border. Set it to <see cref="Border.None"/> to remove the border."/>
	/// </remarks>
	public Border? Border { get; set; }
	/// <summary>
	/// Gets or sets the border color for the tint event.
	/// </summary>
	[Tween]
	[JsonCondition($"$&.{nameof(Border)} is not null and not RhythmBase.RhythmDoctor.{nameof(Border)}.{nameof(RhythmBase.RhythmDoctor.Border.None)}")]
	public PaletteColorWithAlpha BorderColor { get; set; } = Color.White;
	/// <summary>
	/// Gets or sets a value indicating whether the hand border should pulse.
	/// </summary>
	[JsonCondition($"$&.{nameof(Border)} is not null and not RhythmBase.RhythmDoctor.{nameof(Border)}.{nameof(RhythmBase.RhythmDoctor.Border.None)} && $&.{nameof(BorderPulse)}")]
	public bool BorderPulse { get; set; }
	/// <summary>
	/// Gets or sets the minimum value for the border pulse effect.
	/// </summary>
	[JsonCondition($"$&.{nameof(Border)} is not null and not RhythmBase.RhythmDoctor.{nameof(Border)}.{nameof(RhythmBase.RhythmDoctor.Border.None)} && $&.{nameof(BorderPulse)}")]
	public float BorderPulseMin { get; set; }
	/// <summary>
	/// Gets or sets the maximum value for the border pulse effect.
	/// </summary>
	[JsonCondition($"$&.{nameof(Border)} is not null and not RhythmBase.RhythmDoctor.{nameof(Border)}.{nameof(RhythmBase.RhythmDoctor.Border.None)} && $&.{nameof(BorderPulse)}")]
	public float BorderPulseMax { get; set; }
	/// <summary>
	/// Gets or sets the opacity for the tint event.
	/// </summary>
	[Tween]
	public int? Opacity { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether this event is a tint.
	/// </summary>
	[JsonAlias("tint")]
	public bool? IsTint { get; set; }
	/// <summary>
	/// Gets or sets the tint color for the tint event.
	/// </summary>
	[Tween]
	[JsonCondition($"$&.{nameof(IsTint)} is true")]
	public PaletteColorWithAlpha TintColor { get; set; } = Color.White;
	///<inheritdoc/>
	[JsonCondition($"$&.{nameof(Duration)} != 0f")]
	public float Duration { get; set; }
	///<inheritdoc/>
	public override EventType Type => EventType.Tint;
	///<inheritdoc/>
	public override Tab Tab => Tab.Decorations;
	///<inheritdoc/>
	public override string ToString() => base.ToString() +
	                                     $" {Border}{(Border == RhythmBase.RhythmDoctor.Border.None ? "" : ":" + BorderColor.ToString())}";
}
