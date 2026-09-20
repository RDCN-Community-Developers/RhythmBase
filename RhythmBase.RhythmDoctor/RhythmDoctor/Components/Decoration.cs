using RhythmBase.RhythmDoctor.Events;
using System.Text.Json;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// A decoration.
/// </summary>
public class Decoration : OrderedEventCollection<BaseDecorationAction>, IEventEnumerable<BaseDecorationAction>
{
	/// <summary>
	/// Decorated ID.
	/// </summary>
	public string Id
	{
		get => _id;
		set => _id = value;
	}
	/// <summary>
	/// Decoration index.
	/// </summary>
	public int Index => Parent?.Decorations.ToList().IndexOf(this) ?? -1;
	/// <summary>
	/// Gets the zero-based index of this decoration within its parent's decorations that share the same room, or -1 if
	/// the decoration is not found.
	/// </summary>
	public int Y
	{
		get
		{
			int y = 0;
			for (int i = 0; i < Parent?.Decorations.Count; i++)
			{
				if (Parent.Decorations[i].Room == this.Room && Parent.Decorations[i] == this)
					return y;
				y++;
			}
			return -1;
		}
	}
	/// <summary>
	/// Room.
	/// </summary>
	public SingleRoom Room { get; set; }
	/// <summary>
	/// Decoration type.
	/// </summary>
	public DecorationType Type { get; set; } = DecorationType.Sprite;
	#region sprite
	/// <summary>
	/// The file reference used by the decoration.
	/// </summary>
	[JsonCondition($"$&.{nameof(Type)} is {nameof(DecorationType.Sprite)}")]
	public Character Character { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the preview mode is enabled.
	/// </summary>
	[JsonCondition($"$&.{nameof(Type)} is {nameof(DecorationType.Sprite)}")]
	public bool Preview { get; set; } = false;
	#endregion
	#region text
	/// <summary>
	/// The name of the text decoration.
	/// </summary>
	[JsonAlias("decoName")]
	[JsonCondition($"$&.{nameof(Type)} is {nameof(DecorationType.Text)}")]
	public string Name { get; set; } = string.Empty;
	/// <summary>
	/// The font used for the text decoration.
	/// </summary>
	[JsonCondition($"$&.{nameof(Type)} is {nameof(DecorationType.Text)}")]
	public FontName Font { get; set; } = FontName.Default;
	/// <summary>
	/// The layer used for the text decoration.
	/// </summary>
	[JsonCondition($"$&.{nameof(Type)} is {nameof(DecorationType.Text)}")]
	public LayerType Layer { get; set; } = LayerType.Foreground;
	#endregion
	/// <summary>
	/// Decoration depth.
	/// </summary>
	public int Depth { get; set; }
	/// <summary>
	/// The filter used for this decoration.
	/// </summary>
	[JsonIgnore]
	[Obsolete]
	public Filter Filter { get; set; }
	/// <summary>
	/// The initial visibility of this decoration.
	/// </summary>
	public bool Visible { get; set; } = true;
	/// <summary>
	/// Initializes a new instance of the <see cref="Decoration"/> class.
	/// </summary>
	public Decoration()
	{
		Room = new SingleRoom(RoomIndex.Room1);
		_id = GetHashCode().ToString();
	}
	/// <param name="room">Decoration room.</param>
	internal Decoration(SingleRoom room)
	{
		Room = room;
		_id = GetHashCode().ToString();
	}
	/// <summary>
	/// Add an event to decoration.
	/// </summary>
	/// <param name="item">Decoration event.</param>
	public override bool Add(BaseDecorationAction item)
	{
		if(item.Parent == this)
			return false;
		item.Parent?.Remove(item);
		if(Parent is not null)
			item._tick = item._tick.WithLink(Parent);
		item._target = Id;
		bool success = base.Add(item);
		if (Parent is not null)
			success &= Parent.AddDirectlyInternal(item);
		return success;
	}

	/// <summary>
	/// Remove an event from decoration.
	/// </summary>
	/// <param name="item">A decoration event.</param>
	public override bool Remove(BaseDecorationAction item)
	{
		bool v = (Parent?.RemoveDirectlyInternal(item) ?? true) && base.Remove(item);
		return v;
	}
	internal bool AddDirectly(BaseDecorationAction item) => base.Add(item);
	/// <inheritdoc/>
	public override string ToString() => string.Format("{0}, {1}, {2}, {3}",
			[
					_id,
						Index,
						Room,
						Character
			]);
	/// <summary>
	/// Gets or sets the extra data associated with the decoration using a key-value pair.
	/// </summary>
	/// <param name="key">The key of the extra data to get or set.</param>
	/// <returns>The value of the extra data associated with the specified key.</returns>
	public JsonElement this[string key]
	{
		get => _extraData.TryGetValue(key, out JsonElement value) ? value : default;
		set
		{
			if (value.ValueKind is JsonValueKind.Undefined)
				_extraData.Remove(key);
			else
				_extraData[key] = value;
		}
	}
	/// <summary>  
	/// Creates a shallow copy of the current <see cref="Decoration"/> instance.  
	/// </summary>  
	/// <returns>A new <see cref="Decoration"/> instance that is a shallow copy of the current instance.</returns>  
	public Decoration Clone()
	{
		Decoration s = (Decoration)MemberwiseClone();
		s.Parent = null;
		return s;
	}
	/// <summary>
	/// Gets a read-only view of the extra data associated with the decoration.
	/// </summary>
	public IReadOnlyDictionary<string, JsonElement> ExtraData => _extraData;
	private readonly Dictionary<string, JsonElement> _extraData = [];
	private string _id = "";
	internal Chart? Parent = null;
}
