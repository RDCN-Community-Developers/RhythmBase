namespace RhythmBase.Adofai.Components;

	/// <summary>  
	/// Represents a reference to a tile, including its type and offset.  
	/// </summary>  
	public struct TileReference
	{
		/// <summary>  
		/// Gets or sets the type of the tile reference.  
		/// </summary>  
		public RelativeType Type { get; set; }
		/// <summary>  
		/// Gets or sets the offset of the tile reference.  
		/// </summary>  
		public int Offset { get; set; }
	}
