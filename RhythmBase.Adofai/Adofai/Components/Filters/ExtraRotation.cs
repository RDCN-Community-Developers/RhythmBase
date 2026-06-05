namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>EXTRA Rotation</b>.
/// </summary>
[JsonObjectSerializable]
public struct ExtraRotation : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.ExtraRotation;
	/// <summary>
	/// Gets or sets the value of the <b>PositionX</b>.
	/// </summary>
	[JsonAlias("PositionX")]
	public float PositionX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>PositionY</b>.
	/// </summary>
	[JsonAlias("PositionY")]
	public float PositionY { get; set; }
}