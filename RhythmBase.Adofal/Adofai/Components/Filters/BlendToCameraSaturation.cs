namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera Saturation</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraSaturation : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraSaturation;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}