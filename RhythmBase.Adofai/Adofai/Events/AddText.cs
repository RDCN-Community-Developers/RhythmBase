using RhythmBase.Global.Components.Vector;

namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event to add text in the game.  
/// </summary>  
[JsonObjectSerializable]
public class AddText : BaseTileEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.AddText;
	/// <summary>  
	/// Gets or sets the text to be displayed.  
	/// </summary>  
	[JsonAlias("decText")]
	public string DecorationText { get; set; } = string.Empty;
	/// <summary>  
	/// Gets or sets the font of the text.  
	/// </summary>  
	public string Font { get; set; } = string.Empty;
	/// <summary>  
	/// Gets or sets the position of the text.  
	/// </summary>  
	public PointN Position { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the relative position of the text to the camera.  
	/// </summary>  
	public CameraRelativeTo RelativeTo { get; set; } = CameraRelativeTo.Global;
	/// <summary>  
	/// Gets or sets the pivot offset of the text.  
	/// </summary>  
	public PointN PivotOffset { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the rotation of the text in degrees.  
	/// </summary>  
	public float Rotation { get; set; } = 0;
	/// <summary>  
	/// Gets or sets a value indicating whether the rotation of the text is locked.  
	/// </summary>  
	public bool LockRotation { get; set; } = false;
	/// <summary>  
	/// Gets or sets the scale of the text.  
	/// </summary>  
	public SizeN Scale { get; set; } = new(100, 100);
	/// <summary>  
	/// Gets or sets a value indicating whether the scale of the text is locked.  
	/// </summary>  
	public bool LockScale { get; set; } = false;
	/// <summary>  
	/// Gets or sets the color of the text.  
	/// </summary>  
	public Color Color { get; set; } = Color.White;
	/// <summary>  
	/// Gets or sets the opacity of the text.  
	/// </summary>  
	public float Opacity { get; set; } = 100f;
	/// <summary>  
	/// Gets or sets the depth of the text in the rendering order.  
	/// </summary>  
	public int Depth { get; set; } = -1;
	/// <summary>  
	/// Gets or sets the parallax effect of the text.  
	/// </summary>  
	public SizeN Parallax { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the parallax offset of the text.  
	/// </summary>  
	public SizeN ParallaxOffset { get; set; } = new(0, 0);
	/// <summary>  
	/// Gets or sets the tag associated with the text.  
	/// </summary>  
	public string Tag { get; set; } = string.Empty;
}
