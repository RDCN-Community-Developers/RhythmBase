namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Edge Sobel</b>.
/// </summary>
[JsonObjectSerializable]
public struct EdgeSobel : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.EdgeSobel;
}