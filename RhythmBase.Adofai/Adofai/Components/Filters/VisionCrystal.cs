namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Vision Crystal</b>.
/// </summary>
[JsonObjectSerializable]
public struct VisionCrystal : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.VisionCrystal;
	/// <summary>
	/// Gets or sets the value of the <b>Value</b>.
	/// </summary>
	[JsonAlias("Value")]
	public float Value { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>X</b>.
	/// </summary>
	[JsonAlias("X")]
	public float X { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Y</b>.
	/// </summary>
	[JsonAlias("Y")]
	public float Y { get; set; }
}