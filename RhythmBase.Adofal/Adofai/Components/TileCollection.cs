using RhythmBase.Adofai.Events;
using System.Collections;
namespace RhythmBase.Adofai.Components;

	/// <summary>
	/// Represents a collection of ADOFAI tiles.
	/// </summary>
	public abstract class TileCollection : ICollection<Tile>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TileCollection"/> class.
		/// </summary>
		protected TileCollection()
		{
			tileOrder = [];
			IsReadOnly = false;
			End = [];
			Start = End;
		}
		/// <summary>
		/// Gets the number of tiles in the collection.
		/// </summary>
		public int Count => tileOrder.Count;
		/// <summary>
		/// Gets a value indicating whether the collection is read-only.
		/// </summary>
		public bool IsReadOnly { get; }
		/// <summary>
		/// Gets the start tile of the collection.
		/// </summary>
		public Tile Start { get; private set; }
		/// <summary>
		/// Gets the end tile of the collection.
		/// </summary>
		public Tile End { get; }
		/// <summary>
		/// Gets the tile at the specified index.
		/// </summary>
		/// <param name="index">The index of the tile to retrieve.</param>
		/// <returns>The tile at the specified index, or the end tile if the index equals the count.</returns>
		public Tile this[int index] => index == tileOrder.Count ? End : tileOrder[index];
		/// <summary>
		/// Gets an enumerable collection of all events associated with the tiles in the collection.
		/// </summary>
		public virtual IEnumerable<IBaseEvent> Events
		{
			get
			{
				foreach (Tile tile in tileOrder)
					foreach (BaseTileEvent action in tile)
						yield return action;
				foreach (BaseTileEvent action2 in End)
					yield return action2;
			}
		}
		/// <summary>
		/// Adds a tile to the collection.
		/// </summary>
		/// <param name="item">The tile to add.</param>
		public virtual void Add(Tile item)
		{
			if (item.Previous != null || item.Next != null)
				throw new InvalidOperationException("Tile is already in a collection.");
			if (tileOrder.Count == 0)
				Start = item;
			item.Next = End;
			End.Previous = item;
			tileOrder.Add(item);
		}
		/// <summary>  
		/// Inserts a tile into the collection at the specified index.  
		/// </summary>  
		/// <param name="index">The zero-based index at which the tile should be inserted.</param>  
		/// <param name="item">The tile to insert into the collection.</param>  
		public virtual void Insert(int index, Tile item)
		{
			if (item.Previous != null || item.Next != null)
				throw new InvalidOperationException("Tile is already in a collection.");

			if (index < 0 || index > tileOrder.Count)
				throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
			if (index == tileOrder.Count)
			{
				if (tileOrder.Count == 0)
					Start = item;
				item.Previous = End.Previous;
				item.Next = End;
				item.Previous?.Next = item;
				End.Previous = item;
			}
			else
			{
				Tile nextTile = tileOrder[index];
				Tile? previousTile = nextTile.Previous;

				item.Next = nextTile;
				item.Previous = previousTile;

				nextTile.Previous = item;
				if (previousTile != null)
					previousTile.Next = item;
				else
					Start = item;
			}

			tileOrder.Insert(index, item);
		}
		/// <summary>
		/// Removes all tiles from the collection.
		/// </summary>
		public virtual void Clear()
		{
			foreach (Tile tile in tileOrder)
			{
				tile.Previous = null;
				tile.Next = null;
			}
			Start = End;
			End.Previous = null;
			tileOrder.Clear();
		}

		/// <summary>  
		/// Copies the elements of the collection to an array, starting at a particular array index.  
		/// </summary>  
		/// <param name="array">The destination array to which the elements will be copied.</param>  
		/// <param name="arrayIndex">The zero-based index in the destination array at which copying begins.</param>  
		public void CopyTo(Tile[] array, int arrayIndex) => tileOrder.CopyTo(array, arrayIndex);
		/// <summary>
		/// Determines whether the collection contains a specific tile.
		/// </summary>
		/// <param name="item">The tile to locate in the collection.</param>
		/// <returns><c>true</c> if the tile is found; otherwise, <c>false</c>.</returns>
		public bool Contains(Tile item) => item == End || tileOrder.Contains(item);
		/// <summary>
		/// Removes the first occurrence of a specific tile from the collection.
		/// </summary>
		/// <param name="item">The tile to remove.</param>
		/// <returns><c>true</c> if the tile was successfully removed; otherwise, <c>false</c>.</returns>
		public virtual bool Remove(Tile item)
		{
			if (item == End)
				return false;
			Tile? previousTile = item.Previous;
			Tile? nextTile = item.Next;
			if (previousTile != null)
				previousTile.Next = nextTile;
			else if(nextTile != null)
				Start = nextTile;
			nextTile?.Previous = previousTile;
			item.Previous = null;
			item.Next = null;
			return tileOrder.Remove(item);
		}
		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator for the collection.</returns>
		public IEnumerator<Tile> GetEnumerator()
		{
			foreach (Tile tile in tileOrder)
				yield return tile;
			yield return End;
		}

		/// <summary>
		/// Gets the index of a specific tile in the collection.
		/// </summary>
		/// <param name="item">The tile to locate.</param>
		/// <returns>The index of the tile if found; otherwise, -1.</returns>
		public int IndexOf(Tile item) => item == End ? Count : tileOrder.IndexOf(item);
		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator for the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		/// <summary>
		/// The internal list that stores the tiles in the collection.
		/// </summary>
		internal List<Tile> tileOrder;
	}
