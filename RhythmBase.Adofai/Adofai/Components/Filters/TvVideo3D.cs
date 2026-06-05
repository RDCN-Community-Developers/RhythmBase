namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV Video3D</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvVideo3D : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.TvVideo3D;
}