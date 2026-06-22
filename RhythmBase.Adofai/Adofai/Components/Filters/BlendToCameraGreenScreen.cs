namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera GreenScreen</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraGreenScreen : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlendToCameraGreenScreen;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}