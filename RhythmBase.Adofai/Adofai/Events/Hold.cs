namespace RhythmBase.Adofai.Events;

	/// <summary>  
	/// Represents a "Hold" event in the ADOFAI editor.  
	/// </summary>  
	[JsonObjectSerializable]
	public class Hold : BaseTileEvent, ISingleEvent
	{
		/// <inheritdoc/>  
		public override EventType Type => EventType.Hold;
		/// <summary>  
		/// Gets or sets the duration of the hold event.  
		/// </summary>  
		public int Duration { get; set; } = 0;
		/// <summary>  
		/// Gets or sets the distance multiplier for the hold event.  
		/// </summary>  
		public int DistanceMultiplier { get; set; } = 100;
		/// <summary>  
		/// Gets or sets a value indicating whether the landing animation is enabled for the hold event.  
		/// </summary>  
		public bool LandingAnimation { get; set; } = false;
	}
