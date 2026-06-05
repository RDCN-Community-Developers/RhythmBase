namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Oculus NightVision3</b>.
/// </summary>
[JsonObjectSerializable]
public struct OculusNightVision3 : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.OculusNightVision3;
	/// <summary>
	/// Gets or sets the value of the <b>Greenness</b>.
	/// </summary>
	[JsonAlias("Greenness")]
	public float Greenness { get; set; }
}