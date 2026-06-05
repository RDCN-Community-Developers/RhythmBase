namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera SoftLight</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraSoftLight : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraSoftLight;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}