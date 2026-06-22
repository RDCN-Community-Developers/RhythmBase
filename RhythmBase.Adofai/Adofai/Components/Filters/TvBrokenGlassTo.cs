namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>TV BrokenGlass2</b>.
/// </summary>
[JsonObjectSerializable]
public struct TvBrokenGlassTo : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.TvBrokenGlassTo;
	/// <summary>
	/// Gets or sets the value of the <b>Bullet_4</b>.
	/// </summary>
	[JsonAlias("Bullet_4")]
	public float Bullet4 { get; set; }
}