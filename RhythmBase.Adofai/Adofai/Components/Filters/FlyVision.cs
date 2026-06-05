namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Fly Vision</b>.
/// </summary>
[JsonObjectSerializable]
public struct FlyVision : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FlyVision;
	/// <summary>
	/// Gets or sets the value of the <b>Zoom</b>.
	/// </summary>
	[JsonAlias("Zoom")]
	public float Zoom { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}