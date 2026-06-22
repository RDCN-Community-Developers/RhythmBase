namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX 8bits gb</b>.
/// </summary>
[JsonObjectSerializable]
public struct Fx8BitsGb : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.Fx8BitsGb;
}