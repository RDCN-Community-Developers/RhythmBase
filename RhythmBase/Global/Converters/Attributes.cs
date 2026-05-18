#pragma warning disable CS9113
namespace RhythmBase.Global.Converters;


[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonEnumSerializableAttribute(bool pascalCase = true) : Attribute { }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonObjectSerializableAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonObjectHasSerializerAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonObjectNotSerializableAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonIgnoreAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonNotIgnoreAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonConditionAttribute(string condition) : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonAliasAttribute(string name) : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonDefaultSerializerAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonTimeAttribute(RDJsonTimeType type) : Attribute { }
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonConverterAttribute(Type converterType) : Attribute { }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
internal sealed class RDJsonSpecialIDAttribute(string id) : Attribute { }
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class RDJsonConverterForAttribute(Type targetType) : Attribute { }
internal enum RDJsonTimeType
{
    Milliseconds,
    Seconds,
    Beats,
}