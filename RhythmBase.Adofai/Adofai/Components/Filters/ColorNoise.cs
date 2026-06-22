namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Color Noise</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorNoise : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorNoise;
	/// <summary>
	/// Gets or sets the value of the <b>Noise</b>.
	/// </summary>
	[JsonAlias("Noise")]
	public float Noise { get; set; }
}