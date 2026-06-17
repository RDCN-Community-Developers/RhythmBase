using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Global.Converters;

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
	public JsonStrictness Strictness { get; init; } = JsonStrictness.Strict;
	public int Version { get; set; } = 0;
	public bool UpgradeToLatest { get; set; } = true;

	private readonly List<(Type matchType, string field, Delegate handler)> _userHandlers = new();
	private static readonly ConcurrentDictionary<Type, Func<Delegate, object, string, JsonElement, bool>> _dispatchCache = new();

	/// <summary>
	/// Registers a user-level handler for a specific unhandled field on a specific type.
	/// User handlers run after developer handlers (registered via <see cref="UnhandledFieldRegistry"/>).
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="fieldName">The JSON property name to handle.</param>
	/// <param name="handler">The handler to invoke when the field is encountered.</param>
	public void RegisterHandler<T>(string fieldName, UnhandledPropertyHandler<T> handler)
	{
		_userHandlers.Add((typeof(T), fieldName, handler));
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
				if (matchType == typeof(T))
					return ((UnhandledPropertyHandler<T>)handler)(ref target, fieldName, value);
				else
					return GetDispatcher(matchType).Invoke(handler, target!, fieldName, value);
			}
		}
		return false;
	}

	private static Func<Delegate, object, string, JsonElement, bool> GetDispatcher(Type registeredType)
	{
		return _dispatchCache.GetOrAdd(registeredType, t =>
		{
			var method = typeof(MetadataJsonSerializerOptions)
				.GetMethod(nameof(InvokeViaInterface), BindingFlags.NonPublic | BindingFlags.Static)!
				.MakeGenericMethod(t);
			return (Func<Delegate, object, string, JsonElement, bool>)
				method.CreateDelegate(typeof(Func<Delegate, object, string, JsonElement, bool>));
		});
	}

	private static bool InvokeViaInterface<TRegistered>(Delegate handler, object target, string fieldName, JsonElement value)
	{
		TRegistered iface = (TRegistered)target;
		return ((UnhandledPropertyHandler<TRegistered>)handler)(ref iface, fieldName, value);
	}

	/// <summary>
	/// Copies user handlers from the specified settings instance.
	/// </summary>
	internal void CopyUserHandlersFrom(LevelReadSettings settings)
	{
		settings.CopyToUserHandlers(_userHandlers);
	}
}
public enum JsonStrictness
{
	Strict,
	Relaxed,
}