using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Serialization;

/// <summary>
/// Options that carry additional metadata alongside standard <see cref="JsonSerializerOptions"/> for level serialization.
/// </summary>
public record class MetadataJsonSerializerOptions
{
	/// <summary>
	/// Gets or sets the underlying <see cref="JsonSerializerOptions"/> used for JSON serialization.
	/// </summary>
	public required JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		WriteIndented = true,
	};
	/// <summary>
	/// Gets a value indicating whether array elements should be aligned vertically when writing.
	/// </summary>
	public bool WriteAligned { get; init; }
	/// <summary>
	/// Gets or sets a value indicating whether the JSON output should be indented.
	/// </summary>
	public bool WriteIndented { get => JsonSerializerOptions.WriteIndented; init => JsonSerializerOptions.WriteIndented = value; }
	/// <summary>
	/// Gets the JSON deserialization strictness level.
	/// </summary>
	public JsonStrictness Strictness { get; init; } = JsonStrictness.Strict;
	/// <summary>
	/// Gets or sets the target format version for serialization.
	/// </summary>
	public int Version { get; set; } = 0;
	/// <summary>
	/// Gets or sets whether to upgrade legacy fields during deserialization.
	/// </summary>
	public bool UpgradeToLatest { get; set; } = true;

	/// <summary>
	/// Gets or sets sub-project-specific state preserved across a deserialization, used by upgraders.
	/// </summary>
	public JsonUpgradeState? UpgradeState { get; set; }

	/// <summary>
	/// Gets or sets the amount added to condition numeric indices when writing, so that the smallest written
	/// condition id is 1 (the game convention) regardless of the source base.
	/// </summary>
	public int ConditionIdOffset { get; set; }

	/// <summary>
	/// Gets or sets the directory used to resolve referenced asset files during read/write operations.
	/// </summary>
	public string? DirectoryName { get; set; }

	/// <summary>
	/// Gets or sets the level read settings associated with this serialization operation.
	/// </summary>
	public LevelReadConfig? ReadSettings { get; set; }

	/// <summary>
	/// Gets or sets the level write settings associated with this serialization operation.
	/// </summary>
	public LevelWriteConfig? WriteSettings { get; set; }

	private readonly List<(Type matchType, string field, Func<object, JsonElement, bool> handler)> _userHandlers = new();

	/// <summary>
	/// Registers a user-level handler for a specific unhandled field on a specific type.
	/// The handler is wrapped at registration time to accept <see cref="object"/>,
	/// enabling interface-based dispatch without runtime reflection.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="fieldName">The JSON property name to handle.</param>
	/// <param name="handler">The handler to invoke when the field is encountered.</param>
	public void RegisterHandler<T>(string fieldName, UnhandledPropertyHandler<T> handler)
	{
		Func<object, JsonElement, bool> wrapped = (object target, JsonElement value) =>
		{
			if (target is T typed)
				return handler(ref typed, value);
			return false;
		};
		_userHandlers.Add((typeof(T), fieldName, wrapped));
	}

	/// <summary>
	/// Attempts to invoke the user-registered handler for the given type, field name, and enum value.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="target">Reference to the object.</param>
	/// <param name="fieldName">The JSON property name.</param>
	/// <param name="value">The parsed JSON value.</param>
	/// <param name="enumValue">The event's enum value as <see cref="int"/>.</param>
	/// <returns><c>true</c> if a handler was found and returned <c>true</c>; otherwise <c>false</c>.</returns>
	public bool TryHandleUser<T>(ref T target, string fieldName, JsonElement value, int enumValue)
	{
		var factory = UnhandledFieldRegistry.PredicateFactory;
		foreach (var (matchType, field, handler) in _userHandlers)
		{
			if (field == fieldName && factory?.Invoke(matchType)?.Invoke(enumValue) == true)
			{
				return handler(target!, value);
			}
		}
		return false;
	}

	/// <summary>
	/// Copies user handlers from the specified settings instance.
	/// </summary>
	internal void CopyUserHandlersFrom(LevelReadConfig settings)
	{
		settings.CopyToUserHandlers(_userHandlers);
	}
}
/// <summary>
/// Specifies how strictly JSON deserialization should enforce format rules.
/// </summary>
public enum JsonStrictness
{
	/// <summary>
	/// Strict mode: data must exactly match the model definition.
	/// Any type mismatch, value out of range, or missing required field throws an exception.
	/// </summary>
	Strict,

	/// <summary>
	/// Corrective mode: attempts to coerce out-of-range or mismatched values into the valid range of the model.
	/// For example, clamping numeric values to valid intervals, trimming strings to max length,
	/// or mapping invalid enum values to the nearest valid member.
	/// Falls back to <see cref="Fallback"/> behavior when correction is not possible.
	/// </summary>
	Corrective,

	/// <summary>
	/// Fallback mode: ignores mismatched data and uses the type's default value as the parsing result.
	/// No exceptions are thrown; deserialization always succeeds.
	/// </summary>
	Fallback,
}
