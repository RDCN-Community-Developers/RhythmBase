using System.Diagnostics;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Palette color
/// </summary>
/// <remarks>
/// 
/// </remarks>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct PaletteColor
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
			_color = value.WithAlpha(byte.MaxValue);
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
	/// Initializes a new instance of <see cref="PaletteColor"/> using the provided custom color.
	/// </summary>
	/// <param name="color">The custom <see cref="Global.Components.Color"/> value to be assigned.</param>
	public PaletteColor(Color color)
	{
		Color = color;
	}
	/// <summary>
	/// Initializes a new instance of <see cref="PaletteColor"/> with a default color value.
	/// </summary>
	/// <param name="panelIndex">The index of the palette color to be used.</param>
	public PaletteColor(int panelIndex)
	{
		_panel = panelIndex;
	}
	/// <inheritdoc/>
	public readonly override string ToString() => EnablePanel ? $"[{_panel}][?]" : "[-]" + _color.ToString("#RRGGBB");
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
	/// Serializes the current <see cref="PaletteColor"/> instance to a string representation.
	/// </summary>
	/// <returns>
	/// A string representing the palette color. If the color is from a palette, returns "pal" followed by the palette index.
	/// Otherwise, returns the color in "RRGGBBAA" or "RRGGBB" format depending on whether alpha is enabled.
	/// </returns>
	public readonly string Serialize() => EnablePanel ? $"pal{_panel}" : _color.ToString("RRGGBB");
	/// <summary>  
	/// Implicitly converts an <see cref="Global.Components.Color"/> to a <see cref="PaletteColor"/> instance.  
	/// </summary>  
	/// <param name="color">The <see cref="Global.Components.Color"/> to convert.</param>  
	/// <returns>A new <see cref="PaletteColor"/> instance with the specified <see cref="Global.Components.Color"/>.</returns>  
	public static implicit operator PaletteColor(Color color) => new(color);
	private int _panel;
	private Color _color;
	private readonly string GetDebuggerDisplay()
	{
		return ToString();
	}
}
