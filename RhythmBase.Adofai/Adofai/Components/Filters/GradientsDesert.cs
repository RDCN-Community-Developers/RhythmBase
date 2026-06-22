namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Gradients Desert</b>.
/// </summary>
[JsonObjectSerializable]
public struct GradientsDesert : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.GradientsDesert;
	/// <summary>
	/// Gets or sets the value of the <b>Switch</b>.
	/// </summary>
	[JsonAlias("Switch")]
	public float Switch { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}