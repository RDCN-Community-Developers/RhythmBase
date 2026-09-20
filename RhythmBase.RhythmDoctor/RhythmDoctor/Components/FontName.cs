using RhythmBase.Global.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// The font name.
/// Can be the preset font name or a custom font file name.
/// </summary>
public readonly struct FontName
{
	/// <summary>
	/// Built-in font names.
	/// </summary>
	[JsonEnumSerializable]
	public enum BuiltInFontType
	{
		/// <summary>
		/// Using the font that follows the game setting.
		/// </summary>
		Default,
		/// <summary>
		/// Using the pixel font <b>RDLatinFont</b>, which is suitable for pixel art style.
		/// </summary>
		Pixel,
		/// <summary>
		/// Using the font <b>Noto Sans CJK Bold</b>, which is a vector style font.
		/// </summary>
		Vector,
		/// <summary>
		/// Using the font <b>Futura</b>, which is a vector style font.
		/// </summary>
		Flash
	}
	/// <summary>
	/// Uses the default project font, typically optimized for general UI text.
	/// </summary>
	public static FontName Default => new(BuiltInFontType.Default);
	/// <summary>
	/// Renders text with pixel-perfect precision, ideal for retro aesthetics.
	/// </summary>
	public static FontName Pixel => new(BuiltInFontType.Pixel);
	/// <summary>
	/// Utilizes vector-based rendering to keep text crisp at any scale.
	/// </summary>
	public static FontName Vector => new(BuiltInFontType.Vector);
	/// <summary>
	/// Applies a Flash-inspired font style for legacy content compatibility.
	/// </summary>
	public static FontName Flash => new(BuiltInFontType.Flash);
	private readonly BuiltInFontType _type;
	private readonly FileReference? _fileReference;
	/// <summary>
	/// Indicates whether the font is a built-in type.
	/// </summary>
	[MemberNotNullWhen(true, nameof(_fileReference))]
	public readonly bool IsCustom { get; }
	/// <summary>
	/// The name of the font, either a built-in type or a custom file reference.
	/// </summary>
	public readonly string Value => IsCustom ? _fileReference : _type.ToEnumString();
	/// <summary>
	/// Creates a new instance of the <see cref="FontName"/> struct with the specified font name.
	/// </summary>
	/// <param name="fontName"></param>
	public FontName(string fontName)
	{
		if (EnumConverter.TryParse(fontName, out BuiltInFontType type))
		{
			_type = type;
			_fileReference = null;
			IsCustom = false;
		}
		else
		{
			_type = BuiltInFontType.Default;
			_fileReference = fontName;
			IsCustom = true;
		}
	}
	internal FontName(BuiltInFontType type)
	{
		_type = type;
		_fileReference = null;
		IsCustom = false;
	}
	/// <summary>
	/// Converts the string representation of a font name to its <see cref="FontName"/> equivalent.
	/// </summary>
	/// <param name="fontName"></param>
	public static implicit operator FontName(string fontName) => new(fontName);
}
