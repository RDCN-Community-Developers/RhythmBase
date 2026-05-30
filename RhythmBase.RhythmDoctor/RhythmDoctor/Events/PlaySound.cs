using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to play a sound.
/// </summary>
[JsonObjectSerializable]
public record class PlaySound : BaseEvent, IAudioFileEvent
{
	/// <summary>
	/// Gets or sets the type of the custom sound.
	/// </summary>
	public CustomSoundType CustomSoundType { get; set; } = CustomSoundType.CueSound;
	/// <summary>
	/// Gets or sets the audio sound.
	/// </summary>
	public Audio? Sound { get; set; } = new() { Filename = "Shaker" };
	///<inheritdoc/>
	public override EventType Type => EventType.PlaySound;
	///<inheritdoc/>
	public override Tab Tab => Tab.Sounds;

	IEnumerable<FileReference> IAudioFileEvent.AudioFiles => Sound is not null && Sound.IsFile ? [Sound.Filename] : [];
	IEnumerable<FileReference> IFileEvent.Files => Sound is not null && Sound.IsFile ? [Sound.Filename] : [];
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {CustomSoundType}";
}