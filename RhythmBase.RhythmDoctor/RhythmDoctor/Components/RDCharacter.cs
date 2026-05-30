namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// A Character.
/// </summary>
public readonly struct RDCharacter : IEquatable<RDCharacter>
{
    /// <summary>
    /// Whether  in-game character or customized character(sprite).
    /// </summary>
    public bool IsCustom => Character is Characters.Custom;
    /// <summary>
    /// In-game character.
    /// <br />
    /// If using a customized character, this value will be empty
    /// </summary>
    public Characters Character { get; } = Characters.Custom;
    /// <summary>
    /// Customized character(sprite).
    /// <br />
    /// If using an in-game character, this value will be empty
    /// </summary>
    public string? CustomCharacter { get; }
    /// <summary>
    /// Construct an in-game character.
    /// </summary>
    /// <param name="character">Character type.</param>
    public RDCharacter(Characters character)
    {
        Character = character;
        CustomCharacter = null;
    }
    /// <summary>
    /// Construct a customized character.
    /// </summary>
    /// <param name="character">A sprite.</param>
    public RDCharacter(string character)
    {
        Character = Characters.Custom;
        CustomCharacter = character;
    }
    internal IEnumerable<FileReference> GetAllPossibleFileReferences()
    {
        if (IsCustom && CustomCharacter is string cc)
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
    public static implicit operator RDCharacter(Characters character) => new(character);
    /// <inheritdoc/>
    public static implicit operator RDCharacter(string character) => new(character);
    /// <inheritdoc/>
    public static bool operator ==(RDCharacter left, RDCharacter right) => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(RDCharacter left, RDCharacter right) => !(left == right);
    /// <inheritdoc/>
    public readonly override string ToString() => (IsCustom
        ? CustomCharacter
        : Character.ToString())
        ?? "[Null]";
    /// <inheritdoc/>
    public bool Equals(RDCharacter other) => Character == other.Character
            && CustomCharacter == other.CustomCharacter;
    /// <inheritdoc/>
    public override int GetHashCode()
    {
#if NET7_0_OR_GREATER
		return HashCode.Combine(IsCustom, Character, CustomCharacter);
#else
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + IsCustom.GetHashCode();
            hash = hash * 23 + Character.GetHashCode();
            hash = hash * 23 + (CustomCharacter != null ? CustomCharacter.GetHashCode() : 0);
            return hash;
        }
#endif
    }
    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is RDCharacter e && Equals(e);
    }
}
