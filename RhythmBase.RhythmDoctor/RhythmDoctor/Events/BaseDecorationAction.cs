using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Converters;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents the base class for decoration actions.
/// </summary>
[JsonObjectHasSerializer(typeof(RDMemberConverter.BaseDecorationAction<>))]
public abstract record class BaseDecorationAction : BaseEvent, IBaseEvent
{
	/// <summary>
	/// Gets the parent decoration event collection.
	/// </summary>
	public Decoration? Parent => _parent;
	///<inheritdoc/>
	public override int Y { get => Parent?.Y ?? base.Y; set => base.Y = value; }
	/// <inheritdoc/>
	public override Tab Tab => Tab.Decorations;
	/// <summary>
	/// Gets the target identifier.
	/// </summary>
	public virtual string Target => Parent?.Id ?? _decoId;
	///<inheritdoc/>
	public override TEvent CloneAs<TEvent>()
	{
		TEvent temp = base.CloneAs<TEvent>();
		if (temp is BaseDecorationAction tempAction)
			tempAction._decoId = Parent?.Id ?? "";
		return temp;
	}
	/// <summary>
	/// Gets the room associated with this action.
	/// </summary>
	public SingleRoom Room => Parent?.Room ?? SingleRoom.Default;
	/// <summary>
	/// The parent decoration event collection.
	/// </summary>
	internal Decoration? _parent;
	internal string _decoId = "";
}
