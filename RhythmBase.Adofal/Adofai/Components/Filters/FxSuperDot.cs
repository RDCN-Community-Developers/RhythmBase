namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX superDot</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxSuperDot : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FxSuperDot;
}