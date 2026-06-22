namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors Adjust FullColors</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsAdjustFullColors : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorsAdjustFullColors;
	/// <summary>
	/// Gets or sets the value of the <b>Red_R</b>.
	/// </summary>
	[JsonAlias("Red_R")]
	public float RedR { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Green_G</b>.
	/// </summary>
	[JsonAlias("Green_G")]
	public float GreenG { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Blue_B</b>.
	/// </summary>
	[JsonAlias("Blue_B")]
	public float BlueB { get; set; }
}