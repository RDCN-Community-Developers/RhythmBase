namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors RgbClamp</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsRgbClamp : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.ColorsRgbClamp;
	/// <summary>
	/// Gets or sets the value of the <b>Red_End</b>.
	/// </summary>
	[JsonAlias("Red_End")]
	public float RedEnd { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Green_End</b>.
	/// </summary>
	[JsonAlias("Green_End")]
	public float GreenEnd { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Blue_End</b>.
	/// </summary>
	[JsonAlias("Blue_End")]
	public float BlueEnd { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>RGB_End</b>.
	/// </summary>
	[JsonAlias("RGB_End")]
	public float RgbEnd { get; set; }
}