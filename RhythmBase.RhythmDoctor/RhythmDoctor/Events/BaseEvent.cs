using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Utils;
using System.Text.Json;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// The base class of the event.
/// All event types inherit directly or indirectly from this.
/// </summary>
public abstract record class BaseEvent : IBaseEvent
{
	///<inheritdoc/>
	public abstract EventType Type { get; }
	///<inheritdoc/>
	public abstract Tab Tab { get; }
	///<inheritdoc/>
	public virtual TickTime TickTime
	{
		get => _beat;
		set
		{
			if (!value.IsEmpty && _beat == value)
				return;
			BeatCalculator? c = _beat.BaseLevel?.Calculator;
			_beat.BaseLevel?.Remove(this);
			_beat = c == null ?
				value.WithoutLink() :
				new(c,value);
			_beat.BaseLevel?.Add(this);
		}
	}
	///<inheritdoc/>
	public virtual int Y { get; set; }
	///<inheritdoc/>
	public string Tag { get; set; } = "";
	///<inheritdoc/>
	public bool RunTag { get; set; } = false;
	///<inheritdoc/>
	public Condition Condition { get; set; } = new();
	///<inheritdoc/>
	public bool Active { get; set; } = true;
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
			return t with { _beat = TickTime.WithoutLink() };
		}
		TEvent temp = new()
		{
			TickTime = TickTime.WithoutLink(),
			Y = Y,
			Tag = Tag,
			RunTag = RunTag,
			Condition = Condition,
			Active = Active,
		};
		temp.Condition = Condition.Clone();
		return temp;
	}
	/// <inheritdoc/>
	public override string ToString() => $"{TickTime} {Type}";
	internal TickTime _beat = new(1f);
}
