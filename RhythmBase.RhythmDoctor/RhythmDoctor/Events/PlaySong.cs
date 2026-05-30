using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to play a song with specific beats per minute and other properties.
/// </summary>
[JsonObjectSerializable]
public record class PlaySong : BaseBeatsPerMinute, IBarBeginningEvent, IAudioFileEvent
{
	/// <summary>
	/// Gets or sets the beats per minute (BPM) for the song.
	/// </summary>
	[JsonAlias("bpm")]
	public override float BeatsPerMinute
	{
		get => base.BeatsPerMinute;
		set => base.BeatsPerMinute = value;
	}
	/// <summary>
	/// Gets or sets the offset time for the song.
	/// </summary>
	[JsonTime(JsonTimeType.Milliseconds)]
	[JsonCondition($"$&.{nameof(Offset)} != TimeSpan.Zero")]
	public TimeSpan Offset
	{
		get => Song.Offset;
		set => Song.Offset = value;
	}
	/// <summary>
	/// Gets or sets a value indicating whether the song should loop.
	/// </summary>
	public bool Loop { get; set; } = false;
	///<inheritdoc/>
	public override EventType Type { get; } = EventType.PlaySong;
	///<inheritdoc/>
	public override Tab Tab { get; } = Tab.Sounds;
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" BPM:{BeatsPerMinute}, Song:{Song.Filename}";
	/// <summary>
	/// Gets or sets the song to be played.
	/// </summary>
	public Audio Song { get; set; } = new()
	{
		Filename = "sndOrientalTechno"
	};
	IEnumerable<FileReference> IAudioFileEvent.AudioFiles => Song.IsFile ? [Song.Filename] : [];
	IEnumerable<FileReference> IFileEvent.Files => Song.IsFile ? [Song.Filename] : [];
}
