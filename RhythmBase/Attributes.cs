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
/// <param name="objectType">The type to register.</param>
/// <param name="converterType">The converter type for the specified object type.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class JsonConverterLinkAttribute(Type objectType, Type converterType) : Attribute { }
/// <summary>
/// Declares the event type system for a game adapter, linking an event interface, an enum type,
/// a converter base class, and the enum property name. Used by the source generator to produce
/// <c>EventConverterMap</c>, <c>EventTypeRegistry</c>, and individual event converters.
/// </summary>
/// <param name="interfaceType">The event interface (e.g. <c>IBaseEvent</c>).</param>
/// <param name="typeEnumType">The enum type representing event categories (e.g. <c>EventType</c>).</param>
/// <param name="converterBaseType">The open-generic converter base (e.g. <c>MemberConverter&lt;&gt;</c>).</param>
/// <param name="typeEnumPropertyName">The property on the event interface that returns the enum value.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class JsonConverterSourceTypeAttribute(
    Type interfaceType,
    Type typeEnumType,
    Type converterBaseType,
    string typeEnumPropertyName
    ) : Attribute
{ }
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
[Flags]
public enum JsonFlattenMode
{
    /// <summary>Do not flatten; serialize nested objects as JSON objects.</summary>
    None,
    ReadOnly = 1,
    WriteOnly = 2,
    ReadWrite = ReadOnly | WriteOnly,
}