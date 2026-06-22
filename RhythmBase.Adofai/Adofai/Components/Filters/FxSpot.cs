using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Spot</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxSpot : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxSpot;
	/// <summary>
	/// Gets or sets the value of the <b>center</b>.
	/// </summary>
	public PointN Center { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Radius</b>.
	/// </summary>
	[JsonAlias("Radius")]
	public float Radius { get; set; }
}