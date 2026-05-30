namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera PhotoshopFilters</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraPhotoshopFilters : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraPhotoshopFilters;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}