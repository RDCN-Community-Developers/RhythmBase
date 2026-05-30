using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>AAA SuperComputer</b>.
/// </summary>
[JsonObjectSerializable]
public struct AaaSuperComputer : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.AaaSuperComputer;
	/// <summary>
	/// Gets or sets the value of the <b>_AlphaHexa</b>.
	/// </summary>
	[JsonAlias("_AlphaHexa")]
	public float AlphaHexa { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>ShapeFormula</b>.
	/// </summary>
	[JsonAlias("ShapeFormula")]
	public float ShapeFormula { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Shape</b>.
	/// </summary>
	[JsonAlias("Shape")]
	public float Shape { get; set; }
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