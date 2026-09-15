using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.RhythmDoctor.Events
{
	[JsonObjectSerializable]
	public record class LinkRows : BaseRowAction
	{
		/// <inheritdoc/>
		public override EventType Type => EventType.LinkRows;
		/// <inheritdoc/>
		public override Tab Tab => Tab.Actions;
		public int SourceRow { get; set; }
		[JsonCondition($"$&.{(nameof(Parent))}.{nameof(Parent.Index)} != $&.{nameof(SourceRow)}")]
		public LinkRowsAction Action { get; set; }
		[JsonAlias("beatBehavior")]
		[JsonCondition($"""
			$&.{nameof(Parent)}.{nameof(Parent.Index)} != $&.{nameof(SourceRow)} &&
			$&.{nameof(Action)} is {nameof(LinkRowsAction)}.{nameof(LinkRowsAction.Link)}
			""")]
		public LinkRowsBeatBehavior Behavior { get; set; }
		[JsonAlias("muteBeatsounds")]
		[JsonCondition($"""
			$&.{nameof(Parent)}.{nameof(Parent.Index)} != $&.{nameof(SourceRow)} &&
			$&.{nameof(Action)} is {nameof(LinkRowsAction)}.{nameof(LinkRowsAction.Link)} &&
			$&.{nameof(Behavior)} is {nameof(LinkRowsBeatBehavior)}.{nameof(LinkRowsBeatBehavior.CopyBeats)}
			""")]
		public bool MuteBeatsounds { get; set; }
		[JsonAlias("normalizeMistakeWeight")]
		[JsonCondition($"""
			$&.{nameof(Parent)}.{nameof(Parent.Index)} != $&.{nameof(SourceRow)} &&
			$&.{nameof(Action)} is {nameof(LinkRowsAction)}.{nameof(LinkRowsAction.Link)} &&
			$&.{nameof(Behavior)} is {nameof(LinkRowsBeatBehavior)}.{nameof(LinkRowsBeatBehavior.CopyBeats)}
			""")]
		public bool NormalizeMistakeWeight { get; set; } 
		[JsonAlias("copyPaintEffects")]
		[JsonCondition($"""
			$&.{nameof(Parent)}.{nameof(Parent.Index)} != $&.{nameof(SourceRow)} &&
			$&.{nameof(Action)} is {nameof(LinkRowsAction)}.{nameof(LinkRowsAction.Link)}
			""")]
		public bool EnableCopyPaintEffects { get; set; } 
	}
}
