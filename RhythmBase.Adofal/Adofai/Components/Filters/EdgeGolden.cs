namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Edge Golden</b>.
/// </summary>
[JsonObjectSerializable]
public struct EdgeGolden : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.EdgeGolden;
}