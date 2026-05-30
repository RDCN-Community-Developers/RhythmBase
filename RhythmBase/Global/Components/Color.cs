using System.Buffers.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Components;

/// <summary>
/// Represents a color with red, green, blue, and alpha channel.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial struct Color(uint hex) :
#if NET7_0_OR_GREATER
	IEqualityOperators<Color, Color, bool>,
#endif
	IEquatable<Color>, IFormattable
{
	/// <summary>
	/// #AARRGGBB
	/// </summary>
	private uint _color = hex;
	/// <summary>
	/// Gets or sets the alpha component of the color.
	/// </summary>
	public byte A
	{
		readonly get => (byte)(_color >> 24 & 0xFFu);
		set => _color = _color & 0x00FFFFFFu | (uint)value << 24;
	}
	/// <summary>
	/// Gets or sets the red component of the color.
	/// </summary>
	public byte R
	{
		readonly get => (byte)(_color >> 16 & 0xFFu);
		set => _color = _color & 0xFF00FFFFu | (uint)value << 16;
	}
	/// <summary>
	/// Gets or sets the green component of the color.
	/// </summary>
	public byte G
	{
		readonly get => (byte)(_color >> 8 & 0xFFu);
		set => _color = _color & 0xFFFF00FFu | (uint)value << 8;
	}
	/// <summary>
	/// Gets or sets the blue component of the color.
	/// </summary>
	public byte B
	{
		readonly get => (byte)(_color & 0xFFu);
		set => _color = _color & 0xFFFFFF00u | value;
	}
	/// <summary>
	/// Returns a new Color instance with the specified alpha value.
	/// </summary>
	/// <param name="alpha">The alpha value to set.</param>
	/// <returns>A new Color instance with the specified alpha value.</returns>
	public readonly Color WithAlpha(byte alpha) => new(_color & 0x00FFFFFFu | (uint)alpha << 24);
	/// <summary>
	/// Returns a new Color instance with the specified red value.
	/// </summary>
	/// <param name="red">The red value to set.</param>
	/// <returns>A new Color instance with the specified red value.</returns>
	public readonly Color WithRed(byte red) => new(_color & 0xFF00FFFFu | (uint)red << 16);
	/// <summary>
	/// Returns a new Color instance with the specified green value.
	/// </summary>
	/// <param name="green">The green value to set.</param>
	/// <returns>A new Color instance with the specified green value.</returns>
	public readonly Color WithGreen(byte green) => new(_color & 0xFFFF00FFu | (uint)green << 8);
	/// <summary>
	/// Returns a new Color instance with the specified blue value.
	/// </summary>
	/// <param name="blue">The blue value to set.</param>
	/// <returns>A new Color instance with the specified blue value.</returns>
	public readonly Color WithBlue(byte blue) => new(_color & 0xFFFFFF00u | blue);
	/// <summary>
	/// Converts the color to HSL (Hue, Saturation, Lightness) color space.
	/// </summary>
	/// <param name="h">The hue component.</param>
	/// <param name="s">The saturation component.</param>
	/// <param name="l">The lightness component.</param>
	public readonly void ToHsl(out float h, out float s, out float l)
	{
		float r = R / 255f;
		float g = G / 255f;
		float b = B / 255f;

		float max = Math.Max(r, Math.Max(g, b));
		float min = Math.Min(r, Math.Min(g, b));
		float delta = max - min;
		h = 0;
		if (delta != 0)
		{
			if (max == r)
			{
				h = (g - b) / delta + (g < b ? 6 : 0);
			}
			else if (max == g)
			{
				h = (b - r) / delta + 2;
			}
			else
			{
				h = (r - g) / delta + 4;
			}
			h /= 6;
		}
		l = (max + min) / 2;
		s = delta == 0 ? 0 : delta / (1 - Math.Abs(2 * l - 1));
		h *= 360;
		s *= 100;
		l *= 100;
	}
	/// <summary>
	/// Converts the color to HSV (Hue, Saturation, Value) color space.
	/// </summary>
	/// <param name="h">The hue component.</param>
	/// <param name="s">The saturation component.</param>
	/// <param name="v">The value component.</param>
	public readonly void ToHsv(out float h, out float s, out float v)
	{
		float r = R / 255f;
		float g = G / 255f;
		float b = B / 255f;

		float max = Math.Max(r, Math.Max(g, b));
		float min = Math.Min(r, Math.Min(g, b));
		float delta = max - min;
		h = 0f;
		if (delta != 0)
		{
			if (max == r)
			{
				h = (g - b) / delta + (g < b ? 6 : 0);
			}
			else if (max == g)
			{
				h = (b - r) / delta + 2;
			}
			else if (max == b)
			{
				h = (r - g) / delta + 4;
			}
			h /= 6;
		}
		s = max == 0 ? 0 : delta / max;
		v = max;
		h *= 360;
		s *= 100;
		v *= 100;
	}
	/// <summary>
	/// Creates an Color instance from RGBA values.
	/// </summary>
	/// <param name="r">Red component</param>
	/// <param name="g">Green component</param>
	/// <param name="b">Blue component</param>
	/// <param name="a">Alpha component (default is 255)</param>
	/// <returns>Color instance</returns>
	public static Color FromRgba(byte r, byte g, byte b, byte a = 255) => (uint)(a << 24 | r << 16 | g << 8 | b);
	/// <summary>
	/// Attempts to create an <see cref="Color"/> instance from a hexadecimal RGBA color representation.
	/// </summary>
	/// <remarks>This method throws <see cref="ArgumentException"/> for invalid input. 
	/// The method supports both shorthand (e.g., RGB, RGBA) and
	/// full-length (e.g., RRGGBB, RRGGBBAA) hexadecimal color representations.</remarks>
	/// <param name="hex">A string representing the hexadecimal ARGB color. The input can optionally start with a '#'
	/// character and must be in one of the following formats: RGB (3 characters), ARGB (4 characters), RRGGBB (6
	/// characters), or AARRGGBB (8 characters).</param>
	/// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
	public static Color FromRgba(string hex) => TryFromRgba(hex, out Color color) ? color : throw new ArgumentException("");
	/// <summary>
	/// Attempts to create an <see cref="Color"/> instance from a hexadecimal RGBA color representation.
	/// </summary>
	/// <remarks>This method throws <see cref="ArgumentException"/> for invalid input. 
	/// The method supports both shorthand (e.g., RGB, RGBA) and
	/// full-length (e.g., RRGGBB, RRGGBBAA) hexadecimal color representations.</remarks>
	/// <param name="hex">A read-only span of bytes representing the hexadecimal RGBA color. The input can optionally start with a '#'
	/// character and must be in one of the following formats: RGB (3 characters), RGBA (4 characters), RRGGBB (6
	/// characters), or RRGGBBAA (8 characters).</param>
	/// <exception cref="ArgumentException">Thrown when the hexadecimal string length is not 3, 4, 6, or 8.</exception>
	/// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
	public static Color FromRgba(ReadOnlySpan<byte> hex) => TryFromRgba(hex, out Color color) ? color : throw new ArgumentException("");
	/// <summary>
	/// Creates an Color instance from a 32-bit RGBA value.
	/// </summary>
	/// <param name="hex">The 32-bit RGBA value.</param>
	/// <returns>A new Color instance.</returns>
	public static Color FromRgba(uint hex) => hex & 0x00FFFFFFu | ((hex & 0xFFu) << 24);
	/// <summary>
	/// Converts a byte representing a hexadecimal character to its corresponding numeric value.
	/// </summary>
	/// <param name="b">The byte representing a hexadecimal character. Must be a valid ASCII character in the ranges '0'-'9', 'a'-'f', or
	/// 'A'-'F'.</param>
	/// <returns>The numeric value of the hexadecimal character, where '0'-'9' map to 0-9, 'a'-'f' map to 10-15, and 'A'-'F' map to
	/// 10-15.</returns>
	/// <exception cref="ArgumentException">Thrown if <paramref name="b"/> is not a valid hexadecimal character.</exception>
	private static byte FromChar(byte b)
	{
		return b switch
		{
			>= (byte)'0' and <= (byte)'9' => (byte)(b - (byte)'0'),
			>= (byte)'a' and <= (byte)'f' => (byte)(b - (byte)'a' + 10),
			>= (byte)'A' and <= (byte)'F' => (byte)(b - (byte)'A' + 10),
			_ => throw new ArgumentException("Invalid hex character."),
		};
	}
	/// <summary>
	/// Attempts to create an <see cref="Color"/> instance from a hexadecimal RGBA color string.
	/// </summary>
	/// <remarks>This method does not throw exceptions for invalid input. Instead, it returns <see
	/// langword="false"/> if the input is not in a valid format. The method supports both shorthand (e.g., RGB, RGBA) and
	/// full-length (e.g., RRGGBB, RRGGBBAA) hexadecimal color representations.</remarks>
	/// <param name="hex">A string representing the color in hexadecimal format. The string may optionally start with a '#' character and
	/// can be in one of the following formats: RGB (3 characters), RGBA (4 characters), RRGGBB (6 characters), or
	/// RRGGBBAA (8 characters).</param>
	/// <param name="color">When this method returns, contains the <see cref="Color"/> instance corresponding to the specified hexadecimal
	/// color string, if the conversion succeeded; otherwise, contains the default value of <see cref="Color"/>.</param>
	/// <returns><see langword="true"/> if the hexadecimal string was successfully converted to an <see cref="Color"/>;
	/// otherwise, <see langword="false"/>.</returns>
	public static bool TryFromRgba(string hex, out Color color)
	{
		color = default;
		if (string.IsNullOrEmpty(hex))
			return false;
		hex = hex.Trim();
		if (hex.Length == 0)
		{
			color = default;
			return false;
		}
		if (hex[0] == '#')
			hex = hex[1..];
		if (!uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint value))
			return false;
		switch (hex.Length)
		{
			case 3:
				color =
					0xFF000000 |
					(((value & 0xF00) * 0x11) << 8) |
					(((value & 0x0F0) * 0x11) << 4) |
					(((value & 0x00F) * 0x11));
				return true;
			case 4:
				color =
					(((value & 0xF000) * 0x11) << 4) |
					(((value & 0x0F00) * 0x11)) |
					(((value & 0x00F0) * 0x11) >> 4) |
					(((value & 0x000F) * 0x11) << 24);
				return true;
			case 6:
				color =
					0xFF000000 | value;
				return true;
			case 8:
				color =
					((value & 0x000000FF) << 24) |
					((value & 0xFFFFFF00) >> 8);
				return true;
			default:
				color = default;
				return false;
		}
	}
	/// <summary>
	/// Attempts to create an <see cref="Color"/> instance from a hexadecimal RGBA color representation.
	/// </summary>
	/// <remarks>This method does not throw exceptions for invalid input. Instead, it returns <see
	/// langword="false"/> if the input is not in a valid format. The method supports both shorthand (e.g., RGB, RGBA) and
	/// full-length (e.g., RRGGBB, RRGGBBAA) hexadecimal color representations.</remarks>
	/// <param name="hex">A read-only span of bytes representing the hexadecimal RGBA color. The input can optionally start with a '#'
	/// character and must be in one of the following formats: RGB (3 characters), RGBA (4 characters), RRGGBB (6
	/// characters), or RRGGBBAA (8 characters).</param>
	/// <param name="color">When this method returns, contains the resulting <see cref="Color"/> if the conversion was successful;
	/// otherwise, the default value of <see cref="Color"/>.</param>
	/// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
	public static bool TryFromRgba(ReadOnlySpan<byte> hex, out Color color)
	{
		color = default;
		if (hex.Length == 0)
		{
			return false;
		}
		if (hex[0] == '#')
			hex = hex[1..];
		if (!Utf8Parser.TryParse(hex, out uint value, out _, 'X'))
			return false;
		switch (hex.Length)
		{
			case 3:
				color =
					0xFF000000 |
					(((value & 0xF00) * 0x11) << 8) |
					(((value & 0x0F0) * 0x11) << 4) |
					(((value & 0x00F) * 0x11));
				return true;
			case 4:
				color =
					(((value & 0xF000) * 0x11) << 4) |
					(((value & 0x0F00) * 0x11)) |
					(((value & 0x00F0) * 0x11) >> 4) |
					(((value & 0x000F) * 0x11) << 24);
				return true;
			case 6:
				color =
					0xFF000000 | value;
				return true;
			case 8:
				color =
					((value & 0x000000FF) << 24) |
					((value & 0xFFFFFF00) >> 8);
				return true;
			default:
				color = default;
				return false;
		}
	}
	/// <summary>
	/// Creates an Color instance from ARGB values.
	/// </summary>
	/// <param name="a">Alpha component</param>
	/// <param name="r">Red component</param>
	/// <param name="g">Green component</param>
	/// <param name="b">Blue component</param>
	/// <returns>Color instance</returns>
	public static Color FromArgb(byte a, byte r, byte g, byte b) => new((uint)(a << 24 | r << 16 | g << 8 | b));
	/// <summary>
	/// Attempts to create an <see cref="Color"/> instance from a hexadecimal RGBA color representation.
	/// </summary>
	/// <remarks>This method throws <see cref="ArgumentException"/> for invalid input. 
	/// The method supports both shorthand (e.g., RGB, ARGB) and
	/// full-length (e.g., RRGGBB, AARRGGBB) hexadecimal color representations.</remarks>
	/// <param name="hex">A string representing the hexadecimal ARGB color. The input can optionally start with a '#'
	/// character and must be in one of the following formats: RGB (3 characters), ARGB (4 characters), RRGGBB (6
	/// characters), or AARRGGBB (8 characters).</param>
	/// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
	public static Color FromArgb(ReadOnlySpan<byte> hex) => TryFromArgb(hex, out Color color) ? color : throw new ArgumentException("");
	/// <summary>
	/// Attempts to create an <see cref="Color"/> instance from a hexadecimal RGBA color representation.
	/// </summary>
	/// <remarks>This method throws <see cref="ArgumentException"/> for invalid input. 
	/// The method supports both shorthand (e.g., RGB, ARGB) and
	/// full-length (e.g., RRGGBB, AARRGGBB) hexadecimal color representations.</remarks>
	/// <param name="hex">A string representing the hexadecimal ARGB color. The input can optionally start with a '#'
	/// character and must be in one of the following formats: RGB (3 characters), ARGB (4 characters), RRGGBB (6
	/// characters), or AARRGGBB (8 characters).</param>
	/// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
	public static Color FromArgb(string hex) => TryFromArgb(hex, out Color color) ? color : throw new ArgumentException("");
	/// <summary>
	/// Creates an <see cref="Color"/> instance from a 32-bit ARGB value.
	/// </summary>
	/// <param name="hex">The 32-bit ARGB value.</param>
	/// <returns>A new <see cref="Color"/> instance.</returns>
	public static Color FromArgb(uint hex) => new(hex);
	/// <summary>
	/// Attempts to create an <see cref="Color"/> instance from a hexadecimal RGBA color representation.
	/// </summary>
	/// <remarks>This method does not throw exceptions for invalid input. Instead, it returns <see
	/// langword="false"/> if the input is not in a valid format. The method supports both shorthand (e.g., RGB, ARGB) and
	/// full-length (e.g., RRGGBB, AARRGGBB) hexadecimal color representations.</remarks>
	/// <param name="hex">A string representing the hexadecimal ARGB color. The input can optionally start with a '#'
	/// character and must be in one of the following formats: RGB (3 characters), ARGB (4 characters), RRGGBB (6
	/// characters), or AARRGGBB (8 characters).</param>
	/// <param name="color">When this method returns, contains the resulting <see cref="Color"/> if the conversion was successful;
	/// otherwise, the default value of <see cref="Color"/>.</param>
	/// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
	public static bool TryFromArgb(string hex, out Color color)
	{
		color = default;
		if (string.IsNullOrEmpty(hex))
			return false;
		hex = hex.Trim();
		if (hex.Length == 0)
		{
			color = default;
			return false;
		}
		if (hex[0] == '#')
			hex = hex[1..];
		if (!uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint value))
			return false;
		switch (hex.Length)
		{
			case 3:
				color =
					0xFF000000 |
					(((value & 0xF00) * 0x11) << 8) |
					(((value & 0x0F0) * 0x11) << 4) |
					(((value & 0x00F) * 0x11));
				return true;
			case 4:
				color =
					(((value & 0xF000) * 0x11) << 12) |
					(((value & 0x0F00) * 0x11) << 8) |
					(((value & 0x00F0) * 0x11) << 4) |
					(((value & 0x000F) * 0x11));
				return true;
			case 6:
				color =
					0xFF000000 | value;
				return true;
			case 8:
				color =
					value;
				return true;
			default:
				color = default;
				return false;
		}
	}
	/// <summary>
	/// Attempts to create an <see cref="Color"/> instance from a hexadecimal RGBA color representation.
	/// </summary>
	/// <remarks>This method does not throw exceptions for invalid input. Instead, it returns <see
	/// langword="false"/> if the input is not in a valid format. The method supports both shorthand (e.g., RGB, RGBA) and
	/// full-length (e.g., RRGGBB, AARRGGBB) hexadecimal color representations.</remarks>
	/// <param name="hex">A read-only span of bytes representing the hexadecimal ARGB color. The input can optionally start with a '#'
	/// character and must be in one of the following formats: RGB (3 characters), ARGB (4 characters), RRGGBB (6
	/// characters), or AARRGGBB (8 characters).</param>
	/// <param name="color">When this method returns, contains the resulting <see cref="Color"/> if the conversion was successful;
	/// otherwise, the default value of <see cref="Color"/>.</param>
	/// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
	public static bool TryFromArgb(ReadOnlySpan<byte> hex, out Color color)
	{
		color = default;
		if (hex.Length == 0)
		{
			return false;
		}
		if (hex[0] == '#')
			hex = hex[1..];
		if (!Utf8Parser.TryParse(hex, out uint value, out _, 'X'))
			return false;
		switch (hex.Length)
		{
			case 3:
				color =
					0xFF000000 |
					(((value & 0xF00) * 0x11) << 8) |
					(((value & 0x0F0) * 0x11) << 4) |
					(((value & 0x00F) * 0x11));
				return true;
			case 4:
				color =
					(((value & 0xF000) * 0x11) << 12) |
					(((value & 0x0F00) * 0x11) << 8) |
					(((value & 0x00F0) * 0x11) << 4) |
					(((value & 0x000F) * 0x11));
				return true;
			case 6:
				color =
					0xFF000000 | value;
				return true;
			case 8:
				color =
					value;
				return true;
			default:
				color = default;
				return false;
		}
	}
	/// <summary>
	/// Creates an Color object from the HSL color space.
	/// </summary>
	/// <param name="h">Hue (0-360)</param>
	/// <param name="s">Saturation (0-1)</param>
	/// <param name="l">Lightness (0-1)</param>
	/// <param name="a">Alpha (0-255)</param>
	/// <returns>Corresponding Color object</returns>
	public static Color FromHsl(float h, float s, float l, byte a = 255)
	{
		h /= 360;
		s /= 100;
		l /= 100;
		float r, g, b;
		if (s == 0)
		{
			r = g = b = l; // achromatic
		}
		else
		{
			static float hue2rgb(float p, float q, float t)
			{
				if (t < 0) t += 1;
				if (t > 1) t -= 1;
				if (t < 1 / 6.0) return p + (q - p) * 6 * t;
				if (t < 1 / 2.0) return q;
				return t < 2 / 3.0 ? p + (q - p) * (2 / 3.0f - t) * 6 : p;
			}
			float q = l < 0.5 ? l * (1 + s) : l + s - l * s;
			float p = 2 * l - q;
			r = hue2rgb(p, q, h + 1 / 3.0f);
			g = hue2rgb(p, q, h);
			b = hue2rgb(p, q, h - 1 / 3.0f);
		}
		return new Color((uint)(a << 24 | ((int)(r * 255) << 16) | (int)(g * 255) << 8 | (int)(b * 255)));
	}
	/// <summary>
	/// Creates an Color object from HSV values.
	/// </summary>
	/// <param name="h">Hue (0-360)</param>
	/// <param name="s">Saturation (0-1)</param>
	/// <param name="v">Value (0-1)</param>
	/// <param name="a">Alpha (0-255)</param>
	/// <returns>Returns an Color object.</returns>
	public static Color FromHsv(float h, float s, float v, byte a = 255)
	{
		h /= 360;
		s /= 100;
		v /= 100;
		int hi = (int)(h * 6);
		float f = h * 6 - hi;
		float p = v * (1 - s);
		float q = v * (1 - f * s);
		float t = v * (1 - (1 - f) * s);
		float r = 0, g = 0, b = 0;
		switch (hi % 6)
		{
			case 0: r = v; g = t; b = p; break;
			case 1: r = q; g = v; b = p; break;
			case 2: r = p; g = v; b = t; break;
			case 3: r = p; g = q; b = v; break;
			case 4: r = t; g = p; b = v; break;
			case 5: r = v; g = p; b = q; break;
		}
		return new Color((uint)(a << 24 | ((int)(r * 255) << 16) | (int)(g * 255) << 8 | (int)(b * 255)));
	}
	/// <summary>
	/// Linearly interpolates between two <see cref="Color"/> values based on a weighting factor.
	/// </summary>
	/// <remarks>This method interpolates each color channel (alpha, red, green, and blue)
	/// independently.</remarks>
	/// <param name="a">The starting color.</param>
	/// <param name="b">The ending color.</param>
	/// <param name="t">A value between 0 and 1 that specifies the interpolation factor.  If <paramref name="t"/> is 0, the result is
	/// <paramref name="a"/>.  If <paramref name="t"/> is 1, the result is <paramref name="b"/>.  Values outside the range
	/// [0, 1] are clamped to the nearest boundary.</param>
	/// <returns>A new <see cref="Color"/> that represents the interpolated color.</returns>
	public static Color Lerp(Color a, Color b, float t)
	{
		t = t < 0 ? 0 : t > 1 ? 1 : t;
		byte aA = a.A, aR = a.R, aG = a.G, aB = a.B;
		byte bA = b.A, bR = b.R, bG = b.G, bB = b.B;
		byte rA = (byte)(aA + (bA - aA) * t);
		byte rR = (byte)(aR + (bR - aR) * t);
		byte rG = (byte)(aG + (bG - aG) * t);
		byte rB = (byte)(aB + (bB - aB) * t);
		return FromArgb(rA, rR, rG, rB);
	}
	/// <inheritdoc/>
	public static bool operator ==(Color left, Color right) => left._color == right._color;
	/// <inheritdoc/>
	public static bool operator !=(Color left, Color right) => left._color != right._color;
	/// <summary>
	/// Defines an implicit conversion from a 32-bit unsigned integer to an Color instance.
	/// </summary>
	/// <remarks>This operator allows a uint value to be assigned directly to an Color variable without explicit
	/// casting. The interpretation of the uint value depends on the Color format, typically encoding color channels in
	/// ARGB or RGBA order.</remarks>
	/// <param name="color">The 32-bit unsigned integer representing the color value to convert.</param>
	public static implicit operator Color(uint color) => new(color);
	/// <summary>
	/// Converts an Color instance to its 32-bit unsigned integer representation.
	/// </summary>
	/// <remarks>This operator enables implicit conversion from Color to uint, allowing Color values to be
	/// used wherever a 32-bit unsigned integer is expected. The returned value represents the packed color value as
	/// defined by the Color structure.</remarks>
	/// <param name="color">The Color instance to convert.</param>
	public static implicit operator uint(Color color) => color._color;
	/// <inheritdoc/>
	public readonly override string ToString() => ToString("#AARRGGBB");
	private readonly string GetDebuggerDisplay() => ToString();
	/// <inheritdoc/>
	public readonly bool Equals(Color other) => _color == other._color;
	/// <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Color e && Equals(e);
	/// <inheritdoc/>
	public readonly override int GetHashCode() => _color.GetHashCode();
	/// <inheritdoc/>
	public readonly string ToString(string? format, IFormatProvider? formatProvider) =>
		(formatProvider ?? new ColorFormatInfo()).GetFormat(typeof(ICustomFormatter)) is ICustomFormatter formatter
			? formatter.Format(format, this, formatProvider)
			: ToString();
	/// <inheritdoc/>
	public readonly string ToString(string? format) => ToString(format, null);
}