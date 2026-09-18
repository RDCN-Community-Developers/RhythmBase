using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Serialization;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents a base class for a row event.
/// </summary>
[JsonObjectHasSerializer(typeof(RDMemberConverter.BaseRowAction<>))]
public abstract record class BaseRowAction : BaseEvent
{
	/// <summary>
	/// The parent row of the event. If the event is not associated with a row, it returns null.
	/// </summary>
	public Row? Parent => TickTime.BaseChart is Chart chart && Row >= 0 && Row < chart.Rows.Count ? chart.Rows[Row] : null;
	/// <summary>
	/// Clones the current instance of <see cref="BaseRowAction"/> and returns a new instance with the same values.
	/// </summary>
	public BaseRowAction(BaseRowAction source) : base(source)
	{
		Row = source.Row;
	}
	///<inheritdoc/>
	public override TEvent CloneAs<TEvent>()
	{
		TEvent temp = base.CloneAs<TEvent>();
		if (temp is BaseRowAction tempAction)
			tempAction.Row = Row;
		return temp;
	}
	/// <summary>
	/// The index of the row to which the event belongs. If the event is not associated with a row, it returns -1.
	/// </summary>
	public int Row
	{
		get => _row;
		set
		{
			if (_tick.BaseChart is not null)
				throw new InvalidOperationException($"The property {nameof(Row)} is readonly because it has been added to the chart.");
			_row = value;
		}
	}
	/// <inheritdoc/>
	protected override bool PrintMembers(System.Text.StringBuilder builder)
	{
		if (base.PrintMembers(builder))
			builder.Append(", ");
		builder.Append($"{nameof(Row)} = [{Row}]{Parent?.Character}");
		return true;
	}
	internal int _row;
}
