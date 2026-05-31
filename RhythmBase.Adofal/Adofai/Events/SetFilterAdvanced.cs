using RhythmBase.Adofai.Components.Filters;
using RhythmBase.Adofai.Converters;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Events;

namespace RhythmBase.Adofai.Events;

/// <summary>
/// Represents an advanced filter event in the Adofai event system.
/// </summary>
[JsonObjectHasSerializer(typeof(EventMemberConverterSetFilterAdvanced))]
public class SetFilterAdvanced : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
#pragma warning disable CS8618
	/// <inheritdoc/>
	public override EventType Type => EventType.SetFilterAdvanced;
	/// <summary>
	/// Gets or sets a value indicating whether the filter is enabled.
	/// </summary>
	public bool Enabled { get; set; } = true;
	/// <summary>
	/// Gets or sets a value indicating whether other filters should be disabled.
	/// </summary>
	public bool DisableOthers { get; set; }
	/// <summary>
	/// Gets or sets the duration of the filter effect in seconds.
	/// </summary>
	public float Duration { get; set; }
	/// <summary>
	/// Gets or sets the easing type for the filter effect.
	/// </summary>
	public EaseType Ease { get; set; }
	/// <summary>
	/// Gets or sets the target type for the filter effect.
	/// </summary>
	public TargetType TargetType { get; set; }
	/// <summary>
	/// Gets or sets the plane (background or foreground) where the filter is applied.
	/// </summary>
	public Plane Plane { get; set; } = Plane.Background;
	/// <summary>
	/// Gets or sets the target tag for the filter effect.
	/// </summary>
	public string TargetTag { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the properties of the filter as a JSON object.
	/// </summary>
	public IFilter FilterProperties { get; set; } = new AaaSuperComputer();
}