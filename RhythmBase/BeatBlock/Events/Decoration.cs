using RhythmBase.BeatBlock.Components;
using RhythmBase.Global.Components.Easing;
namespace RhythmBase.BeatBlock.Events;

/// <summary>
/// Decoration
/// </summary>
/// <remarks>
/// Draws a sprite on the screen, and allows updating its properties over time
/// </remarks>
public record class Decoration : BaseEvent , IEaseEvent
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
    /// X position
    /// </summary>
    public float? X { get; set; }
    /// <summary>
    /// Y position
    /// </summary>
    public float? Y { get; set; }
    /// <summary>
    /// Rotation
    /// </summary>
    public float? R { get; set; }
    /// <summary>
    /// X scale
    /// </summary>
    public float? Sx { get; set; }
    /// <summary>
    /// Y scale
    /// </summary>
    public float? Sy { get; set; }
    /// <summary>
    /// X offset
    /// </summary>
    public float? Ox { get; set; }
    /// <summary>
    /// Y offset
    /// </summary>
    public float? Oy { get; set; }
    /// <summary>
    /// X skew
    /// </summary>
    public float? Kx { get; set; }
    /// <summary>
    /// Y skew
    /// </summary>
    public float? Ky { get; set; }
    /// <summary>
    /// Mirror this deco?
    /// </summary>
    public MirrorType Mirror { get; set; } = MirrorType.None;
    /// <summary>
    /// Only display the mirrored copies, and ignore the original?
    /// </summary>
    [RDJsonCondition($"{nameof(Mirror)} != null")]
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
    public ColorIndex? Recolor { get; set; }
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
    /// UV X position
    /// </summary>
    [RDJsonCondition($"{nameof(Tiling)} == true")]
    public float? Uvx { get; set; }
    /// <summary>
    /// UV Y position
    /// </summary>
    [RDJsonCondition($"{nameof(Tiling)} == true")]
    public float? Uvy { get; set; }
    /// <summary>
    /// UV Delta X per second
    /// </summary>
    [RDJsonCondition($"{nameof(Tiling)} == true")]
    public float? Uvdx { get; set; }
    /// <summary>
    /// UV Delta Y per second
    /// </summary>
    [RDJsonCondition($"{nameof(Tiling)} == true")]
    public float? Uvdy { get; set; }
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
    [RDJsonCondition($"{nameof(EffectCanvas)}")]
    public bool EffectCanvas { get; set; }
    /// <summary>
    /// Which effect canvas to draw on
    /// </summary>
    [RDJsonCondition($"{nameof(EffectCanvas)}")]
    public EffectCanvasType EffectCanvasType { get; set; }
    /// <summary>
    /// Draw to the effect canvas without recoloring?
    /// </summary>
    [RDJsonCondition($"{nameof(EffectCanvas)}")]
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
