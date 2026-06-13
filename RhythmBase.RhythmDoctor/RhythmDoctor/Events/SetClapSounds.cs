using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set clap sounds for different players and CPU.
/// </summary>
[JsonObjectSerializable]
public record class SetClapSounds : BaseEvent, IAudioFileEvent
{
	/// <summary>
	/// Gets or sets the clap sound for player 1.
	/// </summary>
	[JsonFlatten(nameof(Audio.Volume), "p1Volume", JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Pitch), "p1Pitch", JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Pan), "p1Pan", JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Offset), "p1Offset", JsonFlattenMode.ReadOnly)]
	public Audio? P1Sound { get; set; }
	/// <summary>
	/// Gets or sets the clap sound for player 2.
	/// </summary>
	[JsonFlatten(nameof(Audio.Volume), "p2Volume", JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Pitch), "p2Pitch", JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Pan), "p2Pan", JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Offset), "p2Offset", JsonFlattenMode.ReadOnly)]
	public Audio? P2Sound { get; set; }
	/// <summary>
	/// Gets or sets the clap sound for the CPU.
	/// </summary>
	[JsonFlatten(nameof(Audio.Volume), "cpuVolume", JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Pitch), "cpuPitch", JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Pan), "cpuPan", JsonFlattenMode.ReadOnly)]
	[JsonFlatten(nameof(Audio.Offset), "cpuOffset", JsonFlattenMode.ReadOnly)]
	public Audio? CpuSound { get; set; }
	/// <summary>
	/// Gets or sets the row type for the event.
	/// </summary>
	public RowType RowType { get; set; } = RowType.Classic;
	///<inheritdoc/>
	public override EventType Type => EventType.SetClapSounds;
	///<inheritdoc/>
	public override Tab Tab => Tab.Sounds;
	IEnumerable<FileReference> IAudioFileEvent.AudioFiles
	{
		get
		{
			IEnumerable<FileReference> files = [];
			if (P1Sound is not null && P1Sound.IsFile)
				files = files.Append(P1Sound.Filename);
			if (P2Sound is not null && P2Sound.IsFile)
				files = files.Append(P2Sound.Filename);
			if (CpuSound is not null && CpuSound.IsFile)
				files = files.Append(CpuSound.Filename);
			return files;
		}
	}
	IEnumerable<FileReference> IFileEvent.Files 
	{
		get
		{
			IEnumerable<FileReference> files = [];
			if (P1Sound is not null && P1Sound.IsFile)
				files = files.Append(P1Sound.Filename);
			if (P2Sound is not null && P2Sound.IsFile)
				files = files.Append(P2Sound.Filename);
			if (CpuSound is not null && CpuSound.IsFile)
				files = files.Append(CpuSound.Filename);
			return files;
		}
	}
}
