namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>EyesVision 1</b>.
/// </summary>
[JsonObjectSerializable]
public struct EyesVision1 : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.EyesVision1;
	/// <summary>
	/// Gets or sets the value of the <b>_EyeWave</b>.
	/// </summary>
	[JsonAlias("_EyeWave")]
	public float EyeWave { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_EyeSpeed</b>.
	/// </summary>
	[JsonAlias("_EyeSpeed")]
	public float EyeSpeed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_EyeMove</b>.
	/// </summary>
	[JsonAlias("_EyeMove")]
	public float EyeMove { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_EyeBlink</b>.
	/// </summary>
	[JsonAlias("_EyeBlink")]
	public float EyeBlink { get; set; }
}