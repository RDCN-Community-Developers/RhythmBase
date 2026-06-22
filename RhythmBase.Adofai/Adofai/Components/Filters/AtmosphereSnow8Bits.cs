namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Atmosphere Snow 8bits</b>.
/// </summary>
[JsonObjectSerializable]
public struct AtmosphereSnow8Bits : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.AtmosphereSnow8Bits;
	/// <summary>
	/// Gets or sets the value of the <b>Threshold</b>.
	/// </summary>
	[JsonAlias("Threshold")]
	public float Threshold { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}