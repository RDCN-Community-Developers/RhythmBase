namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera LinearLight</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraLinearLight : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraLinearLight;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}