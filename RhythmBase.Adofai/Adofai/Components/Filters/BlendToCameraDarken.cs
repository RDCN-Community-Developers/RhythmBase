namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera Darken</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraDarken : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraDarken;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}