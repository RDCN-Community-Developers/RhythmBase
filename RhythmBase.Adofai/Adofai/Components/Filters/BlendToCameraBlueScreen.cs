namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera BlueScreen</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraBlueScreen : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlendToCameraBlueScreen;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
}