using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>AAA SuperHexagon</b>.
/// </summary>
[JsonObjectSerializable]
public struct AaaSuperHexagon : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.AaaSuperHexagon;
	/// <summary>
	/// Gets or sets the value of the <b>_AlphaHexa</b>.
	/// </summary>
	[JsonAlias("_AlphaHexa")]
	public float AlphaHexa { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>HexaSize</b>.
	/// </summary>
	[JsonAlias("HexaSize")]
	public float HexaSize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_BorderSize</b>.
	/// </summary>
	[JsonAlias("_BorderSize")]
	public float BorderSize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_BorderColor</b>.
	/// </summary>
	[JsonAlias("_BorderColor")]
	public Color BorderColor { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_HexaColor</b>.
	/// </summary>
	[JsonAlias("_HexaColor")]
	public Color HexaColor { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_SpotSize</b>.
	/// </summary>
	[JsonAlias("_SpotSize")]
	public float SpotSize { get; set; }
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