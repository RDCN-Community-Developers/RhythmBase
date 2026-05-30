namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Oculus NightVision5</b>.
/// </summary>
[JsonObjectSerializable]
public struct OculusNightVision5 : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.OculusNightVision5;
	/// <summary>
	/// Gets or sets the value of the <b>FadeFX</b>.
	/// </summary>
	[JsonAlias("FadeFX")]
	public float FadeFX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Size</b>.
	/// </summary>
	[JsonAlias("_Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Smooth</b>.
	/// </summary>
	[JsonAlias("_Smooth")]
	public float Smooth { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Dist</b>.
	/// </summary>
	[JsonAlias("_Dist")]
	public float Dist { get; set; }
}