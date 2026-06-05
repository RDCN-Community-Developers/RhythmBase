namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Hexagon</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxHexagon : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FxHexagon;
}