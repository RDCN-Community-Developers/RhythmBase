namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera LinearBurn</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraLinearBurn : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraLinearBurn;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}