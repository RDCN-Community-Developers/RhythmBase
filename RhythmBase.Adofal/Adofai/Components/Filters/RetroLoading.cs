namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Retro Loading</b>.
/// </summary>
[JsonObjectSerializable]
public struct RetroLoading : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.RetroLoading;
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
}