using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Serialization;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a event that displays floating text on the screen, which can be used for various purposes such as showing lyrics, dialogue, or other textual information during gameplay.
/// </summary>
[JsonObjectSerializable]
public record class SetText : BaseDecorationAction, IRoomEvent, IDurationEvent, IColorEvent, IFontFileEvent, ITextEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.SetText;
	/// <inheritdoc/>
	public override Tab Tab => Tab.Actions;
	/// <summary>
	/// Gets or sets the room associated with the event.
	/// </summary>
	public Room Rooms { get; set; } = new Room([0]);
	/// <inheritdoc/>
	[JsonAlias("fadeOutRate")]
	public float Duration { get; set; }
	/// <inheritdoc/>
	public PaletteColorWithAlpha Color { get; set; } = Global.Components.Color.White;
	/// <inheritdoc/>
	public float? Angle { get; set; }
	/// <inheritdoc/>
	public int Size { get; set; } = 8;
	/// <inheritdoc/>
	public PaletteColorWithAlpha OutlineColor { get; set; } = Global.Components.Color.Black;
	/// <inheritdoc/>
	[JsonAlias("textPosition")]
	public Point? Position { get; set; }
	/// <inheritdoc/>
	[JsonConverter(typeof(FloatingTextAnchorStylesConverter))]
	public FloatingTextAnchorStyle Anchor { get; set; } = FloatingTextAnchorStyle.Center;
	/// <inheritdoc/>
	[JsonCondition($"$&.{nameof(Narrate)}")]
	public bool Narrate { get; set; } = true;
	/// <inheritdoc/>
	[JsonCondition($"$&.{nameof(Narrate)}")]
	public NarrationCategory NarrationCategory { get; set; } = NarrationCategory.Subtitles;
	/// <inheritdoc/>
	public FloatingTextFadeOutMode Mode { get; set; } = FloatingTextFadeOutMode.FadeOut;
	/// <inheritdoc/>
	public bool ShowChildren { get; set; } = true;
	/// <inheritdoc/>
	public string Text { get; set; } = "等/呀/等/得/好/心/慌……";
	/// <inheritdoc/>
	public FontName Font { get; set; } = FontName.Default;
	/// <summary>
	/// Initializes a new instance of the <see cref="FloatingText"/> class.
	/// </summary>
	public SetText() { }
	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	IEnumerable<FileReference> IFontFileEvent.FontFiles => Font.IsCustom ? [Font.Value] : [];
	IEnumerable<FileReference> IFileEvent.Files => Font.IsCustom ? [Font.Value] : [];
}