namespace RhythmBase.Global.Diff;

/// <summary>
/// Marks a type so the source generator emits a strongly typed differ for it, allowing optional
/// non-event sections (settings, rows, decorations, tiles) to be compared property by property.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
public sealed class DiffableAttribute : Attribute
{
}
