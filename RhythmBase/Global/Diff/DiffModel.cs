using System.Text;
using System.Text.Json;

namespace RhythmBase.Global.Diff;

/// <summary>
/// Describes how an event changed between two charts.
/// </summary>
public enum DiffChangeKind
{
	/// <summary>The event only exists in the right-hand side.</summary>
	Added,
	/// <summary>The event only exists in the left-hand side.</summary>
	Removed,
	/// <summary>The event exists on both sides but at least one property differs.</summary>
	Modified,
}

/// <summary>
/// A single property difference inside an event or nested object.
/// </summary>
public sealed class DiffPropertyChange
{
	/// <summary>Gets the dotted path of the changed value (for example <c>files[0].path</c>).</summary>
	public string Path { get; init; } = "";
	/// <summary>Gets the value on the left-hand side.</summary>
	public object? OldValue { get; init; }
	/// <summary>Gets the value on the right-hand side.</summary>
	public object? NewValue { get; init; }
	/// <inheritdoc/>
	public override string ToString() => $"{Path}: {Format(OldValue)} -> {Format(NewValue)}";
	internal static string Format(object? value) => value switch
	{
		null => "<null>",
		JsonElement element => element.GetRawText(),
		_ => value.ToString() ?? "<null>",
	};
}

/// <summary>
/// A difference for one event between two charts.
/// </summary>
public sealed class DiffEventChange
{
	/// <summary>Gets the boxed event type enum value.</summary>
	public object? Type { get; init; }
	/// <summary>Gets the display name of the event type.</summary>
	public string TypeName { get; init; } = "";
	/// <summary>Gets the kind of change.</summary>
	public DiffChangeKind Kind { get; init; }
	/// <summary>Gets the formatted position of the event.</summary>
	public string Position { get; init; } = "";
	/// <summary>Gets the property-level changes when <see cref="Kind"/> is <see cref="DiffChangeKind.Modified"/>.</summary>
	public IReadOnlyList<DiffPropertyChange> Properties { get; init; } = [];
	/// <summary>Gets the left-hand event instance.</summary>
	public object? Left { get; init; }
	/// <summary>Gets the right-hand event instance.</summary>
	public object? Right { get; init; }
	/// <inheritdoc/>
	public override string ToString()
	{
		if (Kind != DiffChangeKind.Modified || Properties.Count == 0)
			return $"{Kind} {TypeName} @ {Position}";
		StringBuilder builder = new();
		builder.Append($"{Kind} {TypeName} @ {Position}");
		foreach (DiffPropertyChange property in Properties)
			builder.Append($"\n  {property}");
		return builder.ToString();
	}
}

/// <summary>
/// The result of comparing two charts.
/// </summary>
public sealed class LevelDiff
{
	/// <summary>Gets the ordered list of event changes.</summary>
	public List<DiffEventChange> Events { get; } = [];
	/// <summary>
	/// Gets the optional, non-event changes (settings, rows, decorations, tiles) when the corresponding
	/// <see cref="DiffOptions"/> flags are enabled.
	/// </summary>
	public List<DiffEventChange> Structures { get; } = [];
	/// <summary>Gets a value indicating whether the two charts are identical.</summary>
	public bool IsEmpty => Events.Count == 0 && Structures.Count == 0;
	/// <inheritdoc/>
	public override string ToString()
	{
		if (IsEmpty)
			return "No differences.";
		StringBuilder builder = new();
		foreach (DiffEventChange change in Events)
			builder.AppendLine(change.ToString());
		if (Structures.Count > 0)
		{
			builder.AppendLine("-- structures --");
			foreach (DiffEventChange change in Structures)
				builder.AppendLine(change.ToString());
		}
		return builder.ToString();
	}
}
