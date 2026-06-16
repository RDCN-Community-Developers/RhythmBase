namespace RhythmBase.RhythmDoctor.Components;

	/// <summary>
	/// Represents a bookmark in the rhythm base.
	/// </summary>
	public record struct Bookmark
	{
		/// <summary>
		/// Gets or sets the tick where the bookmark is located.
		/// </summary>
		public TickTime Tick { get; set; }
		/// <summary>
		/// Gets or sets the color of the bookmark.
		/// </summary>
		public BookmarkColors Color { get; set; }
		/// <summary>
		/// Returns a string that represents the current bookmark.
		/// </summary>
		/// <returns>A string that represents the current bookmark.</returns>
		public readonly override string ToString() => $"{Tick}, {Color}";
		/// <summary>
		/// Specifies the colors available for bookmarks.
		/// </summary>
		public enum BookmarkColors
		{
			/// <summary>
			/// Represents the color blue.
			/// </summary>
			Blue,
			/// <summary>
			/// Represents the color red.
			/// </summary>
			Red,
			/// <summary>
			/// Represents the color yellow.
			/// </summary>
			Yellow,
			/// <summary>
			/// Represents the color green.
			/// </summary>
			Green
		}
	}
