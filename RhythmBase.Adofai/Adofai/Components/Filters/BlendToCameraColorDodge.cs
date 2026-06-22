namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera ColorDodge</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraColorDodge : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlendToCameraColorDodge;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}