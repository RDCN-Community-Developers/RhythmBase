namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Drunk2</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxDrunkTo : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxDrunkTo;
}