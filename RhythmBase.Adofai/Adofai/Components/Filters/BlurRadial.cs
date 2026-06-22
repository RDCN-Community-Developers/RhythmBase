namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Radial</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurRadial : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlurRadial;
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>MovX</b>.
	/// </summary>
	[JsonAlias("MovX")]
	public float MovX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>MovY</b>.
	/// </summary>
	[JsonAlias("MovY")]
	public float MovY { get; set; }
}