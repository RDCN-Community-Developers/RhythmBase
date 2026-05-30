namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Real VHS</b>.
/// </summary>
[JsonObjectSerializable]
public struct RealVhs : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.RealVhs;
	/// <summary>
	/// Gets or sets the value of the <b>TRACKING</b>.
	/// </summary>
	[JsonAlias("TRACKING")]
	public float Tracking { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>JITTER</b>.
	/// </summary>
	[JsonAlias("JITTER")]
	public float Jitter { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>GLITCH</b>.
	/// </summary>
	[JsonAlias("GLITCH")]
	public float Glitch { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>NOISE</b>.
	/// </summary>
	[JsonAlias("NOISE")]
	public float Noise { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Constrast</b>.
	/// </summary>
	[JsonAlias("Constrast")]
	public float Constrast { get; set; }
}