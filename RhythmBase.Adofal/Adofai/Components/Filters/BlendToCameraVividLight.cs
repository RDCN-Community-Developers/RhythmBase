namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera VividLight</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraVividLight : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraVividLight;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}