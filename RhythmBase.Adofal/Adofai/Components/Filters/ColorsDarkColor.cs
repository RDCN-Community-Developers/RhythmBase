namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors DarkColor</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsDarkColor : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.ColorsDarkColor;
	/// <summary>
	/// Gets or sets the value of the <b>Alpha</b>.
	/// </summary>
	[JsonAlias("Alpha")]
	public float Alpha { get; set; }
}