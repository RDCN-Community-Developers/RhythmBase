using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Defines an interface for events that display text in a rhythm doctor level.
/// </summary>
public interface ITextEvent : IBaseEvent
{
	/// <summary>
	/// Gets or sets the anchor style of the text.
	/// </summary>
	FloatingTextAnchorStyle Anchor { get; set; }
	/// <summary>
	/// Gets or sets the angle of the text.
	/// <remark>
	/// Unit is degree.
	/// </remark>
	/// </summary>
	float? Angle { get; set; }
	/// <summary>
	/// Gets or sets the color of the text.
	/// </summary>
	PaletteColorWithAlpha Color { get; set; }
	/// <summary>
	/// Gets or sets the fade out rate of the text.
	/// <remark>
	/// Must be a non-negative value.
	/// </remark>
	/// </summary>
	float Duration { get; set; }
	/// <summary>
	/// Gets or sets the mode of the text.
	/// </summary>
	FloatingTextFadeOutMode Mode { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether to narrate the text.
	/// </summary>
	bool Narrate { get; set; }
	/// <summary>
	/// Gets or sets the narration category of the text.
	/// </summary>
	NarrationCategory NarrationCategory { get; set; }
	/// <summary>
	/// Gets or sets the outline color of the text.
	/// </summary>
	PaletteColorWithAlpha OutlineColor { get; set; }
	/// <summary>
	/// Gets or sets the position of the text.
	/// </summary>
	Point? Position { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether to show child texts.
	/// </summary>
	bool ShowChildren { get; set; }
	/// <summary>
	/// Gets or sets the size of the text.
	/// <remark>
	/// Must be a non-negative value.
	/// </remark>
	/// </summary>
	int Size { get; set; }
	/// <summary>
	/// Gets or sets the text content.
	/// </summary>
	string Text { get; set; }
}