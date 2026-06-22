namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Vision Drost</b>.
/// </summary>
[JsonObjectSerializable]
public struct VisionDrost : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.VisionDrost;
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
}