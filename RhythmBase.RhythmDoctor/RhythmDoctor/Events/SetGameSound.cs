using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the game sound.
/// </summary>
[JsonObjectSerializable]
public partial record class SetGameSound : BaseEvent, IAudioFileEvent
{
	/// <summary>
	/// Gets or sets the audio associated with the game sound event.
	/// This property is used for single sound types and is read-only when multiple sound types are present.
	/// </summary>
	[JsonCondition($"""!$&.{nameof(MultipleSoundTypes)}.Contains($&.{nameof(SoundType)})""")]
	[JsonFlatten(nameof(Audio.Filename), mode: JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Volume), mode: JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Pitch), mode: JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Pan), mode: JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Offset), mode: JsonFlattenMode.ReadOnly)]
	internal Audio? Audio
	{
		get => Sounds.First;
		set => Sounds.First = value;
	}
	/// <summary>  
	/// Gets or sets the type of the sound.  
	/// </summary>  
	public SoundType SoundType { get; set; } = SoundType.SmallMistake;
	///// <summary>  
	///// Gets or sets the filename of the audio.  
	///// </summary>
	//[JsonCondition($"""
	//	!$&.{nameof(MultipleSoundTypes)}.Contains($&.{nameof(SoundType)})
	//	""")]
	//public string Filename
	//{
	//	get => Audio.Filename;
	//	set => Audio.Filename = value;
	//}
	///// <summary>  
	///// Gets or sets the volume of the audio.  
	///// </summary>  
	//[JsonCondition($"""
	//	!$&.{nameof(MultipleSoundTypes)}.Contains($&.{nameof(SoundType)})
	//	&& $&.{nameof(Volume)} != 100
	//	""")]
	//public int Volume
	//{
	//	get => Audio.Volume;
	//	set => Audio.Volume = value;
	//}
	///// <summary>  
	///// Gets or sets the pitch of the audio.  
	///// </summary>  
	//[JsonCondition($"""
	//	!$&.{nameof(MultipleSoundTypes)}.Contains($&.{nameof(SoundType)})
	//	&& $&.{nameof(Pitch)} != 100
	//	""")]
	//public int Pitch
	//{
	//	get => Audio.Pitch;
	//	set => Audio.Pitch = value;
	//}
	///// <summary>  
	///// Gets or sets the pan of the audio.  
	///// </summary>  
	//[JsonCondition($"""
	//	!$&.{nameof(MultipleSoundTypes)}.Contains($&.{nameof(SoundType)})
	//	&& $&.{nameof(Pan)} != 0
	//	""")]
	//public int Pan
	//{
	//	get => Audio.Pan;
	//	set => Audio.Pan = value;
	//}
	///// <summary>  
	///// Gets or sets the offset time of the audio.  
	///// </summary>  
	//[JsonTime(JsonTimeType.Milliseconds)]
	//[JsonCondition($"""
	//	!$&.{nameof(MultipleSoundTypes)}.Contains($&.{nameof(SoundType)})
	//	&& $&.{nameof(Offset)} != TimeSpan.Zero
	//	""")]
	//public TimeSpan Offset
	//{
	//	get => Audio.Offset;
	//	set => Audio.Offset = value;
	//}
	/// <summary>  
	/// Gets or sets the list of sound subtypes.  
	/// </summary>  
	[JsonAlias("soundSubtypes")]
	[JsonCondition($"$&.{nameof(MultipleSoundTypes)}.Contains($&.{nameof(SoundType)})")]
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
	internal readonly ReadOnlyEnumCollection<SoundType> MultipleSoundTypes = [
		SoundType.ClapSoundHold,
		SoundType.ClapSoundHoldP2,
		SoundType.PulseSoundHold,
		SoundType.PulseSoundHoldP2,
		SoundType.BurnshotSound,
		SoundType.FreezeshotSound,
		SoundType.HoldshotSound
	];
}
