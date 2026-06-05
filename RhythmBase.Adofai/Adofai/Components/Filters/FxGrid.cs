namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Grid</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxGrid : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FxGrid;
	/// <summary>
	/// Gets or sets the value of the <b>Distortion</b>.
	/// </summary>
	[JsonAlias("Distortion")]
	public float Distortion { get; set; }
}