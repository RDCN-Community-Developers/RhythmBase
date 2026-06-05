namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera Lighten</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraLighten : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraLighten;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}