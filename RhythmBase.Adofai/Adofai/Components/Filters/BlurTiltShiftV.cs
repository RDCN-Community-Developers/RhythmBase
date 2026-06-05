namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Blur Tilt Shift V</b>.
/// </summary>
[JsonObjectSerializable]
public struct BlurTiltShiftV : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.BlurTiltShiftV;
	/// <summary>
	/// Gets or sets the value of the <b>Amount</b>.
	/// </summary>
	[JsonAlias("Amount")]
	public float Amount { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>FastFilter</b>.
	/// </summary>
	[JsonAlias("FastFilter")]
	public int FastFilter { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Smooth</b>.
	/// </summary>
	[JsonAlias("Smooth")]
	public float Smooth { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Size</b>.
	/// </summary>
	[JsonAlias("Size")]
	public float Size { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Position</b>.
	/// </summary>
	[JsonAlias("Position")]
	public float Position { get; set; }
}