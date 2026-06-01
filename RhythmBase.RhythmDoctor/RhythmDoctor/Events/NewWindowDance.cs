using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a new window dance event.
/// </summary>
[JsonObjectSerializable]
public record class NewWindowDance : BaseWindowEvent, IEaseEvent
{
	///<inheritdoc/>
	public override EventType Type { get; } = EventType.NewWindowDance;
	/// <summary>
	/// Gets the custom tab associated with this instance.
	/// </summary>
	/// <remarks>
	/// Set <see cref="CustomTab"/> to determine the tab of this event. The default value is Tab.Windows."/>
	/// </remarks>
	public override Tab Tab => CustomTab;
	/// <summary>
	/// Gets or sets the custom tab associated with this instance.
	/// </summary>
	/// <remarks>
	/// It can only be Tab.Actions or Tab.Windows. This property is used to determine the tab of this event. The default value is Tab.Windows.
	/// </remarks>
	[JsonCondition($"$&.{nameof(CustomTab)} is RhythmBase.RhythmDoctor.{nameof(Tab)}.{nameof(Tab.Windows)}")]
	[JsonAlias("tab")]
	public Tab CustomTab
	{
		get;
		set => field = value is Tab.Actions or Tab.Windows ? value : throw new InvalidOperationException();
	} = Tab.Windows;
	/// <summary>
	/// Gets or sets the preset.
	/// </summary>
	public WindowDancePreset Preset { get; set; } = WindowDancePreset.Move;
	/// <summary>
	/// Gets or sets the same preset behavior.
	/// </summary>
	[JsonCondition($"$&.{nameof(Preset)} is not RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Move)}")]
	public SamePresetBehavior SamePresetBehavior { get; set; } = SamePresetBehavior.Reset;
	/// <summary>
	/// Gets or sets the position.
	/// </summary>
	/// <remarks>
	/// Percentage of the screen width and height. (0,0) is the bottom-left corner, (100,100) is the top-right corner.
	/// </remarks>
	[Tween]
	public Point Position { get; set; } = new(50f, 50f);
	/// <summary>
	/// Gets or sets the reference.
	/// </summary>
	[JsonCondition($"$&.{nameof(Preset)} is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Move)}")]
	public WindowDanceReference Reference { get; set; } = WindowDanceReference.Center;
	/// <summary>
	/// Gets or sets a value indicating whether to use a circle.
	/// </summary>
	[JsonCondition($"$&.{nameof(Preset)} is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Ellipse)}")]
	public bool UseCircle { get; set; } = false;
	/// <summary>
	/// Gets or sets the speed.
	/// </summary>
	[Tween]
	[JsonCondition($"""
		$&.{nameof(Preset)}
			is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Wrap)}
			or RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Ellipse)}
			&&
		$&.{nameof(SamePresetBehavior)} is not RhythmBase.RhythmDoctor.{nameof(SamePresetBehavior)}.{nameof(SamePresetBehavior.Keep)}
		""")]
	public float Speed { get; set; } = 0;
	/// <summary>
	/// Gets or sets the amplitude.
	/// </summary>
	[Tween]
	[JsonCondition($"""
		(
		$&.{nameof(Preset)} is not RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Ellipse)} || $&.{nameof(UseCircle)}) &&
		$&.{nameof(Preset)} is not RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.ShakePer)} &&
		$&.{nameof(Preset)} is not RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Move)}
		""")]
	public float? Amplitude { get; set; } = 0f;
	/// <summary>
	/// Gets or sets the amplitude vector.
	/// </summary>
	[Tween]
	[JsonCondition($"""
		(
		$&.{nameof(Preset)} is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Ellipse)} && !$&.{nameof(UseCircle)} ||
		$&.{nameof(Preset)} is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.ShakePer)}) &&
		$&.{nameof(Preset)} is not RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Move)}
		""")]
	public Point AmplitudeVector { get; set; } = new(0f, 0f);
	/// <summary>
	/// Gets or sets the angle.
	/// </summary>
	[Tween]
	[JsonCondition($"""
		$&.{nameof(Preset)}
		is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Sway)}
		or RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Ellipse)} ||
		($&.{nameof(Preset)} is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Wrap)} &&
		$&.{nameof(SamePresetBehavior)} is not RhythmBase.RhythmDoctor.{nameof(SamePresetBehavior)}.{nameof(SamePresetBehavior.Keep)})
		""")]
	public float? Angle { get; set; } = 0f;
	/// <summary>
	/// Gets or sets the frequency.
	/// </summary>
	[Tween]
	[JsonCondition($"""
		$&.{nameof(Preset)}
		is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Sway)}
		or RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Wrap)}
		or RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.ShakePer)} &&
		$&.{nameof(SamePresetBehavior)} is not RhythmBase.RhythmDoctor.{nameof(SamePresetBehavior)}.{nameof(SamePresetBehavior.Keep)}
		""")]
	public float Frequency { get; set; } = 0f;
	/// <summary>
	/// Gets or sets the period.
	/// </summary>
	[Tween]
	[JsonCondition($"""
		$&.{nameof(Preset)} is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.ShakePer)} &&
		$&.{nameof(SamePresetBehavior)} is not RhythmBase.RhythmDoctor.{nameof(SamePresetBehavior)}.{nameof(SamePresetBehavior.Keep)}
		""")]
	public float Period { get; set; } = 0f;
	/// <summary>
	/// Gets or sets the ease type.
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(Preset)} is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Sway)} &&
		$&.{nameof(SamePresetBehavior)} is not RhythmBase.RhythmDoctor.{nameof(SamePresetBehavior)}.{nameof(SamePresetBehavior.Keep)}
		""")]
	public WindowDanceEaseType EaseType { get; set; } = WindowDanceEaseType.Repeat;
	/// <summary>
	/// Gets or sets the sub ease type.
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(Preset)}
			is RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.Sway)}
			or RhythmBase.RhythmDoctor.{nameof(WindowDancePreset)}.{nameof(WindowDancePreset.ShakePer)} &&
		$&.{nameof(SamePresetBehavior)} is not RhythmBase.RhythmDoctor.{nameof(SamePresetBehavior)}.{nameof(SamePresetBehavior.Keep)}
		""")]
	[JsonAlias("subEase")]
	public EaseType SubEaseType { get; set; } = Global.Components.Easing.EaseType.Linear;
	///<inheritdoc/>
	[JsonAlias("easingDuration")]
	public float Duration { get; set; } = 0;
	///<inheritdoc/>
	public EaseType Ease { get; set; } = Global.Components.Easing.EaseType.Linear;
}