namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Mirror</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxMirror : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxMirror;
}