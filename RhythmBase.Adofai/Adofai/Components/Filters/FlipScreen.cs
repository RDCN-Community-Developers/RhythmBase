namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FlipScreen</b>.
/// </summary>
[JsonObjectSerializable]
public struct FlipScreen : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FlipScreen;
}