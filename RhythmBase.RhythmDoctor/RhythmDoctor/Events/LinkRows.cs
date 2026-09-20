using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.RhythmDoctor.Events
{
	/// <summary>
	/// Represents an event that links two rows together in a rhythm doctor level.
	/// </summary>
	[JsonObjectSerializable]
	public record class LinkRows : BaseRowAction
	{
		/// <inheritdoc/>
		public override EventType Type => EventType.LinkRows;
		/// <inheritdoc/>
		public override Tab Tab => Tab.Actions;
		/// <summary>
		/// Gets or sets the index of the source row to link with the target row.
		/// </summary>
		public int SourceRow { get; set; }
		/// <summary>
		/// Gets or sets the action to perform when linking the rows.
		/// </summary>
		[JsonCondition($"$&.{nameof(Parent)}.{nameof(Parent.Index)} != $&.{nameof(SourceRow)}")]
		public LinkRowsAction Action { get; set; }
		/// <summary>
		/// Gets or sets the behavior of the beats when linking the rows.
		/// </summary>
		[JsonAlias("beatBehavior")]
		[JsonCondition($"""
			$&.{nameof(Parent)}.{nameof(Parent.Index)} != $&.{nameof(SourceRow)} &&
			$&.{nameof(Action)} is {nameof(LinkRowsAction)}.{nameof(LinkRowsAction.Link)}
			""")]
		public LinkRowsBeatBehavior Behavior { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the beatsounds should be muted when linking the rows.
		/// </summary>
		[JsonAlias("muteBeatsounds")]
		[JsonCondition($"""
			$&.{nameof(Parent)}.{nameof(Parent.Index)} != $&.{nameof(SourceRow)} &&
			$&.{nameof(Action)} is {nameof(LinkRowsAction)}.{nameof(LinkRowsAction.Link)} &&
			$&.{nameof(Behavior)} is {nameof(LinkRowsBeatBehavior)}.{nameof(LinkRowsBeatBehavior.CopyBeats)}
			""")]
		public bool MuteBeatsounds { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether to normalize the mistake weight when linking the rows.
		/// </summary>
		[JsonAlias("normalizeMistakeWeight")]
		[JsonCondition($"""
			$&.{nameof(Parent)}.{nameof(Parent.Index)} != $&.{nameof(SourceRow)} &&
			$&.{nameof(Action)} is {nameof(LinkRowsAction)}.{nameof(LinkRowsAction.Link)} &&
			$&.{nameof(Behavior)} is {nameof(LinkRowsBeatBehavior)}.{nameof(LinkRowsBeatBehavior.CopyBeats)}
			""")]
		public bool NormalizeMistakeWeight { get; set; } 
		/// <summary>
		/// Gets or sets a value indicating whether to copy the paint effects when linking the rows.
		/// </summary>
		[JsonAlias("copyPaintEffects")]
		[JsonCondition($"""
			$&.{nameof(Parent)}.{nameof(Parent.Index)} != $&.{nameof(SourceRow)} &&
			$&.{nameof(Action)} is {nameof(LinkRowsAction)}.{nameof(LinkRowsAction.Link)}
			""")]
		public bool EnableCopyPaintEffects { get; set; } 
	}
}
