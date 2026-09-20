using RhythmBase.RhythmDoctor.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.RhythmDoctor.Events
{
	/// <summary>
	/// Represents an event that sets the font for text in a rhythm doctor level.
	/// </summary>
	[JsonObjectSerializable]
	public record class SetFont : BaseDecorationAction, IFontFileEvent
	{
		/// <inheritdoc/>
		public override EventType Type => EventType.SetFont;
		/// <summary>
		/// Gets or sets the font to be used for text in the level. If the font is a custom font, it should be specified as a <see cref="FileReference"/>.
		/// If the font is a built-in font, it should be specified as a <see cref="FontName"/> value.
		/// </summary>
		public FontName Font { get; set; } = FontName.Default;
		/// <summary>
		/// Gets or sets the size of the font in points. A value of null indicates that the size is not specified.
		/// </summary>
		[JsonCondition($"$&.{nameof(Font)}.IsCustom")]
		public bool Bold { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the font should be italicized. This property is only applicable when using a custom font.
		/// </summary>
		[JsonCondition($"$&.{nameof(Font)}.IsCustom")]
		public bool Italic { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the font should be underlined. This property is only applicable when using a custom font.
		/// </summary>
		[JsonCondition($"$&.{nameof(Font)}.IsCustom")]
		public bool Underline { get; set; }
		/// <summary>
		/// Gets or sets the character spacing for the font. A value of 0 indicates that the character spacing is not specified. This property is only applicable when using a custom font.
		/// </summary>
		[JsonCondition($"$&.{nameof(Font)}.IsCustom")]
		public float CharacterSpacing { get; set; } = 0f;
		/// <summary>
		/// Gets or sets the outline width for the font. A value of 0 indicates that the outline width is not specified. This property is only applicable when using a custom font.
		/// </summary>
		[JsonCondition($"$&.{nameof(Font)}.IsCustom")]
		public float OutlineWidth { get; set; } = 0f;
		/// <summary>
		/// Gets or sets the wrapping width for the font. A value of 0 indicates that the wrapping width is not specified. This property is only applicable when using a custom font.
		/// </summary>
		public float WrappingWidth { get; set; } = 100f;
		IEnumerable<FileReference> IFontFileEvent.FontFiles => Font.IsCustom ? [Font.Value] : [];
		IEnumerable<FileReference> IFileEvent.Files => Font.IsCustom ? [Font.Value] : [];
	}
}
