namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>FX Glitch3</b>.
/// </summary>
[JsonObjectSerializable]
public struct FxGlitch3 : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.FxGlitch3;
	/// <summary>
	/// Gets or sets the value of the <b>_Glitch</b>.
	/// </summary>
	[JsonAlias("_Glitch")]
	public float Glitch { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>_Noise</b>.
	/// </summary>
	[JsonAlias("_Noise")]
	public float Noise { get; set; }
}