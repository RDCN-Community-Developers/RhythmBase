namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Oculus NightVision2</b>.
/// </summary>
[JsonObjectSerializable]
public struct OculusNightVisionTo : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.OculusNightVisionTo;
	/// <summary>
	/// Gets or sets the value of the <b>FadeFX</b>.
	/// </summary>
	[JsonAlias("FadeFX")]
	public float FadeFX { get; set; }
}