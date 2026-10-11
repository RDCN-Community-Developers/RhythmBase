using System.Collections;

namespace RhythmBase.Global.Components;

/// <summary>
/// A dictionary of charts keyed by chart name that keeps each chart's <see cref="IChart.Name"/> in
/// sync with the dictionary key: adding or setting an entry assigns the key to the chart's name, and
/// removing or clearing an entry resets it. Renaming goes through <see cref="Rename"/>.
/// </summary>
/// <typeparam name="TChart">The concrete chart type stored in this dictionary.</typeparam>
public class ChartDictionary<TChart> : IDictionary<string, IChart>
	where TChart : IChart
{
	private readonly Dictionary<string, TChart> _dict = [with(StringComparer.OrdinalIgnoreCase)];

	/// <summary>
	/// Gets or sets the chart with the specified name.
	/// </summary>
	/// <param name="key">The chart name.</param>
	/// <returns>The chart with the specified name.</returns>
	public TChart this[string key]
	{
		get => _dict[key];
		set
		{
			if (value is null) throw new ArgumentNullException(nameof(value));
			value.Name = key;
			_dict[key] = value;
		}
	}
	/// <inheritdoc/>
	IChart IDictionary<string, IChart>.this[string key]
	{
		get => _dict[key];
		set
		{
			if (value is not TChart typed)
				throw new ArgumentException($"The chart must be of type {typeof(TChart).Name}.", nameof(value));
			this[key] = typed;
		}
	}
	/// <inheritdoc/>
	public ICollection<string> Keys => _dict.Keys;
	/// <summary>
	/// Gets the charts in this dictionary.
	/// </summary>
	public ICollection<TChart> Values => _dict.Values;
	/// <inheritdoc/>
	ICollection<IChart> IDictionary<string, IChart>.Values => _dict.Values.Cast<IChart>().ToArray();
	/// <inheritdoc/>
	public int Count => _dict.Count;
	/// <inheritdoc/>
	public bool IsReadOnly => false;
	/// <summary>
	/// Adds a chart with the specified name.
	/// </summary>
	/// <param name="key">The chart name.</param>
	/// <param name="value">The chart to add.</param>
	public void Add(string key, TChart value)
	{
		if (value is null) throw new ArgumentNullException(nameof(value));
		value.Name = key;
		_dict.Add(key, value);
	}
	/// <inheritdoc/>
	void IDictionary<string, IChart>.Add(string key, IChart value)
	{
		if (value is not TChart typed)
			throw new ArgumentException($"The chart must be of type {typeof(TChart).Name}.", nameof(value));
		Add(key, typed);
	}
	/// <inheritdoc/>
	public void Add(KeyValuePair<string, TChart> item) => Add(item.Key, item.Value);
	/// <inheritdoc/>
	void ICollection<KeyValuePair<string, IChart>>.Add(KeyValuePair<string, IChart> item)
		=> ((IDictionary<string, IChart>)this).Add(item.Key, item.Value);
	/// <inheritdoc/>
	public bool ContainsKey(string key) => _dict.ContainsKey(key);
	/// <inheritdoc/>
	public bool Contains(KeyValuePair<string, TChart> item) => ((IDictionary<string, TChart>)_dict).Contains(item);
	/// <inheritdoc/>
	bool ICollection<KeyValuePair<string, IChart>>.Contains(KeyValuePair<string, IChart> item)
	{
		if (!_dict.TryGetValue(item.Key, out TChart? chart))
			return false;
		return ReferenceEquals(chart, item.Value);
	}
	/// <inheritdoc/>
	public void CopyTo(KeyValuePair<string, TChart>[] array, int arrayIndex) => ((IDictionary<string, TChart>)_dict).CopyTo(array, arrayIndex);
	/// <inheritdoc/>
	void ICollection<KeyValuePair<string, IChart>>.CopyTo(KeyValuePair<string, IChart>[] array, int arrayIndex)
		=> ((IDictionary<string, TChart>)_dict).Select(p => new KeyValuePair<string, IChart>(p.Key, p.Value)).ToList().CopyTo(array, arrayIndex);
	/// <summary>
	/// Removes the chart with the specified name, resetting its <see cref="IChart.Name"/>.
	/// </summary>
	/// <param name="key">The chart name.</param>
	/// <returns><c>true</c> if the chart was found and removed; otherwise, <c>false</c>.</returns>
	public bool Remove(string key)
	{
		if (_dict.TryGetValue(key, out TChart? value))
		{
			_dict.Remove(key);
			value.Name = string.Empty;
			return true;
		}
		return false;
	}
	/// <inheritdoc/>
	public bool Remove(KeyValuePair<string, TChart> item) => Remove(item.Key);
	/// <inheritdoc/>
	bool ICollection<KeyValuePair<string, IChart>>.Remove(KeyValuePair<string, IChart> item) => Remove(item.Key);
	/// <summary>
	/// Gets the chart with the specified name.
	/// </summary>
	/// <param name="key">The chart name.</param>
	/// <param name="value">The chart with the specified name, if found.</param>
	/// <returns><c>true</c> if a chart with the specified name was found; otherwise, <c>false</c>.</returns>
	public bool TryGetValue(string key, out TChart value) => _dict.TryGetValue(key, out value!);
	/// <inheritdoc/>
	bool IDictionary<string, IChart>.TryGetValue(string key, out IChart value)
	{
		if (_dict.TryGetValue(key, out TChart? chart))
		{
			value = chart;
			return true;
		}
		value = null!;
		return false;
	}
	/// <summary>
	/// Gets the chart with the specified name.
	/// </summary>
	public bool TryGetChart(string key, out TChart value) => TryGetValue(key, out value);
	/// <summary>
	/// Removes all charts, resetting their <see cref="IChart.Name"/>.
	/// </summary>
	public void Clear()
	{
		foreach (TChart chart in _dict.Values)
			chart.Name = string.Empty;
		_dict.Clear();
	}
	/// <summary>
	/// Renames a chart, updating both the dictionary key and the chart's <see cref="IChart.Name"/>.
	/// </summary>
	/// <param name="oldName">The current chart name.</param>
	/// <param name="newName">The new chart name.</param>
	/// <returns><c>true</c> if the chart was found and renamed; otherwise, <c>false</c>.</returns>
	public bool Rename(string oldName, string newName)
	{
		if (string.IsNullOrEmpty(newName))
			throw new ArgumentNullException(nameof(newName));
		if (!_dict.TryGetValue(oldName, out TChart? chart))
			return false;
		_dict.Remove(oldName);
		chart.Name = newName;
		_dict.Add(newName, chart);
		return true;
	}
	/// <inheritdoc/>
	public IEnumerator<KeyValuePair<string, TChart>> GetEnumerator() => _dict.GetEnumerator();
	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	/// <inheritdoc/>
	IEnumerator<KeyValuePair<string, IChart>> IEnumerable<KeyValuePair<string, IChart>>.GetEnumerator()
		=> _dict.Select(p => new KeyValuePair<string, IChart>(p.Key, p.Value)).GetEnumerator();
}
