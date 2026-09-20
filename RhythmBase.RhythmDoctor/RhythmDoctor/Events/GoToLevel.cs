using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that transitions to another level in a rhythm doctor level.
/// </summary>
[JsonObjectHasSerializer(typeof(Serialization.RDMemberConverter.GoToLevel))]
public record class GoToLevel : BaseEvent, IChartFileEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.GoToLevel;
	/// <inheritdoc/>
	public override Tab Tab => Tab.Actions;
	/// <summary>
	/// Gets or sets the action to perform when transitioning to the next level.
	/// </summary>
	public GoToLevelAction Action { get; set; }
	/// <summary>
	/// Gets or sets the reference to the chart file to transition to.
	/// </summary>
	public FileReference Chart { get; set; }
	/// <summary>
	/// Gets the resolved level.
	/// </summary>
	public RhythmDoctor.Components.Chart? ResolvedLevel { get; internal set; }
	/// <summary>
	/// Gets or sets a value indicating whether the transition to the next level is skippable.
	/// </summary>
	public bool Skippable { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the transition to the next level should fade out the current level.
	/// </summary>
	public bool FadeOut { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the transition to the next level should start immediately.
	/// </summary>
	public bool StartImmediately { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether mistakes should be kept when transitioning to the next level.
	/// </summary>
	public bool KeepMistakes { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the restart should not be updated when transitioning to the next level.
	/// </summary>
	public bool DontUpdateRestart { get; set; }
	IEnumerable<FileReference> IChartFileEvent.ChartFiles => Chart.IsEmpty ? [] : [Chart];
	IEnumerable<FileReference> IFileEvent.Files => Chart.IsEmpty ? [] : [Chart];
}
