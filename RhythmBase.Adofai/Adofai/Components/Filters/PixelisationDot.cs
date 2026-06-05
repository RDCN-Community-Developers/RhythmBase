namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Pixelisation Dot</b>.
/// </summary>
[JsonObjectSerializable]
public struct PixelisationDot : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.PixelisationDot;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>LightBackGround</b>.
	/// </summary>
	[JsonAlias("LightBackGround")]
	public float LightBackGround { get; set; }
}