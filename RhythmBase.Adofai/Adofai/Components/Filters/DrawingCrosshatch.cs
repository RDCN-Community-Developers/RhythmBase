namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing Crosshatch</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingCrosshatch : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.DrawingCrosshatch;
	/// <summary>
	/// Gets or sets the value of the <b>Width</b>.
	/// </summary>
	[JsonAlias("Width")]
	public float Width { get; set; }
}