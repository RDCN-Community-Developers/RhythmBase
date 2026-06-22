namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV BrokenGlass</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvBrokenGlass : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvBrokenGlass;
	/// <summary>
	/// Gets or sets the value of the <b>Broken_Big</b>.
	/// </summary>
	[JsonAlias("Broken_Big")]
	public float BrokenBig { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>LightReflect</b>.
	/// </summary>
	[JsonAlias("LightReflect")]
	public float LightReflect { get; set; }
}