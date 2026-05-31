using RhythmBase.RhythmDoctor.Converters;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an abstract base class for beat actions.
/// </summary>
[JsonObjectHasSerializer(typeof(RDMemberConverter.BaseBeat<>))]
public abstract record class BaseBeat : BaseRowAction
{
	/// <summary>
	/// Gets the tab associated with the beat action, which is always set to Rows.
	/// </summary>
	public override Tab Tab => Tab.Rows;
	/// <summary>
	/// Gets or sets the Y-coordinate of the object, inheriting the value from the parent if available; otherwise, uses the
	/// base value.
	/// </summary>
	/// <remarks>When a parent object is present, this property reflects the parent's Y-coordinate, ensuring
	/// positional consistency within a hierarchy. Setting this property directly updates the base value, which is used if
	/// no parent is assigned.</remarks>
    public override int Y { get => Parent?.Y ?? base.Y; set => base.Y = value; }
}
