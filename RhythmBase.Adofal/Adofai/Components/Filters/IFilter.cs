namespace RhythmBase.Adofai.Components.Filters;

/// <summary>
/// Defines a contract for a filter with an associated name.
/// </summary>
/// <remarks>The <see cref="IFilter"/> interface provides a mechanism to retrieve the name of a filter instance.
/// Implementations may define specific behaviors or additional properties and methods.</remarks>
public interface IFilter
{
    ///<inheritdoc/>
	public FilterType Type { get; }
}
