namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents an audio file with properties for volume, pitch, pan, and offset.
/// </summary>
public class Audio
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Audio"/> class with default values.
	/// </summary>
	public Audio() { }
	/// <summary>
	/// Gets or sets the file name of the audio.
	/// </summary>
	public FileReference Filename { get; set; } = "";
	/// <summary>
	/// Gets or sets the volume of the audio.
	/// </summary>
	public int Volume { get; set; } = 100;
	/// <summary>
	/// Gets or sets the pitch of the audio.
	/// </summary>
	public int Pitch { get; set; } = 100;
	/// <summary>
	/// Gets or sets the pan of the audio.
	/// </summary>
	public int Pan { get; set; } = 0;
	/// <summary>
	/// Gets or sets the offset of the audio.
	/// </summary>
	public TimeSpan Offset { get; set; } = TimeSpan.Zero;
	/// <summary>
	/// Gets a value indicating whether the file is a valid audio file based on its extension.
	/// </summary>
	public bool IsFile => sourceArray.Contains(Path.GetExtension(Filename));
	private static readonly string[] sourceArray =
			[
				".mp3",
				".wav",
				".ogg",
				".aif",
				".aiff"
			];
	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString() => Filename;
}
