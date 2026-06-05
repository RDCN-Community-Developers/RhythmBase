namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Vision Psycho</b>.
/// </summary>
[JsonObjectSerializable]
public struct VisionPsycho : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.VisionPsycho;
	/// <summary>
	/// Gets or sets the value of the <b>HoleSize</b>.
	/// </summary>
	[JsonAlias("HoleSize")]
	public float HoleSize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>HoleSmooth</b>.
	/// </summary>
	[JsonAlias("HoleSmooth")]
	public float HoleSmooth { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Color1</b>.
	/// </summary>
	[JsonAlias("Color1")]
	public float Color1 { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Color2</b>.
	/// </summary>
	[JsonAlias("Color2")]
	public float Color2 { get; set; }
}