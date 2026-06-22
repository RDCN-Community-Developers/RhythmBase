namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// A Character.
/// </summary>
public readonly struct Character : IEquatable<Character>
{
    /// <summary>
    /// Whether  in-game character or customized character(sprite).
    /// </summary>
    public bool IsCustom => EnumName is GameCharacter.Custom;
    /// <summary>
    /// In-game character.
    /// <br />
    /// If using a customized character, this value will be empty
    /// </summary>
    public GameCharacter EnumName { get; } = GameCharacter.Custom;
    /// <summary>
    /// Customized character(sprite).
    /// <br />
    /// If using an in-game character, this value will be empty
    /// </summary>
    public string? StringName { get; }
    /// <summary>
    /// Construct an in-game character.
    /// </summary>
    /// <param name="character">Character type.</param>
    public Character(GameCharacter character)
    {
        EnumName = character;
        StringName = null;
    }
    /// <summary>
    /// Construct a customized character.
    /// </summary>
    /// <param name="character">A sprite.</param>
    public Character(string character)
    {
        EnumName = GameCharacter.Custom;
        StringName = character;
    }
    internal IEnumerable<FileReference> GetAllPossibleFileReferences()
    {
        if (IsCustom && StringName is string cc)
        {
            if (!string.IsNullOrEmpty(Path.GetExtension(cc)))
                yield return cc;
            else
            {
                yield return cc + ".png";
                yield return cc + ".json";
                yield return cc + "_glow.png";
                yield return cc + "_outline.png";
                yield return cc + "_freeze.png";
            }
        }
    }
    /// <inheritdoc/>
    public static implicit operator Character(GameCharacter character) => new(character);
    /// <inheritdoc/>
    public static implicit operator Character(string character) => new(character);
    /// <inheritdoc/>
    public static bool operator ==(Character left, Character right) => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(Character left, Character right) => !(left == right);
    /// <inheritdoc/>
    public readonly override string ToString() => (IsCustom
        ? StringName
        : EnumName.ToString())
        ?? "[Null]";
    /// <inheritdoc/>
    public bool Equals(Character other) => EnumName == other.EnumName
            && StringName == other.StringName;
    /// <inheritdoc/>
    public override int GetHashCode()
    {
#if NET7_0_OR_GREATER
		return HashCode.Combine(IsCustom, EnumName, StringName);
#else
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + IsCustom.GetHashCode();
            hash = hash * 23 + EnumName.GetHashCode();
            hash = hash * 23 + (StringName != null ? StringName.GetHashCode() : 0);
            return hash;
        }
#endif
    }
    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Character e && Equals(e);
    }
}
