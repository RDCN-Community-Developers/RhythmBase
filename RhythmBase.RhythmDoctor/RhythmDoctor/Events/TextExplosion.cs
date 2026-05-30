using RhythmBase.Global.Components.Easing;
using RhythmBase.RhythmDoctor.Components;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a text explosion event in a room.
/// </summary>
[JsonObjectSerializable]
public record class TextExplosion : BaseEvent, IRoomEvent, IColorEvent
{
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets the color of the text explosion.
	/// </summary>
	public PaletteColor Color { get; set; } = Global.Components.Color.Black;
	/// <summary>
	/// Gets or sets the text to be displayed in the explosion.
	/// </summary>
	public string Text { get; set; } = "";
	/// <summary>
	/// Gets or sets the speed value.
	/// </summary>
	/// <remarks>
	/// Must be a value between 1 and 1000.
	/// </remarks>
	[JsonCondition($"$&.{nameof(Speed)} != 100f")]
    public float Speed { get; set; } = 100f;
	/// <summary>
	/// Gets or sets the direction of the text explosion.
	/// </summary>
	public TextExplosionDirection Direction { get; set; } = TextExplosionDirection.Left;
	/// <summary>
	/// Gets or sets the mode of the text explosion.
	/// </summary>
	public TextExplosionMode Mode { get; set; } = TextExplosionMode.OneColor;
	///<inheritdoc/>
	public override EventType Type => EventType.TextExplosion;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;

	/// <summary>
	/// Gets or sets the easing type.
	/// </summary>
	public EaseType Ease { get; set; }
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {Text}";
}
