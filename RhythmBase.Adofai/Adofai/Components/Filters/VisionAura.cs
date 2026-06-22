namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Vision Aura</b>.
/// </summary>
[JsonObjectSerializable]
public struct VisionAura : IFilter
{
	///<inheritdoc/>
	public readonly AdvancedFilter Type => AdvancedFilter.VisionAura;
	/// <summary>
	/// Gets or sets the value of the <b>Twist</b>.
	/// </summary>
	[JsonAlias("Twist")]
	public float Twist { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed</b>.
	/// </summary>
	[JsonAlias("Speed")]
	public float Speed { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Color</b>.
	/// </summary>
	[JsonAlias("Color")]
	public Color Color { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>PosX</b>.
	/// </summary>
	[JsonAlias("PosX")]
	public float PosX { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>PosY</b>.
	/// </summary>
	[JsonAlias("PosY")]
	public float PosY { get; set; }
}