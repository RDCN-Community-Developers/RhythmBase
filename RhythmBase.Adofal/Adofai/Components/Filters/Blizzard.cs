namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blizzard</b>.
/// </summary>
[JsonObjectSerializable]
public struct Blizzard : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.Blizzard;
	/// <summary>
	/// Gets or sets the value of the <b>_Speed</b>.
	/// </summary>
	[JsonAlias("_Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Size</b>.
	/// </summary>
	[JsonAlias("_Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Fade</b>.
	/// </summary>
	[JsonAlias("_Fade")]
	public float Fade { get; set; }
}