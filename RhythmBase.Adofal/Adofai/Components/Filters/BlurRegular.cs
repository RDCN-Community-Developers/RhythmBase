using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Regular</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurRegular : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlurRegular;
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