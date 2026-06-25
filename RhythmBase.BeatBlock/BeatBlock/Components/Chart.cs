using RhythmBase.BeatBlock.Events;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using static RhythmBase.BeatBlock.Constants;

namespace RhythmBase.BeatBlock.Components;

/// <summary>
/// Represents a chart variant in a BeatBlock level.
/// </summary>
public class Chart :
		OrderedEventCollection<IBaseEvent>,
		IChart<TickTime>
{
	internal bool isDefault = false;
	/// <summary>
	/// Gets or sets the charter of the chart.
	/// </summary>
	public string Charter { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets the difficulty of the chart.
	/// </summary>
	public int Difficulty { get; set; }
	/// <summary>
	/// Gets or sets the display name of the chart.
	/// </summary>
	public string Display { get; set; } = string.Empty;
	/// <summary>
	/// Gets or sets a value indicating whether the chart is an extra chart.
	/// </summary>
	public bool Extra { get; set; } = false;
	/// <summary>
	/// Gets or sets a value indicating whether the chart is hidden.
	/// </summary>
	public bool Hidden { get; set; } = false;
	/// <summary>
	/// Gets or sets the name of the chart.
	/// </summary>
	public string Name { get; set; } = string.Empty;
	/// <summary>
	/// Gets the beat calculator for the level.
	/// </summary>
	public BeatCalculator Calculator => new(this);
	/// <summary>
	/// Gets the slot index of this chart in the parent collection.
	/// </summary>
	public int Slot => _parent?._variants.IndexOf(this) ?? -1;
	/// <summary>
	/// Gets a value indicating whether the chart is using the default level data.
	/// </summary>
	[MemberNotNullWhen(false, nameof(LevelFile))]
	public bool IsUsingDefaultLevel { get; private set; } = true;
	/// <summary>
	/// Gets the format version of the chart.
	/// </summary>
	public int FormatVersion => DefaultVersion;
	/// <summary>
	/// Gets or sets the level file path for this chart variant.
	/// </summary>
	public string? LevelFile { get; set; }
	internal Chart() { }
	/// <summary>
	/// Initializes a new instance of the <see cref="Chart"/> class with the specified name.
	/// </summary>
	/// <param name="name">The name of the chart.</param>
	public Chart(string name)
	{
		Name = name;
	}
	internal ChartCollection? _parent;
	/// <summary>
	/// Separates this chart from the default level, copying shared events to create an independent chart.
	/// </summary>
	[MemberNotNull(nameof(LevelFile))]
	public void SeparateFromDefaultLevel()
	{
		if (_parent == null)
			throw new InvalidOperationException($"Variant '{Name}' is not associated with any level.");
		if (!IsUsingDefaultLevel)
			throw new InvalidOperationException($"Variant '{Name}' is already separated from the default level.");
		foreach (var e in _parent.Default)
		{
			if (!Contains(e) && e is not IChartEvent && e is BaseEvent eb)
				Add(eb with { });
		}
		IsUsingDefaultLevel = false;
		LevelFile = $"level-{Name}.json";
	}
	/// <inheritdoc/>
	public override bool Add(IBaseEvent item)
	{
		if (isDefault && item is IChartEvent)
			return false;
		if (item is not BaseEvent be)
			return false;
		var originalCalculator = be._beat._calculator;
		be._beat._calculator = Calculator;
		bool success = base.Add(item);
		if (!success)
			be._beat._calculator = originalCalculator;
		return success;
	}
	/// <inheritdoc/>
	public override bool Remove(IBaseEvent item)
	{
		if (isDefault && item is IChartEvent)
			return false;
		if (base.Remove(item))
		{
			((BaseEvent)item)._beat._calculator = null;
			return true;
		}
		return false;
	}
	/// <inheritdoc/>
	public override bool Contains(IBaseEvent item)
	{
		if (isDefault && item is IChartEvent)
			return false;
		return base.Contains(item);
	}
}
