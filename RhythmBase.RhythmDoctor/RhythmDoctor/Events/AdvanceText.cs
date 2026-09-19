using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Extensions;
using System.Diagnostics;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event that advances the text of the <see cref="FloatingText"/>.
/// This event is used to progress through the lines of the <see cref="FloatingText"/>.
/// The event can specify a duration for the fade-out effect when advancing the text,
/// allowing for a smooth transition between lines.
/// </summary>
[JsonObjectSerializable]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record class AdvanceText : BaseEvent, IRoomEvent, IDurationEvent, IAdvanceText
{
	/// <inheritdoc/>
	public override EventType Type => EventType.AdvanceText;
	/// <inheritdoc/>
	[JsonIgnore]
	public Room Rooms
	{
		get => Header?.Rooms ?? new();
		set => Header?.Rooms = value;
	}
	/// <inheritdoc/>
	public override Tab Tab => Tab.Actions;
	/// <summary>
	/// The head <see cref="FloatingText"/> event of this <see cref="AdvanceText"/> event.
	/// </summary>
	[JsonIgnore]
	public FloatingText? Header { get; internal set; }
	/// <summary>
	/// Gets or sets the duration of the fade-out effect, in beats. A value of null indicates that the duration is not
	/// specified.
	/// </summary>
	/// <remarks>The duration must be a non-negaiive value if specified. If set to zero, the fade-out effect will not
	/// occur.</remarks>
	[JsonAlias("fadeOutDuration")]
	public float? Duration { get; set; }
	/// <summary>
	/// Gets the ID of the parent floating text.
	/// </summary>
	[JsonAlias("id")]
	internal int Id => Header?.Id ?? -1;
	float IDurationEvent.Duration { get => Duration ?? Header?.Duration ?? 0; set => Duration = value; }
	/// <inheritdoc/>
	public override string ToString()
	{
		string[]? texts = Header?.SplittedTexts;
		int? index = Header?.Children.IndexOf(this);
		return texts is not null && index is not null && texts.Length > index + 1
			? base.ToString() + $" \"{texts[index.Value + 1]}\""
			: base.ToString() + $" ?";
	}
	private string GetDebuggerDisplay() => ToString();
	/// <inheritdoc/>
	protected override bool PrintMembers(System.Text.StringBuilder builder)
	{
		if (base.PrintMembers(builder))
			builder.Append(", ");
		builder.Append($"{nameof(Room)} = {this.Rooms}");
		builder.Append($", {nameof(Duration)} = {this.Duration}");
		if (Header is null)
			return true;
		string[] texts = Header.SplittedTexts;
		int index = Header.Children.IndexOf(this);
		if (index < 0 || texts.Length < index + 1)
			return true;
		builder.Append($", {nameof(Header)} = \"{texts[index + 1]}\"");
		return true;
	}
}