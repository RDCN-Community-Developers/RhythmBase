namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Focus</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurFocus : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlurFocus;
	/// <summary>
	/// Gets or sets the value of the <b>_Size</b>.
	/// </summary>
	[JsonAlias("_Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Eyes</b>.
	/// </summary>
	[JsonAlias("_Eyes")]
	public float Eyes { get; set; }
}