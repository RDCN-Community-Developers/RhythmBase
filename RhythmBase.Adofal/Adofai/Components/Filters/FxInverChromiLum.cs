namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX InverChromiLum</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxInverChromiLum : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FxInverChromiLum;
}