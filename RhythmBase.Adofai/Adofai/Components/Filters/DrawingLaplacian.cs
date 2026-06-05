namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Laplacian</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingLaplacian : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DrawingLaplacian;
}