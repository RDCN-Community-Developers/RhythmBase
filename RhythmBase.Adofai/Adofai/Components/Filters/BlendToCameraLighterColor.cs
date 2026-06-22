namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera LighterColor</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraLighterColor : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlendToCameraLighterColor;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}