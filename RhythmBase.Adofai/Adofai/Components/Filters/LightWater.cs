namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Light Water</b>.
/// </summary>
[JsonObjectSerializable]
public struct LightWater : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.LightWater;
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Alpha</b>.
	/// </summary>
	[JsonAlias("Alpha")]
	public float Alpha { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Distance</b>.
	/// </summary>
	[JsonAlias("Distance")]
	public float Distance { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
}