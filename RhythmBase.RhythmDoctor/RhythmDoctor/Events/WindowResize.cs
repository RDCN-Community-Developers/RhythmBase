using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>  
/// Represents a window resize event.  
/// </summary>
[JsonObjectSerializable]
public record class WindowResize : BaseWindowEvent, IEaseEvent, IRoomEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.WindowResize;
	/// <summary>
	/// Gets the custom tab associated with this instance.
	/// </summary>
	/// <remarks>
	/// Set <see cref="CustomTab"/> to determine the tab of this event. The default value is Tab.Windows."/>
	/// </remarks>
	public override Tab Tab => CustomTab;
	/// <summary>
	/// Gets or sets the custom tab.
	/// </summary>
	/// <remarks>
	/// It can only be Tab.Actions or Tab.Windows. This property is used to determine the tab of this event. The default value is Tab.Windows.
	/// </remarks>
	[JsonAlias("tab")]
	[JsonCondition($"""$&.{nameof(CustomTab)} is RhythmBase.RhythmDoctor.{nameof(RhythmBase.RhythmDoctor.Tab)}.{nameof(Tab.Windows)}""")]
	public Tab CustomTab
	{
		get;
		set => field = value is Tab.Actions or Tab.Windows ? value : throw new InvalidOperationException();
	} = Tab.Windows;
	/// <summary>  
	/// Gets or sets the scale of the window resize event.  
	/// </summary>  
	/// <remarks>
	/// Percentage of the original size. (100,100) is the original size. (0,0) will make the content disappear(or run into issues).
	/// Leave it null to keep the original size.
	/// </remarks>
	[Tween]
	public SizeE? Scale { get; set; }
	/// <summary>
	/// Gets or sets the zoom mode applied to the content.
	/// </summary>
	public ZoomMode ZoomMode { get; set; }
	/// <summary>
	/// Gets or sets the mode used for pivot operations.
	/// </summary>
	public PivotMode PivotMode { get; set; }
	/// <summary>
	/// Gets or sets the edge of the window to which the content is anchored.
	/// </summary>
	public WindowAnchorType AnchorType { get; set; }
	/// <summary>  
	/// Gets or sets the pivot point for the window resize event.  
	/// </summary>  
	/// <remarks>
	/// Percentage of the original size. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
	/// Leave it null to keep the original pivot.
	/// </remarks>
	[Tween]
	public Point? Pivot { get; set; }
	/// <inheritdoc/>
	public EaseType Ease { get; set; }
	/// <inheritdoc/>
	public float Duration { get; set; }
	/// <inheritdoc/>
	public Room Rooms { get; set; }
}