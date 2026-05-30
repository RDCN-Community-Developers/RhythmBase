using RhythmBase.Global.Components;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Palette color
/// </summary>
/// <remarks>
/// 
/// </remarks>
public struct PaletteColorWithAlpha
{
	/// <summary>
	/// Get or set a custom color.
	/// </summary>
	public Color Color
	{
		readonly get
		{
			Color Color = EnablePanel ? default : _color;
			return Color;
		}
		set
		{
			_panel = -1;
			_color = value;
		}
	}
	/// <summary>
	/// Go back to or set the palette color index.
	/// </summary>
	public int PaletteIndex
	{
		readonly get => _panel;
		set
		{
			if (value >= 0)
			{
				_color = default;
				_panel = value;
			}
		}
	}
	/// <summary>
	/// Specifies whether this object is used for this color.
	/// </summary>
	public readonly bool EnablePanel => PaletteIndex >= 0;

	/// <summary>
	/// The actual color of this object.<br />
	/// If comes from a palette, it's a palette color.
	/// If not, it's a custom color.
	/// </summary>
	public readonly Color Value => EnablePanel ? default : _color;
    /// <summary>
    /// Initializes a new instance of <see cref="PaletteColor"/> using the provided custom color.
    /// </summary>
    /// <param name="color">The custom <see cref="Global.Components.Color"/> value to be assigned.</param>
    public PaletteColorWithAlpha(Color color)
	{
		Color = color;
	}
	/// <summary>
	/// Initializes a new instance of <see cref="PaletteColor"/> with a default color value.
	/// </summary>
	/// <param name="index">The index of the palette color to be used.</param>
	public PaletteColorWithAlpha(int index)
	{
		Color = default; this.PaletteIndex = index;
	}
	/// <inheritdoc/>
	public readonly override string ToString() => EnablePanel ? $"[{_panel}][?]" : "[-]" + Value.ToString("#AARRGGBB");
	/// <summary>
	/// Deserializes the specified string value into the current <see cref="PaletteColor"/> instance.
	/// </summary>
	/// <param name="value">
	/// The string representation of the palette color. If the value starts with "pal", it is interpreted as a palette index (e.g., "pal3").
	/// Otherwise, it is interpreted as a custom color in RGBA format.
	/// </param>
	public void Deserialize(string value)
	{
		if (value.StartsWith("pal"))
		{
			_panel = int.Parse(value[3..]);
			_color = default;
		}
		else
		{
			_color = Color.FromRgba(value);
			_panel = -1;
		}
	}
	/// <summary>
	/// Serializes the current <see cref="PaletteColorWithAlpha"/> instance to a string representation.
	/// </summary>
	/// <returns>
	/// A string representing the palette color. If the color is from a palette, returns "pal" followed by the palette index.
	/// Otherwise, returns the color in "RRGGBBAA" or "RRGGBB" format depending on whether alpha is enabled.
	/// </returns>
	public readonly string Serialize() => EnablePanel ? $"pal{_panel}" : Value.ToString("RRGGBBAA");
    /// <summary>  
    /// Implicitly converts a <see cref="PaletteColorWithAlpha"/> instance to an <see cref="Global.Components.Color"/>.  
    /// </summary>  
    /// <param name="paletteColor">The <see cref="PaletteColorWithAlpha"/> instance to convert.</param>  
    /// <returns>The <see cref="Global.Components.Color"/> value of the <see cref="PaletteColorWithAlpha"/>.</returns>  
    public static implicit operator Color(PaletteColorWithAlpha paletteColor) => paletteColor.Value;
    /// <summary>  
    /// Implicitly converts an <see cref="Global.Components.Color"/> to a <see cref="PaletteColorWithAlpha"/> instance.  
    /// </summary>  
    /// <param name="color">The <see cref="Global.Components.Color"/> to convert.</param>  
    /// <returns>A new <see cref="PaletteColorWithAlpha"/> instance with the specified <see cref="Global.Components.Color"/>.</returns>  
    public static implicit operator PaletteColorWithAlpha(Color color) => new(color);
	private int _panel;
	private Color _color;
}
