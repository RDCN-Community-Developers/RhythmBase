namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Special Bubble</b>.
/// </summary>
[JsonObjectSerializable]
public struct SpecialBubble : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.SpecialBubble;
	/// <summary>
	/// Gets or sets the value of the <b>X</b>.
	/// </summary>
	[JsonAlias("X")]
	public float X { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Y</b>.
	/// </summary>
	[JsonAlias("Y")]
	public float Y { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Rate</b>.
	/// </summary>
	[JsonAlias("Rate")]
	public float Rate { get; set; }
}