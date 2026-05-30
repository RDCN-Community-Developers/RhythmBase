namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors Adjust ColorRGB</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsAdjustColorRGB : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.ColorsAdjustColorRGB;
}