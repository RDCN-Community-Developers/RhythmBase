namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera DarkerColor</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraDarkerColor : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraDarkerColor;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}