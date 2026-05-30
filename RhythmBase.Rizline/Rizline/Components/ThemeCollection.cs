namespace RhythmBase.Rizline.Components;

/// <summary>
/// Collection of themes used by a level. The first theme is considered the default
/// and subsequent entries represent Riztime themes (if any).
/// </summary>
public class ThemeCollection
{
    /// <summary>
    /// Default (normal) theme for the level. 
    /// </summary>
    public Theme MainTheme { get; set; } = new Theme();

    /// <summary>
    /// Optional list of Riztime themes following the default theme. 
    /// </summary>
    public List<Theme> RiztimeThemes { get; } = [];
}
