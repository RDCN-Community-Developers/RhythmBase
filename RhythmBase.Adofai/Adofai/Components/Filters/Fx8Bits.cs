namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX 8bits</b>.
/// </summary>
[JsonObjectSerializable]
public struct Fx8Bits : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.Fx8Bits;
	/// <summary>
	/// Gets or sets the value of the <b>ResolutionX</b>.
	/// </summary>
	[JsonAlias("ResolutionX")]
	public int ResolutionX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>ResolutionY</b>.
	/// </summary>
	[JsonAlias("ResolutionY")]
	public int ResolutionY { get; set; }
}