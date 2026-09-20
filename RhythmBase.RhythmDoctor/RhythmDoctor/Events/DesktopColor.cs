using RhythmBase.Global.Components.Easing;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that sets the simulated desktop background for the simulated desktop mode.
/// </summary>
[JsonObjectSerializable]
public record class DesktopColor : BaseWindowEvent, IEaseEvent, IColorEvent
{
	///<inheritdoc/>
	public override int Y => 0;
	/// <summary>
	/// Optional start color for an eased transition.
	/// </summary>
	[JsonCondition($"$&.{nameof(IsCustom)}")]
	public PaletteColor? StartColor { get; set; }

	/// <summary>
	/// Optional end (or sole) color for the desktop background.
	/// </summary>
	[JsonCondition($"$&.{nameof(IsCustom)}")]
	public PaletteColor? EndColor { get; set; }
	/// <summary>
	/// The brightness of the desktop background color, ranging from 0 to 100.
	/// </summary>
	[JsonCondition($"!$&.{nameof(IsCustom)}")]
	public float Brightness { get; set; } = 100f;
	///<inheritdoc/>
	public override EventType Type => EventType.DesktopColor;
	/// <summary>
	/// Indicates whether the desktop color is custom or not.
	/// </summary>
	[JsonAlias("custom")]
	public bool IsCustom { get; set; } = true;
	///<inheritdoc/>
	public EaseType Ease { get; set; }
	///<inheritdoc/>
	public float Duration { get; set; }
}
