namespace RhythmBase.Adofai.Events;

	/// <summary>  
	/// Represents an editor comment event in ADOFAI.  
	/// </summary>  
	[JsonObjectSerializable]
	public class EditorComment : BaseTileEvent, IBeginningEvent
	{
		/// <inheritdoc/>
		public override EventType Type => EventType.EditorComment;		/// <summary>  
		/// Gets or sets the comment associated with this event.  
		/// </summary>  
		public string Comment { get; set; } = string.Empty;
	}