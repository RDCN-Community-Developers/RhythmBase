namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX DigitalMatrix</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxDigitalMatrix : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxDigitalMatrix;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>ColorR</b>.
	/// </summary>
	[JsonAlias("ColorR")]
	public float ColorR { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>ColorG</b>.
	/// </summary>
	[JsonAlias("ColorG")]
	public float ColorG { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>ColorB</b>.
	/// </summary>
	[JsonAlias("ColorB")]
	public float ColorB { get; set; }
}