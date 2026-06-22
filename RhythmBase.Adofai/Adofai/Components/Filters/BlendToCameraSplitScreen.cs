namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blend2Camera SplitScreen</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlendToCameraSplitScreen : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.BlendToCameraSplitScreen;
	/// <summary>
	/// Gets or sets the value of the <b>BlendFX</b>.
	/// </summary>
	[JsonAlias("BlendFX")]
	public float BlendFX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>SplitX</b>.
	/// </summary>
	[JsonAlias("SplitX")]
	public float SplitX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>SplitY</b>.
	/// </summary>
	[JsonAlias("SplitY")]
	public float SplitY { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Smooth</b>.
	/// </summary>
	[JsonAlias("Smooth")]
	public float Smooth { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Rotation</b>.
	/// </summary>
	[JsonAlias("Rotation")]
	public float Rotation { get; set; }
}