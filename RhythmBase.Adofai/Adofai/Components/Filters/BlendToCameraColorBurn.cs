namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera ColorBurn</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraColorBurn : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraColorBurn;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}