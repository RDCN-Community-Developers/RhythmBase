namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur GaussianBlur</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurGaussianBlur : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlurGaussianBlur;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
}