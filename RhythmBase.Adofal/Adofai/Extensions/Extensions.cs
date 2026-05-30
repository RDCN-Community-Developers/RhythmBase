using RhythmBase.Adofai.Components;

namespace RhythmBase.Adofai.Extensions;

	/// <summary>  
	/// Provides extension methods for the <see cref="Level"/> class.  
	/// </summary>  
	public static class Extensions
	{
		/// <summary>  
		/// Adds a range of <see cref="Tile"/> objects to the specified <see cref="Level"/>.  
		/// </summary>  
		/// <param name="level">The <see cref="Level"/> to which the tiles will be added.</param>  
		/// <param name="tiles">The collection of <see cref="Tile"/> objects to add to the level.</param>  
		public static void AddRange(this Level level, IEnumerable<Tile> tiles)
		{
			foreach (Tile item in tiles)
			{
				level.Insert(level.Count, item);
			}
		}
		/// <summary>  
		/// Inserts a range of <see cref="Tile"/> objects into the specified <see cref="Level"/> at the given index.  
		/// </summary>  
		/// <param name="level">The <see cref="Level"/> to which the tiles will be inserted.</param>  
		/// <param name="index">The zero-based index at which the tiles should be inserted.</param>  
		/// <param name="tiles">The collection of <see cref="Tile"/> objects to insert into the level.</param>  
		public static void InsertRange(this Level level, int index, IEnumerable<Tile> tiles)
		{
			foreach (Tile item in tiles)
			{
				level.Insert(index, item);
				index++;
			}
		}
		/// <summary>
		/// Creates an array containing multiple copies of the specified tile.
		/// </summary>
		/// <param name="tile">The tile to be repeated. Must not be <see langword="null"/>.</param>
		/// <param name="count">The number of times the tile should be repeated. Must be greater than or equal to 0.</param>
		/// <returns>An array of tiles, where each element is a clone of the specified tile.  Returns an empty array if <paramref
		/// name="count"/> is 0.</returns>
		public static Tile[] Repeat(this Tile tile, int count)
		{
			Tile[] result = new Tile[count];
			for (int i = 0; i < count; i++)
				result[i] = tile.Clone();
			return result;
		}
	}
