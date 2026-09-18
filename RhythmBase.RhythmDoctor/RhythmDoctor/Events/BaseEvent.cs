using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Utils;
using System.ComponentModel;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// The base class of the event.
/// All event types inherit directly or indirectly from this.
/// </summary>
public abstract record class BaseEvent : IBaseEvent
{
	/// <summary>
	/// The base chart of the event. If the event is not associated with a chart, it returns null.
	/// </summary>
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Chart? BaseChart => _tick.BaseChart;
	/// <summary>
	/// Clones the current instance of <see cref="BaseEvent"/> and returns a new instance with the same values.
	/// </summary>
	/// <param name="source"></param>
	public BaseEvent(BaseEvent source)
	{
		_tick = source._tick.WithoutLink();
		Y = source.Y;
		Tag = source.Tag;
		RunTag = source.RunTag;
		Condition = source.Condition;
		Active = source.Active;
		_extraData = [];
		foreach(var kvp in source._extraData)
			_extraData[kvp.Key] = kvp.Value;
	}
	///<inheritdoc/>
	public abstract EventType Type { get; }
	///<inheritdoc/>
	public abstract Tab Tab { get; }
	///<inheritdoc/>
	public virtual TickTime TickTime
	{
		get => _tick;
		set
		{
			if (!value.IsEmpty && _tick == value)
				return;
			BeatCalculator? c = _tick.BaseChart?.Calculator;
			_tick.BaseChart?.Remove(this);
			_tick = c == null ?
				value.WithoutLink() :
				new(c,value);
			_tick.BaseChart?.Add(this);
		}
	}
	///<inheritdoc/>
	public virtual int Y { get; set; }
	///<inheritdoc/>
	public string Tag { get; set; } = "";
	///<inheritdoc/>
	public bool RunTag { get; set; } = false;
	/// <summary>
	/// Gets a reference to the condition associated with the event.
	/// </summary>
	public ref Condition Condition => ref _condition;
	///<inheritdoc/>
	public virtual bool Active { get; set; } = true;
	///<inheritdoc/>
	public JsonElement this[string propertyName]
	{
		get => _extraData.TryGetValue(propertyName, out JsonElement value) ? value : default;
		set
		{
			if (value.ValueKind == JsonValueKind.Undefined)
				_extraData.Remove(propertyName);
			else
				_extraData[propertyName] = value;
		}
	}
	internal Dictionary<string, JsonElement> _extraData = [];
	/// <summary>
	/// Creates a new instance of the specified event type, copying the current event's properties and assigning a new beat
	/// instance without links.
	/// </summary>
	/// <remarks>If the current event is already of the specified type, this method returns a copy of that instance
	/// with the beat replaced. Use this method to create variations of events while preserving their core
	/// properties.</remarks>
	/// <typeparam name="TEvent">The type of event to create. Must inherit from BaseEvent and have a parameterless constructor.</typeparam>
	/// <returns>A new instance of the specified event type with properties cloned from the current event and a new beat instance
	/// without links.</returns>
	public virtual TEvent CloneAs<TEvent>() where TEvent : BaseEvent, new()
	{
		if (this is TEvent t)
		{
			return t with { _tick = TickTime.WithoutLink() };
		}
		TEvent temp = new()
		{
			TickTime = TickTime.WithoutLink(),
			Y = Y,
			Tag = Tag,
			RunTag = RunTag,
			Active = Active,
		};
		temp.Condition = Condition.Clone();
		return temp;
	}
	/// <inheritdoc/>
	internal TickTime _tick = new(1f);
	internal Condition _condition = new();
	/// <summary></summary>
	protected virtual bool PrintMembers(System.Text.StringBuilder builder)
	{
		builder.Append($"[{(BaseChart is null ? "U" : "L")}]");
		builder.Append($" {nameof(TickTime)} = {TickTime}");
		builder.Append($", {nameof(Type)} = {Type}");
		builder.Append($", {nameof(Tab)} = {Tab}");
		builder.Append($", {nameof(Y)} = {Y}");
		builder.Append($", {nameof(Tag)} = {Tag}");
		builder.Append($", {nameof(RunTag)} = {RunTag}");
		builder.Append($", {nameof(Condition)} = {Condition}");
		return true;
	}
}
