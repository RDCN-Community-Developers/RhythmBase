namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Colors HUE Rotate</b>.
/// </summary>
[JsonObjectSerializable]
public struct ColorsHueRotate : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.ColorsHueRotate;
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
}