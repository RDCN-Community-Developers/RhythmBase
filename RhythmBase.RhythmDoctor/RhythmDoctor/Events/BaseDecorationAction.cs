using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Serialization;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents the base class for decoration actions.
/// </summary>
[JsonObjectHasSerializer(typeof(RDMemberConverter.BaseDecorationAction<>))]
public abstract record class BaseDecorationAction : BaseEvent, IBaseEvent
{
	/// <summary>
	/// Clones the current instance of <see cref="BaseDecorationAction"/> and returns a new instance with the same values.
	/// </summary>
	public BaseDecorationAction(BaseDecorationAction source) : base(source)
	{
		Target = source.Target;
	}
	/// <inheritdoc/>
	public override Tab Tab => Tab.Decorations;
	internal string _target = "";
	/// <summary>
	/// Gets the target identifier.
	/// </summary>
	public virtual string? Target
	{
		get => _target; set
		{
			if (_tick.BaseChart is not null)
				throw new InvalidOperationException($"The property {nameof(Target)} is readonly because it has been added to the chart.");
			_target = value ?? "";
		}
	}
	/// <summary>
	/// The parent decoration of the event. If the event is not associated with a decoration, it returns null.
	/// </summary>
	public Decoration? Parent => Target is null ? null : TickTime.BaseChart?.Decorations[Target];
	///<inheritdoc/>
	public override TEvent CloneAs<TEvent>()
	{
		TEvent temp = base.CloneAs<TEvent>();
		if (temp is BaseDecorationAction baseDecorationAction)
			baseDecorationAction.Target = Target;
		return temp;
	}
	/// <inheritdoc/>
	protected override bool PrintMembers(System.Text.StringBuilder builder)
	{
		if (base.PrintMembers(builder))
			builder.Append(", ");
		if (Parent is null)
			builder.Append($"{nameof(Decoration)} = [?]");
		else
			builder.Append($"{nameof(Decoration)} = [{Parent.Id}]{Parent.Character}");
		return true;
	}
}
