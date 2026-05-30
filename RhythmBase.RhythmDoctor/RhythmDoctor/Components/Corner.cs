using RhythmBase.Global.Components.Vector;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// The corner positions of the room. Percentage of the screen width and height. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
/// </summary>
public record struct Corner
{
    /// <summary>
    /// The left-top corner position of the room. Percentage of the screen width and height. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
    /// </summary>
    public Point? LeftTop { get; set; }
    /// <summary>
    /// The right-top corner position of the room. Percentage of the screen width and height. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
    /// </summary>
    public Point? RightTop { get; set; }
    /// <summary>
    /// The left-bottom corner position of the room. Percentage of the screen width and height. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
    /// </summary>
    public Point? LeftBottom { get; set; }
    /// <summary>
    /// The right-bottom corner position of the room. Percentage of the screen width and height. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
    /// </summary>
    public Point? RightBottom { get; set; }
    /// <summary>
    /// The default corner positions of the room.
    /// </summary>
    public static readonly Corner Default = new()
    {
        LeftBottom = (0, 0),
        RightBottom = (100, 0),
        LeftTop = (0, 100),
        RightTop = (100, 100)
    };
}
