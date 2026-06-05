namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Gradients Ansi</b>.
/// </summary>
[JsonObjectSerializable]
public struct GradientsAnsi : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.GradientsAnsi;
	/// <summary>
	/// Gets or sets the value of the <b>Switch</b>.
	/// </summary>
	[JsonAlias("Switch")]
	public float Switch { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade</b>.
	/// </summary>
	[JsonAlias("Fade")]
	public float Fade { get; set; }
}