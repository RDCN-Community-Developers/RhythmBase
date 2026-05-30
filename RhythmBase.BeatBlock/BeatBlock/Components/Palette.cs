namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Represents a color palette containing up to 8 colors.
/// </summary>
public struct Palette()
{
    private readonly Color[] colors = new Color[8];
    /// <summary>
    /// Gets or sets the color at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the color.</param>
    /// <returns>The <see cref="Color"/> at the specified index.</returns>
    public Color this[int index]
    {
        get => colors[index];
        set => colors[index] = value;
    }
    /// <summary>
    /// Gets the default palette.
    /// </summary>
    public static Palette Default => new()
    {
        [0] = White,
        [1] = Black,
        [2] = Red,
        [3] = Blue,
        [4] = Green,
        [5] = Yellow,
        [6] = Pink,
        [7] = Cyan,
    };
    /// <summary>
    /// Gets the black color.
    /// </summary>
    public static Color Black => 0xFF000000;
    /// <summary>
    /// Gets the red color.
    /// </summary>
    public static Color Red => 0xFFFF0000;
    /// <summary>
    /// Gets the blue color.
    /// </summary>
    public static Color Blue => 0xFF0000FF;
    /// <summary>
    /// Gets the grey color.
    /// </summary>
    public static Color Grey => 0xFFD4D4D4;
    /// <summary>
    /// Gets the gray color.
    /// </summary>
    public static Color Gray => 0xFFD4D4D4;
    /// <summary>
    /// Gets the white color.
    /// </summary>
    public static Color White => 0xFFFFFFFF;
    /// <summary>
    /// Gets the pink color.
    /// </summary>
    public static Color Pink => 0xFFFF00FF;
    /// <summary>
    /// Gets the magenta color.
    /// </summary>
    public static Color Magenta => 0xFFFF00FF;
    /// <summary>
    /// Gets the green color.
    /// </summary>
    public static Color Green => 0xFF00FF00;
    /// <summary>
    /// Gets the yellow color.
    /// </summary>
    public static Color Yellow => 0xFFFFFF00;
    /// <summary>
    /// Gets the cyan color.
    /// </summary>
    public static Color Cyan => 0xFF00FFFF;
}
