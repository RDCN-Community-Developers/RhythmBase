namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors Adjust PreFilters</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsAdjustPreFilters : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorsAdjustPreFilters;
	/// <summary>
	/// Gets or sets the value of the <b>FadeFX</b>.
	/// </summary>
	[JsonAlias("FadeFX")]
	public float FadeFX { get; set; }
}