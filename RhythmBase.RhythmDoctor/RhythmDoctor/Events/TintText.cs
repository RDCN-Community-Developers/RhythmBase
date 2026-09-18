using RhythmBase.Global.Components.Easing;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a Tint event which is a type of BaseDecorationAction and implements IEaseEvent.
/// </summary>
[JsonObjectSerializable]
public record class TintText : BaseDecorationAction, IEaseEvent, IColorEvent
{
	///<inheritdoc/>
	public EaseType Ease { get; set; }
	/// <summary>
	/// Gets or sets the border color for the tint event.
	/// </summary>
	[Tween]
	[JsonCondition($"$&.{nameof(BorderColor)} is not null")]
	public PaletteColorWithAlpha? BorderColor { get; set; } = Color.White;
	/// <summary>
	/// Gets or sets the tint color for the tint event.
	/// </summary>
	[Tween]
	[JsonCondition($"$&.{nameof(TintColor)} is not null")]
	public PaletteColorWithAlpha? TintColor { get; set; } = Color.White;
	///<inheritdoc/>
	[JsonCondition($"$&.{nameof(Duration)} != 0f")]
	public float Duration { get; set; }
	///<inheritdoc/>
	public override EventType Type => EventType.TintText;
}
