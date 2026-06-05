namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera Exclusion</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraExclusion : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraExclusion;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}