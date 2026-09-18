using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Extensions;

namespace RhythmBase.RhythmDoctor.Events;

[JsonObjectSerializable]
public record class AdvanceTextDecoration : BaseDecorationAction, IAdvanceText
{
	/// <inheritdoc/>
	public override EventType Type { get; } = EventType.AdvanceTextDecoration;
	/// <summary>
	/// Gets or sets the duration of the fade-out effect, in beats. A value of null indicates that the duration is not
	/// specified.
	/// </summary>
	/// <remarks>The duration must be a non-negaiive value if specified. If set to zero, the fade-out effect will not
	/// occur.</remarks>
	[JsonAlias("fadeOutDuration")]
	public float? Duration { get; set; }
	float IDurationEvent.Duration { get => Duration ?? this.FrontOrDefault<SetText>()?.Duration ?? -1; set => Duration = value; }
	protected override bool PrintMembers(System.Text.StringBuilder builder)
	{
		if (base.PrintMembers(builder))
			builder.Append(", ");
		builder.Append($", {nameof(Duration)} = {this.Duration}");
		var head = this.Head;
		if (head is null)
			return true;
		string[] texts = head.SplittedTexts;
		int index = head.Children.IndexOf(this);
		if (index < 0 || texts.Length < index + 1)
			return true;
		builder.Append($", {nameof(Parent)} = \"{texts[index + 1]}\"");
		return true;
	}
}