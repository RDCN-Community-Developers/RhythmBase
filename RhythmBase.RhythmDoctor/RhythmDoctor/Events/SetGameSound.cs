using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the game sound.
/// </summary>
[JsonObjectHasSerializer(typeof(Serialization.RDMemberConverter.SetGameSound))]
public partial record class SetGameSound : BaseEvent, IAudioFileEvent
{
	/// <summary>  
	/// Gets or sets the type of the sound.  
	/// </summary>  
	public SoundType SoundType { get; set; } = SoundType.SmallMistake;
	/// <summary>  
	/// Gets or sets the sound collection. The keys follow the group member order when
	/// <see cref="SoundType"/> is a group key (see <see cref="Constants.SoundGroupTypeMap"/>).
	/// </summary>
	public SoundCollection Sounds { get; set; } = new SoundCollection.SingleAudioSoundCollection(SoundType.ClapSoundP1Classic);
	///<inheritdoc/>
	public override EventType Type => EventType.SetGameSound;
	///<inheritdoc/>
	public override Tab Tab => Tab.Sounds;

	IEnumerable<FileReference> IAudioFileEvent.AudioFiles => Sounds.Values
				.Where(a => a is Audio au && au.IsFile)
				.Select(a => a!.Filename);

	IEnumerable<FileReference> IFileEvent.Files => Sounds.Values
				.Where(a => a is Audio au && au.IsFile)
				.Select(a => a!.Filename);
}
