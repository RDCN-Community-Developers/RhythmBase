using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Dithering2x2</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurDitheringToxTo : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlurDitheringToxTo;
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