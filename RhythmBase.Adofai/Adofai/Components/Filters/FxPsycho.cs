namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Psycho</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxPsycho : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxPsycho;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}