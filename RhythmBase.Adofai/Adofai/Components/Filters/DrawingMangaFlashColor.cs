namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Manga Flash Color</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingMangaFlashColor : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DrawingMangaFlashColor;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Color</b>.
	/// </summary>
	[JsonAlias("Color")]
	public Color Color { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public int Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>PosX</b>.
	/// </summary>
	[JsonAlias("PosX")]
	public float PosX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>PosY</b>.
	/// </summary>
	[JsonAlias("PosY")]
	public float PosY { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
}