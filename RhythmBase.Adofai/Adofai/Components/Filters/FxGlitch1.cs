namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Glitch1</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxGlitch1 : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxGlitch1;
	/// <summary>
	/// Gets or sets the value of the <b>Glitch</b>.
	/// </summary>
	[JsonAlias("Glitch")]
	public float Glitch { get; set; }
}