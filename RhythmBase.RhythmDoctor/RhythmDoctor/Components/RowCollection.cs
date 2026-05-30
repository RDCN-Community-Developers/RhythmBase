using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Components;

	/// <summary>
	/// Represents a collection of row events in a level.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the <see cref="RowCollection"/> class.
	/// </remarks>
	/// <param name="parent">The parent <see cref="Level"/> instance associated with this collection.</param>
	public class RowCollection(Level parent) : LevelElementCollection<Row, BaseRowAction>(parent, true)
	{
		internal readonly List<BaseRowAction> _unhandledRowEvents = [];
		/// <summary>
		/// Adds a <see cref="Row"/> to the collection.
		/// </summary>
		/// <param name="row">The <see cref="Row"/> to add.</param>
		public override void Add(Row row)
		{
			if (_items.Contains(row))
				return;
			row.Parent = parent;
			foreach (BaseRowAction? i in row)
				parent.Add(i);
			foreach(BaseRowAction? e in _unhandledRowEvents.Where(i=>i.Index == Count))
				row.Add(e);
			_items.Add(row);
		}
		/// <summary>
		/// Removes a <see cref="Row"/> from the collection.
		/// </summary>
		/// <param name="row">The <see cref="Row"/> to remove.</param>
		/// <returns>True if the item was successfully removed; otherwise, false.</returns>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="row"/> is null.</exception>
		public override bool Remove(Row row)
		{
			if (!_items.Contains(row))
				return false;
			BaseRowAction[] rowsToRemove = [.. row];
			foreach (BaseRowAction i in rowsToRemove)
				parent.Remove(i);
			row.Parent = null;
			return _items.Remove(row);
		}
		/// <summary>  
		/// Gets or sets the <see cref="Row"/> at the specified index.  
		/// </summary>  
		/// <param name="index">The zero-based index of the <see cref="Row"/> to get or set.</param>  
		/// <returns>The <see cref="Row"/> at the specified index.</returns>  
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the specified index is out of range.</exception>  
		public Row this[int index]
		{
			get => _items[index];
			set
			{
				if (index < 0 || index >= _items.Count)
					throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
				_items[index].Parent = null;
				value.Parent = parent;
				_items[index] = value;
			}
		}
		/// <summary>  
		/// Gets an enumerable collection of <see cref="Row"/> instances associated with the specified <see cref="Room"/>.  
		/// </summary>  
		/// <param name="room">The <see cref="Room"/> to filter the <see cref="Row"/> instances by.</param>  
		/// <returns>An enumerable collection of <see cref="Row"/> instances associated with the specified room.</returns>  
		public IEnumerable<Row> this[Room room]
		{
			get
			{
				foreach (Row? item in _items)
				{
					if (room.Contains(item.Room))
						yield return item;
				}
			}
		}
		/// <summary>  
		/// Removes the <see cref="Row"/> at the specified index from the collection.  
		/// </summary>  
		/// <param name="index">The zero-based index of the <see cref="Row"/> to remove.</param>  
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the specified index is out of range.</exception>  
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= _items.Count)
				throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
			_items[index].Parent = null;
			_items.RemoveAt(index);
		}
		/// <inheritdoc/>
		public override IEnumerable<Row> ElementsOf(Room room) => _items.Where(item => room.Contains(item.Room));
	}