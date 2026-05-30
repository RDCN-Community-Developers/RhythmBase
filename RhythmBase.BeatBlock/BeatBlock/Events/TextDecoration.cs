using RhythmBase.BeatBlock.Components;
using RhythmBase.Global.Components.Easing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Text Decoration
/// </summary>
/// <remarks>
/// Draws a string on the screen, and allows updating its properties over time
/// </remarks>
[JsonObjectSerializable]
public record class TextDecoration : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.TextDecoration;
    /// <summary>
    /// Unique ID for the text deco (if this ID does not exist, a deco will be created)
    /// </summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// Text to display.\nSeparate syllables with | to use "Advance Text Deco" event.
    /// </summary>
    public string? Text { get; set; }
    /// <summary>
    /// Localize this string?
    /// </summary>
    public bool? Localize { get; set; }
    /// <summary>
    /// Default font to use for the text. Can be overridden with tags.\nDefaults to Digital Disco.\n
    /// </summary>
    public string? Font { get; set; } = string.Empty;
    /// <summary>
    /// Default text colour
    /// </summary>
    public ColorIndex? Colour { get; set; }
    /// <summary>
    /// Width in pixels to wrap text at (-1 for no wrapping)
    /// </summary>
    public float? WrapLen { get; set; }
    /// <summary>
    /// Amount of pixels to add between every character
    /// </summary>
    public float? ExtraCharSpacing { get; set; }
    /// <summary>
    /// How to center the text.
    /// </summary>
    public TextJustification Justification { get; set; }
    /// <summary>
    /// Is the text meant to be outlined?
    /// </summary>
    public bool? Outline { get; set; }
    /// <summary>
    /// Adds an additional pixel of colour around the text
    /// </summary>
    public bool? Specialoutline { get; set; }
    /// <summary>
    /// Colour of the additional outline
    /// </summary>
    public ColorIndex? Specialcolour { get; set; }
    /// <summary>
    /// Unique ID for the parent deco (if this ID does not exist, the deco will not have a parent)
    /// </summary>
    public string? Parentid { get; set; } = string.Empty;
    /// <summary>
    /// How much the parent rotation influences the child deco
    /// </summary>
    public float? Rotationinfluence { get; set; }
    /// <summary>
    /// only rotate position, not angle
    /// </summary>
    public bool? Orbit { get; set; }
    /// <summary>
    /// X position
    /// </summary>
    public float? X { get; set; }
    /// <summary>
    /// Y position
    /// </summary>
    public float? Y { get; set; }
    /// <summary>
    /// Text rotation
    /// </summary>
    public float? R { get; set; }
    /// <summary>
    /// X scale
    /// </summary>
    public float? Sx { get; set; }
    /// <summary>
    /// Y scale
    /// </summary>
    public float? Sy { get; set; }
    /// <summary>
    /// X skew
    /// </summary>
    public float? Kx { get; set; }
    /// <summary>
    /// Y skew
    /// </summary>
    public float? Ky { get; set; }
    /// <summary>
    /// Fake Y skew - moves characters this many pixels down for every drawn character
    /// </summary>
    public float? KyFake { get; set; }
    /// <summary>
    /// Layer to render on
    /// </summary>
    public Layer? DrawLayer { get; set; }
    /// <summary>
    /// Order in layer, lower is drawn first
    /// </summary>
    public int? DrawOrder { get; set; }
    /// <summary>
    /// Hide the deco
    /// </summary>
    public bool? Hide { get; set; }
    /// <summary>
    /// Enable alpha dithering for this deco
    /// </summary>
    public bool? Alphadither { get; set; }
    /// <summary>
    /// Dither percent (0-1)
    /// </summary>
    public float? Ditherpercent { get; set; }
    /// <summary>
    /// Draw this deco on the effect canvas instead of the regular canvas
    /// </summary>
    public bool? EffectCanvas { get; set; }
    /// <summary>
    /// Draw to the effect canvas without recoloring?
    /// </summary>
    public bool? EffectCanvasRaw { get; set; }
    /// <summary>
    /// Length of ease (IGNORED IF DECO IS JUST BEING CREATED)
    /// </summary>
    public float? Duration { get; set; }
    /// <summary>
    /// Ease function to use (IGNORED IF DECO IS JUST BEING CREATED)
    /// </summary>
    public EaseType? Ease { get; set; }
}
