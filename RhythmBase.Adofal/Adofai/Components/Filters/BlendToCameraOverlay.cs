namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera Overlay</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraOverlay : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlendToCameraOverlay;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}