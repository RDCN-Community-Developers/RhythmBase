namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Color Invert</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorInvert : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.ColorInvert;
	/// <summary>
	/// Gets or sets the value of the <b>_Fade</b>.
	/// </summary>
	[JsonAlias("_Fade")]
	public float Fade { get; set; }
}