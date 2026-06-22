namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors Brightness</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsBrightness : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorsBrightness;
	/// <summary>
	/// Gets or sets the value of the <b>_Brightness</b>.
	/// </summary>
	[JsonAlias("_Brightness")]
	public float Brightness { get; set; }
}