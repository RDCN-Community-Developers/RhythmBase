namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX DarkMatter</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxDarkMatter : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FxDarkMatter;
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
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
	/// Gets or sets the value of the <b>Zoom</b>.
	/// </summary>
	[JsonAlias("Zoom")]
	public float Zoom { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>DarkIntensity</b>.
	/// </summary>
	[JsonAlias("DarkIntensity")]
	public float DarkIntensity { get; set; }
}