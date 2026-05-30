namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event that sets the hold sound properties for a tile in the ADOFAI editor.  
/// </summary>  
[JsonObjectSerializable]
public class SetHoldSound : BaseTileEvent, ISingleEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.SetHoldSound;
	/// <summary>  
	/// Gets or sets the sound to play at the start of the hold.  
	/// </summary>  
	public HoldSound HoldStartSound { get; set; } = HoldSound.Fuse;
	/// <summary>  
	/// Gets or sets the sound to loop during the hold.  
	/// </summary>  
	public HoldSound HoldLoopSound { get; set; } = HoldSound.Fuse;
	/// <summary>  
	/// Gets or sets the sound to play at the end of the hold.  
	/// </summary>  
	public HoldSound HoldEndSound { get; set; } = HoldSound.Fuse;
	/// <summary>  
	/// Gets or sets the sound to play in the middle of the hold.  
	/// </summary>  
	public HoldMidSound HoldMidSound { get; set; } = HoldMidSound.Fuse;
	/// <summary>  
	/// Gets or sets the type of the mid-hold sound.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(HoldMidSound)} is not RhythmBase.Adofai.{nameof(Adofai.HoldMidSound)}.{nameof(HoldMidSound.None)}")]
	public HoldMidSoundType HoldMidSoundType { get; set; } = HoldMidSoundType.Once;
	/// <summary>  
	/// Gets or sets the delay before the mid-hold sound is played.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(HoldMidSound)} is not RhythmBase.Adofai.{nameof(Adofai.HoldMidSound)}.{nameof(HoldMidSound.None)}")]
	public float HoldMidSoundDelay { get; set; } = 0.5f;
	/// <summary>  
	/// Gets or sets the timing reference for the mid-hold sound.  
	/// </summary>  
	[JsonCondition($"$&.{nameof(HoldMidSound)} is not RhythmBase.Adofai.{nameof(Adofai.HoldMidSound)}.{nameof(HoldMidSound.None)}")]
	public RelativeType HoldMidSoundTimingRelativeTo { get; set; } = RelativeType.End;
	/// <summary>  
	/// Gets or sets the volume of the hold sound.  
	/// </summary>  
	public int HoldSoundVolume { get; set; } = 100;
}