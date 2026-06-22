namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Distortion Aspiration</b>.
/// </summary>
[JsonObjectSerializable]
public struct DistortionAspiration : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DistortionAspiration;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>PosX</b>.
	/// </summary>
	[JsonAlias("PosX")]
	public float PosX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>PosY</b>.
	/// </summary>
	[JsonAlias("PosY")]
	public float PosY { get; set; }
}