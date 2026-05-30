using RhythmBase.Global.Components.Easing;
using RhythmBase.RhythmDoctor.Components;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that tints rows with specified colors and effects.
/// </summary>
[JsonObjectSerializable]
public record class TintRows : BaseRowAction, IEaseEvent, IColorEvent, IRoomEvent, ITintEvent
{
	/// <summary>
	/// Initializes a new instance of the TintRows class with the row index set to an initial value.
	/// </summary>
	public TintRows()
	{
		_row = -1;
	}
	/// <summary>
	/// Gets or sets the tint color.
	/// </summary>
	[JsonCondition($"$&.{nameof(IsTint)} is true")]
	public PaletteColorWithAlpha TintColor { get; set; } = Color.White;
	///<inheritdoc/>
	[JsonCondition($"$&.{nameof(Duration)} != 0f")]
	public EaseType Ease { get; set; }
	/// <summary>
	/// Gets or sets the border style.
	/// </summary>
	public Border? Border { get; set; }
	/// <summary>
	/// Gets or sets the border color.
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
	/// Gets or sets the opacity level.
	/// </summary>
	[Tween]
	public int? Opacity { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether to apply tint.
	/// </summary>
	[JsonAlias("tint")]
	public bool? IsTint { get; set; }
	///<inheritdoc/>
	[JsonCondition($"$&.{nameof(Duration)} != 0f")]
	public float Duration { get; set; }
	/// <summary>
	/// Gets or sets the row effect.
	/// </summary>
	public TintRowEffect? Effect { get; set; } = TintRowEffect.None;
	///<inheritdoc/>
	public override EventType Type => EventType.TintRows;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
	/// <summary>
	/// Gets or sets the heart type associated with the entity.
	/// </summary>
	/// <remarks>
	/// Leave it null to keep the original heart type.
	/// </remarks>
	public HeartType? Heart { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the heart transition effect is enabled.
	/// </summary>
	public bool HeartTransition { get; set; }
	/// <inheritdoc/>
	[JsonCondition($"$&.{nameof(Index)} == -1")]
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets a value indicating whether to tint all rows.
	/// </summary>
	[JsonIgnore]
	public bool TintAll => Parent is null;
	///<inheritdoc/>
	public override string ToString() => base.ToString() +
	                                     $" {Border}{(Border == RhythmBase.RhythmDoctor.Border.None ? "" : ":" + BorderColor.ToString())}";
}

