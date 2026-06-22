namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Color YUV</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorYuv : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorYuv;
}