namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Vision Rainbow</b>.
/// </summary>
[JsonObjectSerializable]
public struct VisionRainbow : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.VisionRainbow;
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
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
	/// Gets or sets the value of the <b>Colors</b>.
	/// </summary>
	[JsonAlias("Colors")]
	public float Colors { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Vision</b>.
	/// </summary>
	[JsonAlias("Vision")]
	public float Vision { get; set; }
}