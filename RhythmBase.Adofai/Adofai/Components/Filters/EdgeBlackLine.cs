namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Edge BlackLine</b>.
/// </summary>
[JsonObjectSerializable]
public struct EdgeBlackLine : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.EdgeBlackLine;
}