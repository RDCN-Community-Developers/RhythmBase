namespace RhythmBase.RhythmDoctor.Components.Conditions;

/// <summary>
/// Represents a condition based on the game language.
/// </summary>
public class LanguageCondition : BaseConditional
{
	/// <summary>
	/// Gets or sets the game language.
	/// </summary>
	public Language TargetLanguage { get; set; }
	///<inheritdoc/>
	public override ConditionType Type => ConditionType.Language;
	/// <summary>
	/// Represents the supported game languages.
	/// </summary>
	[JsonEnumSerializable]
	public enum Language
	{
		/// <summary>
		/// English language.
		/// </summary>
		English,
		/// <summary>
		/// Spanish language.
		/// </summary>
		Spanish,
		/// <summary>
		/// Portuguese language.
		/// </summary>
		Portuguese,
		/// <summary>
		/// Simplified Chinese language.
		/// </summary>
		ChineseSimplified,
		/// <summary>
		/// Traditional Chinese language.
		/// </summary>
		ChineseTraditional,
		/// <summary>
		/// Korean language.
		/// </summary>
		Korean,
		/// <summary>
		/// Polish language.
		/// </summary>
		Polish,
		/// <summary>
		/// Japanese language.
		/// </summary>
		Japanese,
		/// <summary>
		/// German language.
		/// </summary>
		German
	}
}
