namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Plasma</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxPlasma : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxPlasma;
}