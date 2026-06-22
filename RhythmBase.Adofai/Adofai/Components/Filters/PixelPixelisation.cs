namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Pixel Pixelisation</b>.
/// </summary>
[JsonObjectSerializable]
public struct PixelPixelisation : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.PixelPixelisation;
	/// <summary>
	/// Gets or sets the value of the <b>_Pixelisation</b>.
	/// </summary>
	[JsonAlias("_Pixelisation")]
	public float Pixelisation { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_SizeX</b>.
	/// </summary>
	[JsonAlias("_SizeX")]
	public float SizeX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_SizeY</b>.
	/// </summary>
	[JsonAlias("_SizeY")]
	public float SizeY { get; set; }
}