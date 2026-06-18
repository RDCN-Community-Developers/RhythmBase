using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RhythmBase.Global.Converters;

/// <summary>
/// Handles an unhandled JSON property during deserialization.
/// </summary>
/// <typeparam name="TTarget">The concrete type of the object being deserialized.</typeparam>
/// <param name="target">Reference to the object being deserialized. Can be modified directly.</param>
/// <param name="fieldName">The JSON property name.</param>
/// <param name="value">The parsed JSON value.</param>
/// <returns><c>true</c> if the property was handled; <c>false</c> to pass it to the next handler or store it in extra data.</returns>
public delegate bool UnhandledPropertyHandler<TTarget>(ref TTarget target, JsonElement value);

/// <summary>
/// Static registry for developer-registered unhandled property handlers.
/// Handlers are registered per type and per field name, with an enum-based predicate for efficient matching.
/// When a <see cref="MemberConverter{TEvent}"/> encounters a property that is not recognized by its
/// <c>Read</c> override, it checks this registry before falling back to storing the value in extra data.
/// </summary>
public static class UnhandledFieldRegistry
{
	private static readonly List<(Type matchType, string field, Delegate handler)> _handlers = new();
	private static Func<Type, Func<int, bool>>? _predicateFactory;

	/// <summary>
	/// Gets the configured predicate factory, if any.
	/// Used by <see cref="Components.LevelReadSettings"/> to compute matching predicates for user-level handlers.
	/// </summary>
	internal static Func<Type, Func<int, bool>>? PredicateFactory => _predicateFactory;

	/// <summary>
	/// Configures the predicate factory used to compute enum-based matching predicates.
	/// Must be called once during package initialization before any dispatch occurs.
	/// Registration (<see cref="Register{T}"/>) can happen before configuration — predicates
	/// are resolved lazily at dispatch time.
	/// </summary>
	/// <param name="predicateFactory">
	/// A function that, given a <see cref="Type"/>, returns a predicate that checks
	/// whether an event's enum value belongs to the set of types matching the given type.
	/// </param>
	public static void Configure(Func<Type, Func<int, bool>> predicateFactory)
		=> _predicateFactory = predicateFactory;

	/// <summary>
	/// Registers a handler for a specific field on a specific type.
	/// The matching predicate is resolved lazily at dispatch time using the configured factory.
	/// </summary>
	/// <typeparam name="T">The object type whose properties are being handled.</typeparam>
	/// <param name="fieldName">The JSON property name to handle.</param>
	/// <param name="handler">The handler to invoke when the field is encountered.</param>
	/// <param name="predicateType">
	/// Optional. The type used to compute the matching predicate. When registering for an interface
	/// (e.g., <c>ITintEvent</c>), pass the interface type here so the predicate matches all concrete
	/// types that implement it. When omitted, defaults to <typeparamref name="T"/>.
	/// </param>
	public static void Register<T>(string fieldName, UnhandledPropertyHandler<T> handler, Type? predicateType = null)
	{
		_handlers.Add((predicateType ?? typeof(T), fieldName, handler));
	}

	/// <summary>
	/// Registers a handler that ignores (silently skips) a specific field on a specific type.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="fieldName">The JSON property name to ignore.</param>
	public static void Ignore<T>(string fieldName)
		=> Register<T>(fieldName, (ref T _, JsonElement __) => true);

	/// <summary>
	/// Attempts to invoke the registered handler for the given type, field name, and enum value.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="target">Reference to the object.</param>
	/// <param name="fieldName">The JSON property name.</param>
	/// <param name="value">The parsed JSON value.</param>
	/// <param name="enumValue">The event's enum value as <see cref="int"/>.</param>
	/// <returns><c>true</c> if a handler was found and returned <c>true</c>; otherwise <c>false</c>.</returns>
	public static bool TryHandle<T>(ref T target, string fieldName, JsonElement value, int enumValue)
	{
		foreach (var (matchType, field, handler) in _handlers)
		{
			if (field == fieldName && Matches(matchType, enumValue))
			{
				return ((UnhandledPropertyHandler<T>)handler)(ref target, value);
			}
		}
		return false;
	}

	private static bool Matches(Type matchType, int enumValue)
	{
		return _predicateFactory?.Invoke(matchType)?.Invoke(enumValue) == true;
	}
}
