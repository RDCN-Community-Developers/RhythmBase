namespace RhythmBase.Global.Diff;

/// <summary>
/// Controls which optional, non-event sections are included in a diff.
/// Event-sequence diffing is always performed; these sections are opt-in.
/// </summary>
public sealed class DiffOptions
{
	/// <summary>Gets or sets a value indicating whether chart/level settings are compared.</summary>
	public bool IncludeSettings { get; set; }
	/// <summary>Gets or sets a value indicating whether row definitions are compared.</summary>
	public bool IncludeRows { get; set; }
	/// <summary>Gets or sets a value indicating whether decoration definitions are compared.</summary>
	public bool IncludeDecorations { get; set; }
	/// <summary>Gets or sets a value indicating whether tile/floor definitions are compared.</summary>
	public bool IncludeTiles { get; set; }
	/// <summary>Gets an options instance that enables every optional section.</summary>
	public static DiffOptions All { get; } = new()
	{
		IncludeSettings = true,
		IncludeRows = true,
		IncludeDecorations = true,
		IncludeTiles = true,
	};
}
