namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Glitch2</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxGlitchTo : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.FxGlitchTo;
	/// <summary>
	/// Gets or sets the value of the <b>Glitch</b>.
	/// </summary>
	[JsonAlias("Glitch")]
	public float Glitch { get; set; }
}