namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera HardMix</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraHardMix : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraHardMix;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}