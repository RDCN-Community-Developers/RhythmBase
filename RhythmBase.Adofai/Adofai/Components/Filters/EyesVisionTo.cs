namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>EyesVision 2</b>.
/// </summary>
[JsonObjectSerializable]
public struct EyesVisionTo : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.EyesVisionTo;
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