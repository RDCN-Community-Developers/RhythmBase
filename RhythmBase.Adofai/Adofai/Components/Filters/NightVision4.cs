namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>NightVision 4</b>.
/// </summary>
[JsonObjectSerializable]
public struct NightVision4 : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.NightVision4;
	/// <summary>
	/// Gets or sets the value of the <b>FadeFX</b>.
	/// </summary>
	[JsonAlias("FadeFX")]
	public float FadeFX { get; set; }
}