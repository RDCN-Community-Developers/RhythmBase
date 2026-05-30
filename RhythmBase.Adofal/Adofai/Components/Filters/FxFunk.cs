namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Funk</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxFunk : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FxFunk;
}