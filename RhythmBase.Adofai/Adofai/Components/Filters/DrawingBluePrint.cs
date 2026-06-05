namespace RhythmBase.Adofai.Components.Filters;
/// <summary>
/// The filter of <b>Drawing BluePrint</b>.
/// </summary>
[JsonObjectSerializable]
public struct DrawingBluePrint : IFilter
{
	///<inheritdoc/>
	public readonly FilterType Type => FilterType.DrawingBluePrint;
	/// <summary>
	/// Gets or sets the value of the <b>Pencil_Color</b>.
	/// </summary>
	[JsonAlias("Pencil_Color")]
	public Color PencilColor { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Pencil_Size</b>.
	/// </summary>
	[JsonAlias("Pencil_Size")]
	public float PencilSize { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Pencil_Correction</b>.
	/// </summary>
	[JsonAlias("Pencil_Correction")]
	public float PencilCorrection { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Intensity</b>.
	/// </summary>
	[JsonAlias("Intensity")]
	public float Intensity { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Speed_Animation</b>.
	/// </summary>
	[JsonAlias("Speed_Animation")]
	public float SpeedAnimation { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Corner_Lose</b>.
	/// </summary>
	[JsonAlias("Corner_Lose")]
	public float CornerLose { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade_Paper_to_BackColor</b>.
	/// </summary>
	[JsonAlias("Fade_Paper_to_BackColor")]
	public float FadePaperToBackColor { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Fade_With_Original</b>.
	/// </summary>
	[JsonAlias("Fade_With_Original")]
	public float FadeWithOriginal { get; set; }
	/// <summary>
	/// Gets or sets the value of the <b>Back_Color</b>.
	/// </summary>
	[JsonAlias("Back_Color")]
	public Color BackColor { get; set; }
}