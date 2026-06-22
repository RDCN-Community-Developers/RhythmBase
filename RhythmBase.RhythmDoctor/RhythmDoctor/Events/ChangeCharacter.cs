using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>  
/// Represents an action to change the character in a row event.  
/// </summary>  
[JsonObjectSerializable]
public record class ChangeCharacter : BaseRowAction
{
	/// <inheritdoc/>  
	public override EventType Type => EventType.ChangeCharacter;

	/// <inheritdoc/>  
	public override Tab Tab => Tab.Actions;

	/// <summary>  
	/// Gets or sets the character to be changed to.  
	/// </summary>  
	[JsonIgnore]
	public Character Character { get; set; } = GameCharacter.Samurai;
	[JsonAlias("character")]
	internal GameCharacter EnumCharacter
	{
		get => Character.EnumName; set => Character = value;
	}

	[JsonCondition($"$&.{nameof(Character)}.{nameof(Components.Character.IsCustom)} && !string.IsNullOrEmpty($&.{nameof(StringCharacter)})")]
	[JsonAlias("customCharacter")]
	internal string StringCharacter { get => Character.IsCustom ? Character.StringName ?? string.Empty : string.Empty; set => Character = value; }
	/// <summary>  
	/// Gets or sets the transition type for the character change.  
	/// </summary>  
	public Transition Transition { get; set; } = Transition.Instant;
}
