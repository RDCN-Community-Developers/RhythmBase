namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Color Switching</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorSwitching : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorSwitching;
	/// <summary>
	/// Gets or sets the value of the <b>Color</b>.
	/// </summary>
	[JsonAlias("Color")]
	public int Color { get; set; }
}