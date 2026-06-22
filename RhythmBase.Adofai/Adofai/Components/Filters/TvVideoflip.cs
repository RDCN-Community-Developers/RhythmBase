namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Videoflip</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvVideoflip : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvVideoflip;
}