namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera Color</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraColor : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraColor;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}