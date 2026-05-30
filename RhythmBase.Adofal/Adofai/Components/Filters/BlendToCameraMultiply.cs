namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera Multiply</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraMultiply : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraMultiply;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}