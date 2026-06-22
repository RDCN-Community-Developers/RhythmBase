using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Noise</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurNoise : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlurNoise;
	/// <summary>
	/// Gets or sets the value of the <b>Level</b>.
	/// </summary>
	[JsonAlias("Level")]
	public int Level { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Distance</b>.
	/// </summary>
	[JsonAlias("Distance")]
	public PointN Distance { get; set; }
}