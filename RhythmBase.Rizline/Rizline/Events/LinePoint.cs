using RhythmBase.Global.Components.Easing;

namespace RhythmBase.Rizline.Events;

/// <summary>
/// A point on a guide line that defines its position, color and interpolation to the next point.
/// </summary>
[JsonObjectSerializable]
public record class LinePoint : BaseEvent, IFloorPositionEvent
{
    public override EventType Type => EventType.LinePoint;
    /// <summary>
    /// Time of the line point in ticks. 
    /// </summary>
    [JsonAlias("time")]
    public float Duration { get; set; }

    /// <summary>
    /// X (horizontal) position of the line point. 
    /// </summary>
    public float XPosition { get; set; }

    /// <summary>
    /// Color at this line point. Interpolated between adjacent points. 
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Ease type used when interpolating from this point to the next. 
    /// </summary>
    [JsonAlias("easeType")]
    public EaseType Ease { get; set; }
    /// <summary>
    /// Canvas (track) index where this point resides. 
    /// </summary>
    public int CanvasIndex { get; set; }
    /// <summary>
    /// Cumulative floor position (vertical offset) for this point in its canvas.
    /// </summary>
    public float FloorPosition { get; set; }
}
