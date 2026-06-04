#pragma warning disable CS9113

namespace RhythmBase;


[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public sealed class JsonEnumSerializableAttribute(bool pascalCase = true) : Attribute { }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class JsonObjectSerializableAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class JsonObjectHasSerializerAttribute(Type serializerType) : Attribute { }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class JsonObjectNotSerializableAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class JsonObjectSerializationFallbackAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class JsonIgnoreAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class JsonConditionAttribute(string condition) : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class JsonAliasAttribute(string name) : Attribute { }
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class JsonDefaultSerializerAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class JsonTimeAttribute(JsonTimeType type) : Attribute { }
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class JsonConverterAttribute(Type converterType) : Attribute { }
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class JsonConverterForAttribute(Type targetType) : Attribute { }
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class JsonConverterIdAttribute(string namespaceId) : Attribute { }
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class JsonConverterLinkAttribute(Type objectType, Type converterType) : Attribute { }
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class JsonConverterSourceTypeAttribute(
    Type interfaceType,
    Type typeEnumType,
    Type converterBaseType,
    string typeEnumPropertyName
    ) : Attribute
{ }
public enum JsonTimeType
{
    Milliseconds,
    Seconds,
}