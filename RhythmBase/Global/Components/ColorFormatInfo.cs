namespace RhythmBase.Global.Components;

/// <summary>
/// Provides a custom format provider for Color.
/// </summary>
internal class ColorFormatInfo : IFormatProvider
{
	/// <inheritdoc/>
	public object? GetFormat(Type? formatType) => formatType == typeof(ICustomFormatter) ? new ColorFormatter() : (object?)null;
}
/// <summary>
/// Custom formatter for Color.
/// </summary>
internal class ColorFormatter : ICustomFormatter, IFormatProvider
{
	/// <inheritdoc/>
	public string Format(string? format, object? arg, IFormatProvider? formatProvider)
	{
		return arg is Color color
			? format switch
			{
				"RRGGBB" => $"{color.R:X2}{color.G:X2}{color.B:X2}",
				"RRGGBBAA" => $"{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}",
				"AARRGGBB" => $"{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}",
				"#RRGGBB" => $"#{color.R:X2}{color.G:X2}{color.B:X2}",
				"#RRGGBBAA" => $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}",
				"#AARRGGBB" => $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}",
				"rrggbb" => $"{color.R:x2}{color.G:x2}{color.B:x2}",
				"rrggbbaa" => $"{color.R:x2}{color.G:x2}{color.B:x2}{color.A:x2}",
				"aarrggbb" => $"{color.A:x2}{color.R:x2}{color.G:x2}{color.B:x2}",
				"#rrggbb" => $"#{color.R:x2}{color.G:x2}{color.B:x2}",
				"#rrggbbaa" => $"#{color.R:x2}{color.G:x2}{color.B:x2}{color.A:x2}",
				"#aarrggbb" => $"#{color.A:x2}{color.R:x2}{color.G:x2}{color.B:x2}",
				"R,G,B" or "r,g,b" => $"{color.R},{color.G},{color.B}",
				"R,G,B,A" or "r,g,b,a" => $"{color.R},{color.G},{color.B},{color.A}",
				"A,R,G,B" or "a,r,g,b" => $"{color.A},{color.R},{color.G},{color.B}",
				"RGB" or "rgb" => $"R:{color.R},G:{color.G},B:{color.B}",
				"RGBA" or "rgba" => $"R:{color.R},G:{color.G},B:{color.B},A:{color.A}",
				"ARGB" or "argb" or _ => $"A:{color.A},R:{color.R},G:{color.G},B:{color.B}",
			}
			: arg?.ToString() ?? string.Empty;
	}
	/// <inheritdoc/>
	public object? GetFormat(Type? formatType) => formatType == typeof(ICustomFormatter) ? this : (object?)null;
}
