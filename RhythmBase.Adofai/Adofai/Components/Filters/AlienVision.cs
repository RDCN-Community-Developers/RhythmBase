namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Alien Vision</b>.
/// </summary>
[JsonObjectSerializable]
public struct AlienVision : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.AlienVision;
	/// <summary>
	/// Gets or sets the value of the <b>Therma_Variation</b>.
	/// </summary>
	[JsonAlias("Therma_Variation")]
	public float ThermaVariation { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
}