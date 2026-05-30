using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an action to set the beat sound in the rhythm base.
/// </summary>
[JsonObjectSerializable]
public record class SetBeatSound : BaseRowAction, IAudioFileEvent
{
	/// <summary>
	/// Gets or sets the audio sound for the beat.
	/// </summary>
	public Audio Sound { get; set; } = new()
	{
		Filename = "Shaker",
	};
	/// <summary>
	/// Gets the type of the event.
	/// </summary>
	public override EventType Type => EventType.SetBeatSound;
	/// <summary>
	/// Gets the tab associated with the event.
	/// </summary>
	public override Tab Tab => Tab.Sounds;
	IEnumerable<FileReference> IAudioFileEvent.AudioFiles => Sound.IsFile ? [Sound.Filename] : [];
	IEnumerable<FileReference> IFileEvent.Files => Sound.IsFile ? [Sound.Filename] : [];
}
