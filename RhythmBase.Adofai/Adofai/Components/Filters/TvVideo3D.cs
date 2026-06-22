namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Video3D</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvVideo3D : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvVideo3D;
}