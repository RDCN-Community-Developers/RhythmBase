namespace RhythmBase.Rizline.Components;

/// <summary>
/// Holds a single theme's colors used by a level (background, object and UI colors).
/// </summary>
public record class Theme
{
    /// <summary>
    /// Background color for the theme. 
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Primary object color (notes, bars, stars, mods). 
    /// </summary>
    public Color ObjectColor { get; set; }

    /// <summary>
    /// UI color used for hit effects and interface elements. 
    /// </summary>
    public Color UserInterfaceColor { get; set; }
}
