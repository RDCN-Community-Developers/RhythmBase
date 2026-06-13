namespace RhythmBase.Generator;

partial class ConverterGenerator
{
	// Attribute 名称常量
	private const string JsonEnumAttrName = "RhythmBase.JsonEnumSerializableAttribute";
	private const string JsonObjectSerializableAttrName = "RhythmBase.JsonObjectSerializableAttribute";
	private const string JsonObjectHasSerializerAttrName = "RhythmBase.JsonObjectHasSerializerAttribute";
	private const string JsonObjectNotSerializableAttrName = "RhythmBase.JsonObjectNotSerializableAttribute";
	private const string JsonObjectSerializationFallbackAttrName = "RhythmBase.JsonObjectSerializationFallbackAttribute";
	private const string JsonAliasAttrName = "RhythmBase.JsonAliasAttribute";
	private const string JsonIgnoreAttrName = "RhythmBase.JsonIgnoreAttribute";
	private const string JsonDefaultSerializerAttrName = "RhythmBase.JsonDefaultSerializerAttribute";
	private const string JsonConditionAttrName = "RhythmBase.JsonConditionAttribute";
	private const string JsonTimeAttrName = "RhythmBase.JsonTimeAttribute";
	private const string JsonConverterAttrName = "RhythmBase.JsonConverterAttribute";
	private const string JsonConverterForAttrName = "RhythmBase.JsonConverterForAttribute";
	private const string JsonConverterLinkAttrName = "RhythmBase.JsonConverterLinkAttribute";
	private const string JsonConverterIdAttrName = "RhythmBase.JsonConverterIdAttribute";
	private const string JsonConverterSourceTypeAttrName = "RhythmBase.JsonConverterSourceTypeAttribute";
	private const string JsonFlattenAttrName = "RhythmBase.JsonFlattenAttribute";

	// Type 类型
	private const string IEventTypeName = "RhythmBase.Global.Events.IEvent";
	private const string MemberConverterTypeName = "RhythmBase.Global.Converters.MemberConverter`1";
	private const string MetadataJsonConverterTypeName = "RhythmBase.Global.Converters.MetadataJsonConverter`1";

	//class SymbolResults
	//{
	//    public INamedTypeSymbol JsonEnumAttr;
	//    public INamedTypeSymbol JsonObjectSerializableAttr;
	//    public INamedTypeSymbol JsonObjectHasSerializerAttr;
	//    public INamedTypeSymbol JsonObjectNotSerializableAttr;
	//    public INamedTypeSymbol JsonAliasAttr;
	//    public INamedTypeSymbol JsonIgnoreAttr;
	//    public INamedTypeSymbol JsonNotIgnoreAttr;
	//    public INamedTypeSymbol JsonDefaultSerializerAttr;
	//    public INamedTypeSymbol JsonConditionAttr;
	//    public INamedTypeSymbol JsonTimeAttr;
	//    public INamedTypeSymbol JsonConverterAttr;
	//    public INamedTypeSymbol JsonConverterForAttr;
	//    public INamedTypeSymbol JsonConverterIdAttr;
	//    public INamedTypeSymbol JsonConverterSourceTypeAttr;

	//    public INamedTypeSymbol IEventType;
	//    public SymbolResults(Compilation compilation)
	//    {
	//        JsonEnumAttr = compilation.GetTypeByMetadataName(JsonEnumAttrName);
	//        JsonObjectSerializableAttr = compilation.GetTypeByMetadataName(JsonObjectSerializableAttrName);
	//        JsonObjectHasSerializerAttr = compilation.GetTypeByMetadataName(JsonObjectHasSerializerAttrName);
	//        JsonObjectNotSerializableAttr = compilation.GetTypeByMetadataName(JsonObjectNotSerializableAttrName);
	//        JsonAliasAttr = compilation.GetTypeByMetadataName(JsonAliasAttrName);
	//        JsonIgnoreAttr = compilation.GetTypeByMetadataName(JsonIgnoreAttrName);
	//        JsonNotIgnoreAttr = compilation.GetTypeByMetadataName(JsonNotIgnoreAttrName);
	//        JsonDefaultSerializerAttr = compilation.GetTypeByMetadataName(JsonDefaultSerializerAttrName);
	//        JsonConditionAttr = compilation.GetTypeByMetadataName(JsonConditionAttrName);
	//        JsonTimeAttr = compilation.GetTypeByMetadataName(JsonTimeAttrName);
	//        JsonConverterAttr = compilation.GetTypeByMetadataName(JsonConverterAttrName);
	//        JsonConverterForAttr = compilation.GetTypeByMetadataName(JsonConverterForAttrName);
	//        JsonConverterIdAttr = compilation.GetTypeByMetadataName(JsonConverterIdAttrName);
	//        JsonConverterSourceTypeAttr = compilation.GetTypeByMetadataName(JsonConverterSourceTypeAttrName);

	//        IEventType = compilation.GetTypeByMetadataName(IEventTypeName);
	//    }
	//}
}
