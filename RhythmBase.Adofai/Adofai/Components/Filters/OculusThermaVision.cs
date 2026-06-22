namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Oculus ThermaVision</b>.
/// </summary>
[JsonObjectSerializable]
public struct OculusThermaVision : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.OculusThermaVision;
	/// <summary>
	/// Gets or sets the value of the <b>Therma_Variation</b>.
	/// </summary>
	[JsonAlias("Therma_Variation")]
	public float ThermaVariation { get; set; }
}