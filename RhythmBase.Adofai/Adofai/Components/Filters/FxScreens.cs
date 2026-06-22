namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Screens</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxScreens : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxScreens;
	/// <summary>
	/// Gets or sets the value of the <b>Tiles</b>.
	/// </summary>
	[JsonAlias("Tiles")]
	public float Tiles { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>color</b>.
	/// </summary>
	public Color Color { get; set; }
}