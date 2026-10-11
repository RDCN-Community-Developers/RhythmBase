#pragma warning disable CS9113

namespace RhythmBase;


/// <summary>
/// Marks an enum type for string-based JSON serialization by the source generator.
/// </summary>
/// <param name="pascalCase">If <c>true</c>, enum member names are serialized in PascalCase; otherwise, camelCase.</param>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public sealed class JsonEnumSerializableAttribute(bool pascalCase = true) : Attribute { }
/// <summary>
/// Marks a class or struct for automatic JSON converter generation by the source generator.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class JsonObjectSerializableAttribute : Attribute { }
/// <summary>
/// Marks a class or struct as having a hand-written JSON converter, while still including it in the
/// source generator's type-enum mapping.
/// </summary>
/// <param name="serializerType">The type of the hand-written converter.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class JsonObjectHasSerializerAttribute(Type serializerType) : Attribute { }
/// <summary>
/// Marks a class or struct to be excluded from automatic JSON converter generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class JsonObjectNotSerializableAttribute : Attribute { }
/// <summary>
/// Marks a class or struct as the fallback model for unknown event types during deserialization.
/// Only one type per game adapter may carry this attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class JsonObjectSerializationFallbackAttribute : Attribute { }
/// <summary>
/// Excludes a property from JSON serialization, overriding the default behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class JsonIgnoreAttribute : Attribute { }
/// <summary>
/// Flattens a nested JSON object's property into the parent object during serialization/deserialization.
/// </summary>
/// <param name="property">The property path to flatten (e.g. <c>"color"</c>).</param>
/// <param name="alias">Optional alias for the flattened property name.</param>
/// <param name="mode">Specifies whether flattening applies to read, write, or both.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public sealed class JsonFlattenAttribute(string property, string? alias = null, JsonFlattenMode mode = JsonFlattenMode.ReadWrite) : Attribute { }
/// <summary>
/// Specifies a conditional expression that determines whether a property should be serialized.
/// </summary>
/// <param name="condition">The condition expression. Use <c>$&amp;</c> for the current object, <c>$r</c> for read settings, <c>$w</c> for write settings.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class JsonConditionAttribute(string condition) : Attribute { }
/// <summary>
/// Specifies an alternative JSON property name for a field or property.
/// </summary>
/// <param name="name">The alias name to use in JSON.</param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class JsonAliasAttribute(string name) : Attribute { }
/// <summary>
/// Forces a property to use the default enum serializer (numeric) instead of the string-based serializer.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class JsonDefaultSerializerAttribute : Attribute { }
/// <summary>
/// Specifies how a <see cref="System.TimeSpan"/> property should be serialized (as milliseconds or seconds).
/// </summary>
/// <param name="type">The time unit to use during serialization.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class JsonTimeAttribute(JsonTimeType type) : Attribute { }
/// <summary>
/// Specifies a custom <see cref="System.Text.Json.Serialization.JsonConverter"/> to use for a property.
/// </summary>
/// <param name="converterType">The converter type.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class JsonConverterAttribute(Type converterType) : Attribute { }
/// <summary>
/// Applies a unit conversion between the JSON value and the model value of a property.
/// </summary>
/// <remarks>
/// <para>
/// On deserialization the model value is <c>raw * multiplier + addend</c>; on serialization the
/// JSON value is <c>(model - addend) / multiplier</c>. When a custom converter is also present, the
/// converter runs first (into a temporary) and the unit conversion is applied afterwards; on
/// serialization the unit conversion runs first and the converter writes the result.
/// </para>
/// <para>
/// On a complex type, pass the member name (for example <c>nameof(Point.X)</c>) to convert that
/// member; the attribute may be repeated for several members.
/// </para>
/// <para>
/// For non-linear conversions, reference a pair of static <c>float (float)</c> methods: the first
/// maps the raw value to the model value, the second reverses it.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed class JsonUnitAttribute : Attribute
{
	/// <summary>
	/// Converts a numeric value as <c>raw * multiplier + addend</c>.
	/// </summary>
	/// <param name="multiplier">The multiplier applied to the raw value.</param>
	/// <param name="addend">The offset added after multiplying.</param>
	public JsonUnitAttribute(float multiplier, float addend = 0)
	{
		Multiplier = multiplier;
		Addend = addend;
	}

	/// <summary>
	/// Converts a numeric value as <c>raw * multiplier + addend</c>, truncating to an integer.
	/// </summary>
	/// <param name="multiplier">The multiplier applied to the raw value.</param>
	/// <param name="addend">The offset added after multiplying.</param>
	public JsonUnitAttribute(int multiplier, int addend = 0)
	{
		Multiplier = multiplier;
		Addend = addend;
	}

	/// <summary>
	/// Converts a single member of a complex value, for example <c>[JsonUnit(nameof(Point.X), 19.2f, -960f)]</c>.
	/// </summary>
	/// <param name="member">The name of the member to convert.</param>
	/// <param name="multiplier">The multiplier applied to the raw member value.</param>
	/// <param name="addend">The offset added after multiplying.</param>
	public JsonUnitAttribute(string member, float multiplier, float addend = 0)
	{
		Member = member;
		Multiplier = multiplier;
		Addend = addend;
	}

	/// <summary>
	/// Converts a value through a pair of static <c>float (float)</c> methods.
	/// </summary>
	/// <param name="scale">The static method mapping the raw value to the model value.</param>
	/// <param name="reverse">The static method mapping the model value back to the raw value.</param>
	public JsonUnitAttribute(string scale, string reverse)
	{
		ScaleFunction = scale;
		ReverseFunction = reverse;
	}

	/// <summary>Gets the multiplier applied to the raw value.</summary>
	public float Multiplier { get; }

	/// <summary>Gets the offset added after multiplying.</summary>
	public float Addend { get; }

	/// <summary>Gets the member name to convert on a complex value, if any.</summary>
	public string? Member { get; }

	/// <summary>Gets the static method mapping the raw value to the model value, if any.</summary>
	public string? ScaleFunction { get; }

	/// <summary>Gets the static method mapping the model value back to the raw value, if any.</summary>
	public string? ReverseFunction { get; }
}
/// <summary>
/// Marks the valid inclusive range of a data model member, for example <c>[ValueRange&lt;float&gt;(0f, 100f)]</c>.
/// </summary>
/// <typeparam name="T">The numeric type of the range bounds.</typeparam>
/// <remarks>
/// <para>
/// This attribute is informational only: it is a marker describing the expected value range of the annotated
/// member and is not read or emitted by the serialization pipeline.
/// </para>
/// <para>
/// The bounds describe the member's model value, that is the value after any conversion applied by
/// <see cref="JsonUnitAttribute"/>.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public class ValueRangeAttribute<T> : Attribute where T : struct, IComparable<T>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ValueRangeAttribute{T}"/> class.
	/// </summary>
	/// <param name="min">The minimum value of the range.</param>
	/// <param name="max">The maximum value of the range.</param>
	/// <param name="unit">The unit of the value range; empty when unspecified.</param>
	public ValueRangeAttribute(T min, T max, string unit = "")
	{
		Min = min;
		Max = max;
		Unit = unit;
	}
	/// <summary>Gets the minimum value of the range.</summary>
	public T Min { get; }
	/// <summary>Gets the maximum value of the range.</summary>
	public T Max { get; }
	/// <summary>Gets the unit of the range, if any (for example "px" or "ms").</summary>
	public string Unit { get; }
}
/// <summary>
/// Registers a hand-written converter as the converter for the specified target type in the
/// generated <c>TypeConverterRegistry</c>.
/// </summary>
/// <param name="targetType">The type this converter handles.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class JsonConverterForAttribute(Type targetType) : Attribute { }
/// <summary>
/// Declares the namespace identifier for a game adapter, used by the source generator to organize
/// generated converter code.
/// </summary>
/// <param name="namespaceId">The namespace identifier (e.g. "RhythmDoctor", "Adofai").</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class JsonConverterIdAttribute(string namespaceId) : Attribute { }
/// <summary>
/// Links a public type to a custom converter in the generated <c>TypeConverterRegistry</c>.
/// </summary>
/// <typeparam name="TObject">The type to register.</typeparam>
/// <typeparam name="TConverter">The converter type for the specified object type. Must derive from
/// <see cref="System.Text.Json.Serialization.JsonConverter"/>.</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class JsonConverterLinkAttribute<TObject, TConverter> : Attribute
	where TConverter : System.Text.Json.Serialization.JsonConverter
{ }
/// <summary>
/// Declares the event type system for a game adapter, linking an event interface, its category enum,
/// a converter base class, and the enum property name. Used by the source generator to produce
/// <c>EventConverterMap</c>, <c>EventTypeRegistry</c>, and individual event converters.
/// </summary>
/// <typeparam name="TInterface">The event interface (e.g. <c>IBaseEvent</c>).</typeparam>
/// <typeparam name="TEnum">The enum type representing event categories (e.g. <c>EventType</c>).</typeparam>
/// <param name="converterBaseType">The open-generic converter base (e.g. <c>MemberConverter&lt;&gt;</c>).</param>
/// <param name="typeEnumPropertyName">The property on the event interface that returns the enum value.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class JsonConverterSourceTypeAttribute<TInterface, TEnum>(Type converterBaseType, string typeEnumPropertyName) : Attribute
	where TInterface : class
	where TEnum : struct, Enum
{ }
/// <summary>
/// TickTime generation attribute for a game adapter, specifying the chart type, level type, calculator type,
/// tick time type, event enum type, and event interface type.
/// </summary>
/// <typeparam name="TChart">The chart type. Must implement <see cref="IChart"/>.</typeparam>
/// <typeparam name="TLevel">The level type. Must implement <see cref="ILevel"/>.</typeparam>
/// <typeparam name="TCalculator">The calculator type.</typeparam>
/// <typeparam name="TTickTime">The tick time type. Must implement <see cref="ITickTime{TBeat}"/>.</typeparam>
/// <typeparam name="TEnum">The enum type representing event categories (e.g. <c>EventType</c>).</typeparam>
/// <typeparam name="TEventInterface">The event interface (e.g. <c>IBaseEvent</c>).</typeparam>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class AdapterTypeAttribute<TChart, TLevel, TCalculator, TTickTime, TEnum, TEventInterface> : Attribute
	where TChart : Global.Components.IChart
	where TLevel : Global.Components.ILevel
	where TCalculator : class
	where TTickTime : struct, Global.Components.ITickTime<TTickTime>
	where TEnum : struct, Enum
	where TEventInterface : class
{ }
/// <summary>
/// Specifies that a given enum type should be serialized as a string in JSON in new package, with an option for PascalCase or camelCase.
/// </summary>
/// <param name="enumType"></param>
/// <param name="pascalCase"></param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class JsonEnumCastingAttribute(Type enumType, bool pascalCase = true) : Attribute { }
/// <summary>
/// Specifies the unit of time used when serializing a <see cref="System.TimeSpan"/> property.
/// </summary>
public enum JsonTimeType
{
	/// <summary>Serialize as milliseconds (integer).</summary>
	Milliseconds,
	/// <summary>Serialize as seconds (floating-point).</summary>
	Seconds,
}
/// <summary>
/// Specifies when JSON property flattening should be applied.
/// </summary>
[Flags]
public enum JsonFlattenMode
{
	/// <summary>Do not flatten; serialize nested objects as JSON objects.</summary>
	None,
	/// <summary>Flatten only during deserialization (reading).</summary>
	ReadOnly = 1,
	/// <summary>Flatten only during serialization (writing).</summary>
	WriteOnly = 2,
	/// <summary>Flatten during both serialization and deserialization.</summary>
	ReadWrite = ReadOnly | WriteOnly,
}