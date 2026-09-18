using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Utils;
using RhythmBase.Global;
using RhythmBase.RhythmDoctor.Extensions;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to set the number of crotchets per bar.
/// </summary>
[JsonObjectSerializable]
public record class SetCrotchetsPerBar : BaseEvent, IBarBeginningEvent
{
	/// <summary>
	/// Gets the type of the event.
	/// </summary>
	public override EventType Type => EventType.SetCrotchetsPerBar;
	/// <summary>
	/// Gets the tab associated with the event.
	/// </summary>
	public override Tab Tab => Tab.Sounds;
	/// <summary>
	/// Gets or sets the visual beat multiplier.
	/// </summary>
	/// <remarks>
	/// Must be a value greater than 1.
	/// </remarks>
	public float VisualBeatMultiplier { get; set; } = 1;
	/// <summary>
	/// Gets or sets the number of crotchets per bar.
	/// </summary>
	/// <remarks>
	/// Must be a value greater than 1.
	/// </remarks>
	public int CrotchetsPerBar
	{
		get => _crotchetsPerBar + 1;
		set
		{
			_crotchetsPerBar = Math.Max(0, value - 1);
			if (_tick._calculator != null)
			{
				TickTime += 0f;
			}
		}
	}
	/// <inheritdoc/>
	public override bool Active
	{
		get => base.Active;
		set
		{
			if (!_tick.IsEmpty)
			{
				OrderedEventCollection<IBaseEvent> b = _tick.BaseChart;
				if (value && !base.Active)
				{
					(int bar, float beat) = _tick;
					if (beat != 1)
						throw new InvalidOperationException($"Cannot activate {nameof(SetCrotchetsPerBar)} event at bar {bar} beat {beat}. It must be at the beginning of a bar.");
					CpbCache cache = new(TickTime.Tick, bar, CrotchetsPerBar);
					bool extra = _tick._calculator.AddCpbAt(cache, (byte)Config.Strategy, out CpbCache fix);
					b.Add(this);
					if (extra)
					{
						SetCrotchetsPerBar cpb = new() { _tick = new TickTime(_tick._calculator, fix.Tick), _crotchetsPerBar = fix.Cpb - 1 };
						b.Add(cpb);
						_tick.BaseChart.OnEventAdded(new(cpb) { IsAutoPopulated = true, });
					}
				}
				else if(!value && base.Active)
				{
					var node = _tick.BaseChart.EventsBeatOrder.FindNode(_tick);
					if (node is null) return;
					var col = node.Value;
					if (!col.ContainsType(EventType.SetCrotchetsPerBar)) return;
					var lastcpb = col.OfType<SetCrotchetsPerBar>().Last();
					if (lastcpb != this) return;
					(int bar, _) = _tick;
					CpbCache cache = new(_tick.Tick, bar, CrotchetsPerBar);
					bool extra = _tick._calculator.RemoveCpbAt(cache, (byte)Config.Strategy, out CpbCache fix);
					b.Remove(this);
					if (extra)
					{
						SetCrotchetsPerBar cpb = new() { _tick = new TickTime(_tick._calculator, fix.Tick), _crotchetsPerBar = fix.Cpb - 1 };
						b.Add(cpb);
						_tick.BaseChart.OnEventRemoved(new(cpb) { IsAutoPopulated = true, });
					}
				}
			}
			base.Active = value;
		}
	}
	internal int _crotchetsPerBar = 7;
}
