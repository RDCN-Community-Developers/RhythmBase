namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera Luminosity</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraLuminosity : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraLuminosity;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}