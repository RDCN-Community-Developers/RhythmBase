namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Color Chromatic Aberration</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorChromaticAberration : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.ColorChromaticAberration;
	/// <summary>
	/// Gets or sets the value of the <b>Offset</b>.
	/// </summary>
	[JsonAlias("Offset")]
	public float Offset { get; set; }
}