namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera Hue</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraHue : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlendToCameraHue;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}