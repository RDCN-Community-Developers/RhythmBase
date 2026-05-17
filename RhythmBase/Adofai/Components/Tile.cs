using RhythmBase.Adofai.Events;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Adofai.Components
{
	/// <summary>
	/// Represents a tile in the ADOFAI level, containing events and properties related to the tile.
	/// </summary>
	public class Tile : TypedEventCollection<EventType, ADBeat>
	{
		/// <summary>
		/// Gets or sets the angle of the tile.
		/// The value is normalized to the range [-180, 180].
		/// If the value is outside the range [-360, 360], it is set to -999.
		/// </summary>
		public float Angle
		{
			get => _angle;
			set
			{
				_angle = (value % 360 + 540) % 360 - 180;
			}
		}
		/// <summary>
		/// Gets or sets the current beat information for the audio detection system.
		/// </summary>
		public ADBeat Beat { get; set; }
		/// <summary>
		/// Gets a value indicating whether the tile is in a mid-spin state.
		/// A tile is considered mid-spin if its angle is less than -180 or greater than 180.
		/// </summary>
		public bool IsMidSpin { get; set; } = false;
		/// <summary>  
		/// Gets a value indicating whether the tile is a hairpin.  
		/// A tile is considered a hairpin if the absolute difference between its angle and the previous tile's angle is exactly 180 degrees.  
		/// </summary>  
		public bool IsHairPin => Math.Abs(_angle - (Previous?.Angle ?? 0)) == 180f;
		/// <summary>  
		/// Gets the tick value of the tile.  
		/// The tick is calculated based on the relationship between the current tile, the previous tile, and the next tile.  
		/// If the tile is a hairpin, the tick is set to 2.  
		/// If there is no next tile, the tick is set to 0.  
		/// Otherwise, the tick is calculated as the normalized angular difference between the previous tile and the current tile.  
		/// </summary>  
		public float Tick
		{
			get
			{
				float v = (float)(((Previous?.Angle ?? 0) - _angle) / 360f) - 0.5f;
				float tick;
				tick = (1 + v + float.Floor(-v)) * 2;
				return tick;
			}
		}

		///// <summary>
		///// Gets the beat associated with the tile, calculated based on its angle and index.
		///// </summary>
		//public ADBeat Beat => new(Parent?.Calculator, Index + (_angle / 180f));
		/// <summary>
		/// Gets or sets the parent level of the tile.
		/// </summary>
		internal ADLevel? Parent { get; set; }
		/// <summary>
		/// Initializes a new instance of the <see cref="Tile"/> class.
		/// </summary>
		public Tile()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Tile"/> class with a specified angle.
		/// </summary>
		/// <param name="angle">The angle of the tile.</param>
		public Tile(float angle) => Angle = angle;
		/// <summary>
		/// Initializes a new instance of the <see cref="Tile"/> class with a specified mid-spin state.
		/// </summary>
		/// <param name="isMidSpin">A value indicating whether the tile is in a mid-spin state.</param>
		public Tile(bool isMidSpin) => IsMidSpin = isMidSpin;
		/// <summary>
		/// Initializes a new instance of the <see cref="Tile"/> class with a collection of tile events.
		/// </summary>
		/// <param name="actions">The collection of tile events to associate with this tile.</param>
		public Tile(IEnumerable<BaseTileEvent> actions)
		{
			foreach (BaseTileEvent i in actions)
			{
				i.Parent = this;
				Add(i);
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Tile"/> class with a specified angle and a collection of tile events.
		/// </summary>
		/// <param name="angle">The angle of the tile.</param>
		/// <param name="actions">The collection of tile events to associate with this tile.</param>
		public Tile(float angle, IEnumerable<BaseTileEvent> actions) : this(actions) => Angle = angle;
		/// <summary>  
		/// Gets or sets the previous tile in the sequence.  
		/// This property is set internally and represents the tile that comes before the current tile.  
		/// </summary>  
		public Tile? Previous { get; set; }
		/// <summary>  
		/// Gets or sets the next tile in the sequence.  
		/// This property is set internally and represents the tile that comes after the current tile.  
		/// </summary>  
		[NotNull]
		public Tile? Next { get; set; }
		/// <summary>
		/// Initializes a new instance of the <see cref="Tile"/> class with a specified mid-spin state and a collection of tile events.
		/// </summary>
		/// <param name="isMidSpin">A value indicating whether the tile is in a mid-spin state.</param>
		/// <param name="actions">The collection of tile events to associate with this tile.</param>
		public Tile(bool isMidSpin, IEnumerable<BaseTileEvent> actions) : this(actions) => IsMidSpin = isMidSpin;
		/// <summary>
		/// Gets the index of the tile within its parent level.
		/// Returns -1 if the tile does not belong to any level.
		/// </summary>
		public int Index => Parent?.IndexOf(this) ?? Parent?.Count ?? -1;
		/// <summary>
		/// Creates a new instance of the tile with the same state as the current instance.
		/// </summary>
		/// <remarks>The cloned tile will have the same angle and spin state as the original.  Any additional
		/// properties or behaviors not explicitly set in the clone  will need to be manually configured after
		/// cloning.</remarks>
		/// <returns>A new <see cref="Tile"/> instance that is a copy of the current tile.</returns>
		public Tile Clone()
		{
			Tile tile = new()
			{
				_angle = _angle,
				IsMidSpin = IsMidSpin,
			};
			//foreach (BaseTileEvent action in this)
			//{
			//	BaseTileEvent action2 = (BaseTileEvent)action.Clone();
			//	action2.Parent = tile;
			//	tile.Add(action2);
			//}
			return tile;
		}
		/// <summary>
		/// Gets the event of the specified type associated with this tile.
		/// </summary>
		/// <typeparam name="T">The type of event to retrieve.</typeparam>
		/// <returns>The event of type <typeparamref name="T"/> if found; otherwise, <c>null</c>.</returns>
		public T? EventOf<T>() where T : ISingleEvent
		{
			//if(ContainsType(Utils.EventTypeUtils.<T>()))
			//	return OfType<T>().SingleOrDefault();
			throw new NotImplementedException();
		}
        /// <inheritdoc />
		public bool Add(BaseTileEvent item)
		{
			bool result = false;
			if ((item is IBeginningEvent) || (Previous is not null))
			{
				if (Parent is not null)
					switch (item)
					{
						case SetSpeed: // tick
													 //Parent.Calculator
							break;
						case Twirl: // reverse
							break;
						case Pause: // delay
							break;
						case Hold: // delay
							break;
						case FreeRoam: // delay
							break;
						case MultiPlanet: // runtype
							break;
					}
				result = base.Add(item);
			}
			if (result)
				item.Parent = this;
			return result;
		}
		/// <inheritdoc />
		public bool Remove(BaseTileEvent item)
		{
			if (base.Remove(item))
			{
				item.Parent = null;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Returns a string representation of the tile, including its index, beat, angle, and event count.
		/// </summary>
		/// <returns>A string that represents the tile.</returns>
		public override string ToString() => $"[{Index}]<{(
				IsMidSpin ? "MS".PadRight(4) : $"{_angle,4}"
			)}>{(
				this.Any() ? $"[{Count}]" : string.Empty
			)}";
		/// <summary>
		/// Defines an implicit conversion from a float representing an angle to a Tile instance.
		/// </summary>
		/// <remarks>This conversion enables float values to be used directly in contexts where a Tile is expected.
		/// The special value 999 is interpreted as a request for a default Tile instance.</remarks>
		/// <param name="angle">The angle in degrees to convert to a Tile. If the value is 999, a default Tile is created.</param>
		public static implicit operator Tile(float angle)
		{
			return angle == 999 ? new(true) : new(angle);
		}
        /// <summary>
        /// Defines an implicit conversion from a boolean representing a mid-spin state to a Tile instance.
        /// </summary>
        /// <param name="isMidSpin"></param>

        public static implicit operator Tile(bool isMidSpin) => new(isMidSpin);
		/// <summary>
		/// The internal storage for the angle of the tile.
		/// </summary>
		private float _angle = 0;
		internal Status _status;
	}
	internal struct Status
	{
		internal int _index;
		internal bool _flip;
		internal float _bpm;
		internal TimeSpan _time;
		internal Planets _planet;
		internal float _tick;
	}
}
