namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Represents a base class for different types of conditions.
/// </summary>
public abstract class BaseConditional
{
	/// <summary>
	/// Gets the type of this condition.
	/// </summary>
	public abstract ConditionType Type { get; }
	/// <summary>
	/// Gets or sets the condition tag. Its role has not been clarified.
	/// </summary>
	public string Tag { get; set; } = "";
	/// <summary>
	/// Gets or sets the condition name.
	/// </summary>
	public string Name { get; set; } = "";
	/// <summary>
	/// Gets the 1-based serial number of this condition in the parent collection.
	/// </summary>
	public int Id => checked((ParentCollection?.IndexOf(this) ?? 0) + 1);
	/// <summary>
	/// Returns the name of the condition.
	/// </summary>
	/// <returns>The name of the condition.</returns>
	public override string ToString() => Name;
	/// <summary>
	/// Gets or sets the parent collection of conditions.
	/// </summary>
	internal ConditionalCollection? ParentCollection;
	/// <summary>
	/// Specifies the type of condition.
	/// </summary>
	[JsonEnumSerializable]
	public enum ConditionType
	{
		/// <summary>
		/// Condition based on the last hit.
		/// </summary>
		LastHit,
		/// <summary>
		/// Custom condition.
		/// </summary>
		Custom,
		/// <summary>
		/// Condition based on the number of times executed.
		/// </summary>
		TimesExecuted,
		/// <summary>
		/// Condition based on the language.
		/// </summary>
		Language,
		/// <summary>
		/// Condition based on the player mode.
		/// </summary>
		PlayerMode,
		/// <summary>
		/// Narration.
		/// </summary>
		Narration,
		/// <summary>
		/// Accessibility.
		/// </summary>
		Accessibility,
	}
}
