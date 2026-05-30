namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Load Custom Font
/// </summary>
/// <remarks>
/// Loads a custom font file from your custom level folder
/// </remarks>
[JsonObjectSerializable]
public record class LoadCustomFont : BaseEvent
{
    /// <inheritdoc/>
    public override EventType Type => EventType.LoadCustomFont;
    /// <summary>
    /// Filename of the font.\nMust be either a font file or a specially defined .png.
    /// </summary>
    public string Fontpath { get; set; } = string.Empty;
    /// <summary>
    /// Which character map to use for pixel fonts.\n"regular" is ASCII 32~127, "full" is 32~255.\nIgnored if using a truetype font.
    /// </summary>
    public CharacterMap CharacterMap { get; set; }
    /// <summary>
    /// Custom character map (may require json editing)
    /// </summary>
    public string CustomMap { get; set; } = string.Empty;
    /// <summary>
    /// Size to render truetype fonts at.\nIgnored if using a pixel font.
    /// </summary>
    public float? TtfFontSize { get; set; }
    /// <summary>
    /// Pixels between individual characters.\nIgnored if using a truetype font.
    /// </summary>
    public float? PixelCharSpacing { get; set; }
    /// <summary>
    /// What font should be used as a fallback if this font is missing characters? *This font has to be the same type (ttf, png) as the fallback.*
    /// </summary>
    public string? FallbackFont { get; set; } = string.Empty;
}
