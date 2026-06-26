using RhythmBase.BeatBlock.Components;
using RhythmBase.BeatBlock.Converters;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Decoration
/// </summary>
/// <remarks>
/// Draws a sprite on the screen, and allows updating its properties over time
/// </remarks>
[JsonObjectSerializable]
public record class Decoration : BaseEvent, IEaseEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.Decoration;
	/// <summary>
	/// Unique ID for the deco (if this ID does not exist, a deco will be created)
	/// </summary>
	public string Id { get; set; } = string.Empty;
	/// <summary>
	/// File name of sprite
	/// </summary>
	public string? Sprite { get; set; } = string.Empty;
	/// <summary>
	/// Unique ID for the parent deco (if this ID does not exist, the deco will not have a parent)
	/// </summary>
	public string? Parentid { get; set; } = string.Empty;
	/// <summary>
	/// How much the parent rotation influences the child deco
	/// </summary>
	public float? Rotationinfluence { get; set; }
	/// <summary>
	/// only rotate position, not angle
	/// </summary>
	public bool? Orbit { get; set; }
	/// <summary>
	/// Position
	/// </summary>
	[JsonFlatten(nameof(Point.X), "x")]
	[JsonFlatten(nameof(Point.Y), "y")]
	[JsonIgnore]
	public Point Position { get; set; }
	/// <summary>
	/// Rotation
	/// </summary>
	[JsonAlias("r")]
	public float? Rotation { get; set; }
	/// <summary>
	/// Scale
	/// </summary>
	[JsonFlatten(nameof(Size.Width), "sx")]
	[JsonFlatten(nameof(Size.Height), "sy")]
	[JsonIgnore]
	public Size Scale { get; set; }
	/// <summary>
	/// Offset
	/// </summary>
	[JsonFlatten(nameof(Point.X), "ox")]
	[JsonFlatten(nameof(Point.Y), "oy")]
	[JsonIgnore]
	public Point Offset { get; set; }
	/// <summary>
	/// Skew
	/// </summary>
	[JsonFlatten(nameof(Point.X), "kx")]
	[JsonFlatten(nameof(Point.Y), "ky")]
	[JsonIgnore]
	public Point Skew { get; set; }
	/// <summary>
	/// Mirror this deco?
	/// </summary>
	public MirrorType? Mirror { get; set; } = MirrorType.None;
	/// <summary>
	/// Only display the mirrored copies, and ignore the original?
	/// </summary>
	[JsonCondition($"$&.{nameof(Mirror)} != null")]
	public bool? ExclusiveMirror { get; set; }
	/// <summary>
	/// Layer to render on
	/// </summary>
	public Layer? DrawLayer { get; set; }
	/// <summary>
	/// Order in layer, lower is drawn first
	/// </summary>
	public int? DrawOrder { get; set; }
	/// <summary>
	/// Color to replace all non-alpha with (-1 to use original colors)
	/// </summary>
	public ColorIndexOrDefault Recolor { get; set; }
	/// <summary>
	/// Enable global outlining for this deco
	/// </summary>
	public bool? Outline { get; set; }
	/// <summary>
	/// Hide the deco
	/// </summary>
	public bool? Hide { get; set; }
	/// <summary>
	/// Allow this deco to tile
	/// </summary>
	public bool? Tiling { get; set; }
	///// <summary>
	///// How the tiling should repeat/extend, if at all
	///// </summary>
	//public TileRepeatMode TileRepeatMode { get; set; }
	/// <summary>
	/// UV Position
	/// </summary>
	[JsonCondition($"$&.{nameof(Tiling)} == true")]
	[JsonFlatten(nameof(Point.X), "uvx")]
	[JsonFlatten(nameof(Point.Y), "uvy")]
	[JsonIgnore]
	public Point UvPosition { get; set; }
	/// <summary>
	/// UV Delta X per second
	/// </summary>
	[JsonCondition($"$&.{nameof(Tiling)} == true")]
	[JsonFlatten(nameof(Point.X), "uvdx")]
	[JsonFlatten(nameof(Point.Y), "uvdy")]
	[JsonIgnore]
	public Point UvDelta { get; set; }
	/// <summary>
	/// Name of deco shader
	/// </summary>
	public string? Shader { get; set; } = string.Empty;
	/// <summary>
	/// Enable alpha dithering for this deco
	/// </summary>
	public bool? Alphadither { get; set; }
	/// <summary>
	/// Dither percent (0-1)
	/// </summary>
	public float? Ditherpercent { get; set; }
	/// <summary>
	/// Draw this deco on the effect canvas instead of the regular canvas
	/// </summary>
	[JsonCondition($"$&.{nameof(EffectCanvas)}")]
	public bool EffectCanvas { get; set; }
	/// <summary>
	/// Which effect canvas to draw on
	/// </summary>
	[JsonCondition($"$&.{nameof(EffectCanvas)}")]
	public EffectCanvasType EffectCanvasType { get; set; }
	/// <summary>
	/// Draw to the effect canvas without recoloring?
	/// </summary>
	[JsonCondition($"$&.{nameof(EffectCanvas)}")]
	public bool? EffectCanvasRaw { get; set; }
	/// <summary>
	/// Length of ease (IGNORED IF DECO IS JUST BEING CREATED)
	/// </summary>
	public float Duration { get; set; }
	/// <summary>
	/// Ease function to use (IGNORED IF DECO IS JUST BEING CREATED)
	/// </summary>
	public EaseType Ease { get; set; }
}
