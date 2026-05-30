using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Text;

// 这坨写得太史了

namespace RhythmBase.Generator;

public record struct RegistryInfo(
		string NamespaceId,
		ITypeSymbol Type
		);

[Generator(LanguageNames.CSharp)]
public partial class ConverterGenerator : IIncrementalGenerator
{
	private static readonly DiagnosticDescriptor InvalidConverterRegistrationRule = new(
		"RD0001",
		"Invalid converter registration",
		"Converter '{0}' has invalid RDJsonConverterFor registration: {1}",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	// 重复注册同一目标类型的转换器
	private static readonly DiagnosticDescriptor DuplicateConverterRegistrationRule = new(
		"RD0002",
		"Duplicate converter registration",
		"Target type '{0}' is registered by multiple converters: {1}",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	// 类型不是枚举
	private static readonly DiagnosticDescriptor InvalidEnumTypeRule = new(
		"RD0003",
		"Invalid enum type",
		"Type '{0}' is not an enum type",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	// 表示类型的属性需要初始值
	private static readonly DiagnosticDescriptor MissingEnumInitializerRule = new(
		"RD0004",
		"Missing enum initializer",
		"Property '{0}' in type '{1}' must have an initializer with an enum member value",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	// 需要一个用于回退的数据模型
	private static readonly DiagnosticDescriptor MissingFallbackModelRule = new(
		"RD0005",
		"Missing fallback model",
		"Converter '{0}' must have a fallback model for unknown types, but no suitable type was found",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	// 回退数据模型必须具有无参构造函数
	private static readonly DiagnosticDescriptor FallbackModelMissingParameterlessCtorRule = new(
		"RD0006",
		"Fallback model missing parameterless constructor",
		"Fallback model '{0}' for converter '{1}' must have an accessible parameterless constructor",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	// 回退数据模型只能有 1 个
	private static readonly DiagnosticDescriptor MultipleFallbackModelsRule = new(
		"RD0007",
		"Multiple fallback models",
		"Converter '{0}' has multiple fallback models: {1}",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	// 需要为合法的命名空间名
	private static readonly DiagnosticDescriptor InvalidNamespaceIdRule = new(
		"RD0008",
		"Invalid namespace ID",
		"Namespace ID '{0}' is invalid. It must be a non-empty string consisting of letters, digits, or underscores.",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	// 类型没有注册 Attribute
	private static readonly DiagnosticDescriptor MissingTypeRegistrationRule = new(
		"RD0009",
		"Missing type registration",
		"Type '{0}' does not have a valid RhythmBase.JsonObjectSerializableAttribute or RhythmBase.JsonObjectHasSerializerAttribute registration",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	// 父类型的转换器不是泛型类型
	private static readonly DiagnosticDescriptor NonGenericBaseConverterRule = new(
		"RD0010",
		"Non-generic base converter",
		"Base converter '{0}' for converter '{1}' must be a generic type with one type parameter",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	private static ISymbol? GetEnumInitializerValue(IPropertySymbol propertySymbol, Compilation compilation)
	{
		var syntaxRefs = propertySymbol.DeclaringSyntaxReferences;
		if (syntaxRefs.Length == 0) return null;
		var syntax = syntaxRefs[0].GetSyntax() as PropertyDeclarationSyntax;
		if (syntax == null) return null;
		var model = compilation.GetSemanticModel(syntax.SyntaxTree);
		MemberAccessExpressionSyntax memSyntax;
		if (syntax.ExpressionBody?.Expression is MemberAccessExpressionSyntax memberAccess)
			memSyntax = memberAccess;
		else if (syntax.Initializer?.Value is MemberAccessExpressionSyntax initAccess)
			memSyntax = initAccess;
		else return null;
		return compilation.GetSemanticModel(memSyntax.SyntaxTree).GetSymbolInfo(memSyntax).Symbol;
	}
	public struct PropertyGenerateConverterInfo
	{
		public IPropertySymbol Property;
	}
	public interface IClassGen
	{
		public INamedTypeSymbol ICGClassType { get; }
	}
	public struct ClassGenCvtrInfo : IClassGen
	{
		public INamedTypeSymbol ClassType;
		public ISymbol ClassTypeEnum;
		public PropertyGenerateConverterInfo[] Properties;
		public INamedTypeSymbol ICGClassType => ClassType;
	}
	public struct ClassRefConverterInfo : IClassGen
	{
		public INamedTypeSymbol ClassType;
		public ISymbol? ClassTypeEnum;
		public INamedTypeSymbol ConverterType;
		public INamedTypeSymbol ICGClassType => ClassType;
	}
	public struct ClassNoInfo : IClassGen
	{
		public INamedTypeSymbol ClassType;
		public INamedTypeSymbol ICGClassType => ClassType;
	}
	public struct ClassGenAttr
	{
		public INamedTypeSymbol RootType;
		public INamedTypeSymbol EnumType;
		public INamedTypeSymbol RootConverterType;
		public string TypePropertyName;
	}
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var errors = new HashSet<Diagnostic>();
		IncrementalValueProvider<string?> registryInfo = context.CompilationProvider // checked
				.Select((compilation, ct) =>
		{
			var attrType = compilation.GetTypeByMetadataName(JsonConverterIdAttrName);
			if (attrType == null) return default;
			var assembly = compilation.Assembly;
			foreach (var attr in assembly.GetAttributes())
			{
				if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attrType))
				{
					static bool IsValidNamespaceId(string? id)
					{
						return !string.IsNullOrEmpty(id)
								&& id[0] is not '.'
								&& !char.IsDigit(id[0])
								&& id.All(c => char.IsLetterOrDigit(c) || c is '_' or '.');
					}

					var args = attr.ConstructorArguments;
					string namespaceId = args[0].Value as string ?? "";

					if (!IsValidNamespaceId(namespaceId))
					{
						errors.Add(Diagnostic.Create(InvalidNamespaceIdRule, attr.ApplicationSyntaxReference?.GetSyntax().GetLocation(), namespaceId));
					}
					return namespaceId;
				}
			}
			return default;
		});
		var classEnumAttrPairInfo = context.CompilationProvider // checked
				.Select((compilation, ct) =>
				{
					var attrType = compilation.GetTypeByMetadataName(JsonConverterSourceTypeAttrName);
					if (attrType == null) return default;
					var assembly = compilation.Assembly;
					List<ClassGenAttr> types = [];
					foreach (var attr in assembly.GetAttributes())
					{
						if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attrType))
						{
							var args = attr.ConstructorArguments;
							var type = args[0].Value as INamedTypeSymbol;
							if (type == null) return default;
							if (args[1].Value is not INamedTypeSymbol enumType || enumType.TypeKind != TypeKind.Enum)
							{
								errors.Add(Diagnostic.Create(InvalidEnumTypeRule, type?.Locations.FirstOrDefault(), args[1].Value?.ToString() ?? "null"));
								continue;
							}
							if (args[2].Value is not INamedTypeSymbol converterBaseType)
							{
								continue;
							}
							if (args[3].Value is not string enumPropertyName) return default;
							//types.Add((type, enumType, converterBaseType, enumPropertyName));
							types.Add(new()
							{
								RootType = type,
								EnumType = enumType,
								RootConverterType = converterBaseType,
								TypePropertyName = enumPropertyName,
							});
						}
					}
					return (compilation, types.ToArray(), errors);
				});
		var allasms = context.CompilationProvider.Select((compilation, ct) =>
		{
			var assemblies = compilation.References.Select(i => compilation.GetAssemblyOrModuleSymbol(i)).OfType<IAssemblySymbol>().Concat(new[] { compilation.Assembly });
			return assemblies.ToArray();
		});
		IncrementalValueProvider<ClassEnumMapGenerationInfo[]> classEnumMapInfo = classEnumAttrPairInfo // checked
				.Select((input, ct) =>
		{
			(
					Compilation compilation,
					ClassGenAttr[] types,
					var errors
					) = input;
			var eventTypeInterface = compilation.GetTypeByMetadataName(IEventTypeName);
			var fallbackAttr = compilation.GetTypeByMetadataName(JsonObjectSerializationFallbackAttrName);
			List<ClassEnumMapGenerationInfo> typesToGenerate = [];
			foreach (var classGen in types)
			{
				INamedTypeSymbol baseType = classGen.RootType;
				INamedTypeSymbol enumType = classGen.EnumType;
				string enumPropertyName = classGen.TypePropertyName;
				Dictionary<INamedTypeSymbol, HashSet<ISymbol>> dict = new(SymbolEqualityComparer.Default);
				if (baseType is null || enumType is null)
					continue;
				ISymbol[] enumMembers = enumType?.GetMembers().Where(m => m.Kind == SymbolKind.Field).ToArray() ?? [];
				ISymbol fallbackEnumMember = null;
				INamedTypeSymbol[] subTypes = [.. GetDerivedTypes(baseType, compilation)];
				INamedTypeSymbol[] subEventClasses = [.. subTypes.Where(t => t.TypeKind == TypeKind.Class && !t.IsAbstract)];
				IEnumerable<INamedTypeSymbol> fallbackTypes = subEventClasses.Where(c => c.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, fallbackAttr)));
				INamedTypeSymbol fallbackType = null;
				switch (fallbackTypes.Count())
				{
					case 0:
						errors.Add(Diagnostic.Create(MissingFallbackModelRule, baseType.Locations.FirstOrDefault(), baseType.ToDisplayString()));
						continue;
					case > 1:
						errors.Add(Diagnostic.Create(MultipleFallbackModelsRule, baseType.Locations.FirstOrDefault(), baseType.ToDisplayString(), string.Join(", ", fallbackTypes.Select(i => i.ToDisplayString()))));
						continue;
				}
				fallbackType = fallbackTypes.FirstOrDefault();
				subEventClasses = subEventClasses.Where(c =>
								!SymbolEqualityComparer.Default.Equals(c, fallbackType) &&
								!IsDerivedFrom(c, fallbackType)).ToArray();
				Dictionary<INamedTypeSymbol, ISymbol> classEnumMap = new(SymbolEqualityComparer.Default);
				foreach (var subClass in subEventClasses)
				{
					var property = subClass.GetMembers().FirstOrDefault(m => m.Kind == SymbolKind.Property && m.Name == enumPropertyName) as IPropertySymbol;
					ISymbol enumSymbol = null;
					if (SymbolEqualityComparer.Default.Equals((ITypeSymbol?)property.Type, enumType))
					{
						enumSymbol = GetEnumInitializerValue(property, compilation);
					}
					if (enumSymbol != null)
						classEnumMap[subClass] = enumSymbol;
					else errors.Add(Diagnostic.Create(MissingEnumInitializerRule, property.Type.Locations.FirstOrDefault(), property.Name, property.ContainingType?.ToDisplayString()));
				}
				{
					var property = fallbackType.GetMembers().FirstOrDefault(m => m.Kind == SymbolKind.Property && m.Name == enumPropertyName);
					if (property is IPropertySymbol propSymbol)
					{
						var typeOfEnum = propSymbol.Type;
						if (SymbolEqualityComparer.Default.Equals(typeOfEnum, enumType))
						{
							var enumSymbol = GetEnumInitializerValue(propSymbol, compilation);
							if (enumSymbol == null)
								errors.Add(Diagnostic.Create(MissingEnumInitializerRule, propSymbol.Locations.FirstOrDefault(), propSymbol.Name, propSymbol.ContainingType?.ToDisplayString()));
							else
								fallbackEnumMember = enumSymbol;
						}
					}
				}
				foreach (INamedTypeSymbol subType in GetDerivedTypes(eventTypeInterface, compilation, includeReferences: true)
								.Where(i => !i.IsGenericType)
						)
				{
					var subTypeSubEventClasses = subEventClasses
						.Where(c =>
							SymbolEqualityComparer.Default.Equals(c, subType) ||
							(subType.TypeKind == TypeKind.Interface
								? c.AllInterfaces.Contains(subType, SymbolEqualityComparer.Default)
								: IsDerivedFrom(c, subType)));
					if (dict.TryGetValue(subType, out var enumMap))
						enumMap.UnionWith(subTypeSubEventClasses.Where(c => classEnumMap.ContainsKey(c)).Select(c => classEnumMap[c]));
					else
						dict[subType] = new HashSet<ISymbol>(subTypeSubEventClasses.Where(c => classEnumMap.ContainsKey(c)).Select(c => classEnumMap[c]), SymbolEqualityComparer.Default);
				}
				typesToGenerate.Add(new()
				{
					ClassType = baseType,
					ClassTypeEnum = enumType,
					FallbackClassType = fallbackType,
					FallbackClassTypeEnum = fallbackEnumMember,
					Classes = subEventClasses,
					ClassEnumMap = dict,
					ClassEnumDoubleMap = classEnumMap,
				});
			}
			return typesToGenerate.ToArray();
		});
		IncrementalValueProvider<INamedTypeSymbol?> classConverterBaseInfo = context.CompilationProvider
				.Select((compilation, ct) => compilation.GetTypeByMetadataName(InstanceConverterTypeName));

		var enumNeedCvtr = context.CompilationProvider.Combine(allasms).Select((compilationAndAsms, ct) =>
		{
			var (compilation, asms) = compilationAndAsms;
			List<INamedTypeSymbol> enums = [];
			foreach (var asm in asms)
			{
				var types = GetAllTypes(asm.GlobalNamespace)
							.OfType<INamedTypeSymbol>()
							.Where(i => i.TypeKind == TypeKind.Enum && i.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == JsonEnumAttrName));
				enums.AddRange(types);
			}
			return (compilation, enums.ToArray());
		});
		//var jsonClassNeedConverterInfo = context.SyntaxProvider.ForAttributeWithMetadataName(JsonObjectSerializableAttrName,
		//    static (node, ct) => node is ClassDeclarationSyntax or StructDeclarationSyntax or RecordDeclarationSyntax,
		//    static (context, ct) =>
		//{
		//    if (context.TargetSymbol is not INamedTypeSymbol typeSymbol) return default;
		//    return typeSymbol;
		//}).Collect().Combine(classEnumAttrPairInfo)
		//.Select((classEnumPairInfoAndSymbols, ct) =>
		//{
		//    Exception? ex1 = null;
		//    List<ClassGenCvtrInfo> classGenerateConverterInfos = [];
		//    try
		//    {
		//        var (symbols, classEnumPairInfo) = classEnumPairInfoAndSymbols;
		//        foreach (var pair in classEnumPairInfo.Item2)
		//        {
		//            var (baseType, enumType, converterBaseType, enumPropertyName) = pair;
		//            IEnumerable<INamedTypeSymbol?> derivedTypes = symbols.Where(s =>
		//                IsDerivedFrom(s, baseType));
		//            foreach (var symbol in derivedTypes)
		//            {
		//                ClassGenCvtrInfo info;
		//                info = GetClassGenCvtrInfo(classEnumPairInfo, enumType, enumPropertyName, symbol);
		//                classGenerateConverterInfos.Add(info);
		//            }
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        ex1 = ex;
		//    }
		//    return (classGenerateConverterInfos.ToArray(), ex1);
		//});
		var jsonClassExistedConverterInfor = context.SyntaxProvider.ForAttributeWithMetadataName(JsonObjectHasSerializerAttrName,
			static (node, ct) => node is ClassDeclarationSyntax or StructDeclarationSyntax or RecordDeclarationSyntax,
			static (context, ct) =>
			{
				if (context.TargetSymbol is not INamedTypeSymbol typeSymbol) return default;
				return typeSymbol;
			}).Collect();

		var jsonClassCvtrMapGenInfo = classEnumAttrPairInfo
			.Select((classEnumAttrPairs, ct) =>
			{
				var (compilation, pairs, errors) = classEnumAttrPairs;
				var jsonSlz = compilation.GetTypeByMetadataName(JsonObjectSerializableAttrName);
				var jsonHasSlz = compilation.GetTypeByMetadataName(JsonObjectHasSerializerAttrName);
				List<(IClassGen[], ClassGenAttr)> result = [];
				foreach (var classEnumPair in pairs)
				{
					List<IClassGen> gens = [];
					ClassGenAttr cga = classEnumPair;
					var type = cga.RootType;
					var enumType = cga.EnumType;
					var enumPropertyName = cga.TypePropertyName;
					var subTypes = GetAllTypes(type.ContainingNamespace)
						.Where(
							t => IsDerivedFrom(t, type) &&
							t.TypeKind != TypeKind.Interface);
					foreach (var subType in subTypes)
					{
						var attrs = subType.GetAttributes();
						var jsonSlzSmp = attrs.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, jsonSlz));
						var jsonHasSlzSmp = attrs.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, jsonHasSlz));
						if (jsonSlzSmp is not null)
						{
							var classInfo = GetClassGenCvtrInfo(compilation, enumType, enumPropertyName, subType);
							gens.Add(classInfo);
						}
						else if (jsonHasSlzSmp is AttributeData jsonHasSlzAttr)
						{
							var arg0 = jsonHasSlzAttr.ConstructorArguments[0].Value as INamedTypeSymbol;
							ClassRefConverterInfo classInfo = new()
							{
								ClassType = subType,
								ClassTypeEnum = GetEnumTypeMember(compilation, enumType, enumPropertyName, subType),
								ConverterType = arg0,
							};
							gens.Add(classInfo);
						}
						else
						{
							ClassNoInfo classNoInfo = new()
							{
								ClassType = subType,
							};
							gens.Add(classNoInfo);
						}
					}
					result.Add((gens.ToArray(), cga));
				}
				return result.ToArray();
			});

		var jsonObjSlzForInfo = allasms.Combine(context.CompilationProvider).SelectMany((allasmsAndCompilation, ct) =>
		{
			var (allasms, compilation) = allasmsAndCompilation;
			List<(ITypeSymbol, INamedTypeSymbol)> result = new();
			var attrType = compilation.GetTypeByMetadataName(JsonConverterForAttrName);
			static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol namespaceSymbol)
			{
				foreach (var member in namespaceSymbol.GetMembers())
				{
					if (member is INamedTypeSymbol typeSymbol)
					{
						yield return typeSymbol;
					}
					else if (member is INamespaceSymbol nestedNamespace)
					{
						foreach (var nestedType in GetAllTypes(nestedNamespace))
						{
							yield return nestedType;
						}
					}
				}
			}
			foreach (var asm in allasms)
			{
				var types = GetAllTypes(asm.GlobalNamespace)
								.OfType<INamedTypeSymbol>()
								.Select(i => (i, i.GetAttributes().FirstOrDefault(j => j.AttributeClass.Equals(attrType, SymbolEqualityComparer.Default))))
								.Where(pair => pair.Item2 != null);
				foreach (var pair in types)
				{
					if (SymbolEqualityComparer.Default.Equals(pair.Item2.AttributeClass, attrType))
					{
						var arg0 = pair.Item2.ConstructorArguments[0].Value as ITypeSymbol;
						if (arg0 is INamedTypeSymbol namedType)
							result.Add((arg0, pair.i));
					}
				}
			}
			return result.ToArray();
		})
		.Collect();

		var jsonObjSlzLnkInfo = context.CompilationProvider.Combine(allasms).Select((compilationAndAsms, ct) =>
		{
			(Compilation compilation, IAssemblySymbol[] assemblies) = compilationAndAsms;
			var attrType = compilation.GetTypeByMetadataName(JsonConverterLinkAttrName);
			if (attrType == null) return default;
			List<(ITypeSymbol, INamedTypeSymbol)> types = [];
			foreach (var attr in assemblies.SelectMany(i => i.GetAttributes()))
			{
				if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attrType))
				{
					var args = attr.ConstructorArguments;
					var arg0 = args[0].Value as ITypeSymbol;
					var arg1 = args[1].Value as INamedTypeSymbol;
					types.Add((arg0, arg1));
				}
			}
			return types.ToArray();
		});
		var jsonObjSlzLnksInfo = jsonObjSlzForInfo.Combine(jsonObjSlzLnkInfo).Select((context, ct) =>
		{
			List<(ITypeSymbol, INamedTypeSymbol)> gens = [];
			foreach (var pair in context.Left)
				gens.Add((pair.Item1, pair.Item2));
			foreach (var pair in context.Right)
				gens.Add((pair.Item1, pair.Item2));
			return gens.ToArray();
		});

		//context.RegisterSourceOutput(registryInfo.Combine(jsonObjSlzLnksInfo), (context, input) =>
		//{
		//    var gens = input;
		//    StringBuilder sb = new();
		//    sb.AppendLine("/*");
		//    foreach (var info in gens.Right.OrderBy(i => i.Item1?.ToDisplayString()))
		//    {
		//        sb.AppendLine($"""
		//            {info.Item1?.ToDisplayString() ?? "null"}
		//             +->{info.Item2?.ToDisplayString() ?? "null"}

		//            """);
		//    }
		//    context.AddSource("test.g.cs", sb.AppendLine("*/").ToString());
		//});
		
		GenerateConverterHub(context, registryInfo.Combine(jsonObjSlzLnksInfo));
		GenerateConverter(context, (registryInfo.Combine(context.CompilationProvider)).Combine((jsonClassCvtrMapGenInfo.Combine(jsonObjSlzLnksInfo))));
		GenerateClassEnumMap(context, registryInfo.Combine(classEnumMapInfo), errors);
		GenerateEnumConverter(context, registryInfo);
		GenerateOtherFiles(context, registryInfo);

		//GenerationConfig[] configs = [
		//  //new()
		//  //{
		//  //	Id = "RD",
		//  //	SourceNamespace = "RhythmBase.RhythmDoctor.Events",
		//  //	TargetConverterNamespace = "RhythmBase.RhythmDoctor.Converters",
		//  //	TargetUtilsNamespace = "RhythmBase.RhythmDoctor.Utils",
		//  //	TargetUtilsClassName = "EventTypeUtils",
		//  //	BaseConverterClassName = "EventInstanceConverter",
		//  //	BaseInterfaceFullName = "RhythmBase.RhythmDoctor.Events.IBaseEvent",
		//  //	ClassTypeEnumFullname = "RhythmBase.RhythmDoctor.EventType",
		//  //	ClassTypeEnumUnknownMemberName = "ForwardEvent",
		//  //},
		//  //new()
		//  //{
		//  //	Id = "AD",
		//  //	SourceNamespace = "RhythmBase.Adofai.Events",
		//  //	TargetConverterNamespace = "RhythmBase.Adofai.Converters",
		//  //	TargetUtilsNamespace = "RhythmBase.Adofai.Utils",
		//  //	TargetUtilsClassName = "EventTypeUtils",
		//  //	BaseConverterClassName = "EventInstanceConverter",
		//  //	BaseInterfaceFullName = "RhythmBase.Adofai.Events.IBaseEvent",
		//  //	ClassTypeEnumFullname = "RhythmBase.Adofai.EventType",
		//  //	ClassTypeEnumUnknownMemberName = "ForwardEvent",
		//  //},
		//  //new()
		//  //{
		//  //	Id = "Filter",
		//  //	SourceNamespace = "RhythmBase.Adofai.Components.Filters",
		//  //	TargetConverterNamespace = "RhythmBase.Adofai.Converters",
		//  //	TargetUtilsNamespace = "RhythmBase.Adofai.Utils",
		//  //	TargetUtilsClassName = "FilterTypeUtils",
		//  //	BaseConverterClassName = "FilterInstanceConverter",
		//  //	BaseInterfaceFullName = "RhythmBase.Adofai.Components.Filters.IFilter",
		//  //	ClassTypeEnumFullname = "RhythmBase.Adofai.FilterType",
		//  //	ClassTypeEnumUnknownMemberName = "Unknown",
		//  //},
		//  //new()
		//  //{
		//  //	Id = "BeatBlock",
		//  //	SourceNamespace = "RhythmBase.BeatBlock.Events",
		//  //	TargetConverterNamespace = "RhythmBase.BeatBlock.Converters",
		//  //	TargetUtilsNamespace = "RhythmBase.BeatBlock.Utils",
		//  //	TargetUtilsClassName = "EventTypeUtils",
		//  //	BaseConverterClassName = "EventInstanceConverter",
		//  //	BaseInterfaceFullName = "RhythmBase.BeatBlock.Events.IBaseEvent",
		//  //	ClassTypeEnumFullname = "RhythmBase.BeatBlock.EventType",
		//  //	ClassTypeEnumUnknownMemberName = "ForwardEvent",
		//  //   }
		//  ];
		//foreach (var config in configs)
		//{
		//    GenerateConverter(context, config);
		//}
		//GenerateFilterTypeUtilsForEnum(context);
	}
	private const string CoreNs = "Global";
	private void GenerateOtherFiles(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<string?> registryInfo)
	{
		context.RegisterSourceOutput(registryInfo, (context, registryId) =>
		{
			if (string.IsNullOrEmpty(registryId) || registryId == CoreNs)
				return;
			string src = $$"""
			using System.Text.Json;

			namespace RhythmBase.{{registryId}}.Converters;

			public class FileMainEntryConverter
			{
				public static T DeserializeMainEntry<T>(RhythmBase.Global.Converters.JsonSerialization.IJsonDataSource dataSource, RhythmBase.Global.Converters.MetadataJsonSerializerOptions options)
						where T : new()
				{
					ReadOnlyMemory<byte> jsonData =
							dataSource.CanGetMemoryDirectly
							? dataSource.GetMemory()
							: dataSource.GetMemoryAsync().GetAwaiter().GetResult();
					Utf8JsonReader reader = new(jsonData.Span,
							new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
					return RhythmBase.{{registryId}}.Converters.ConverterHub.Read<T>(ref reader, options) ?? new();
				}
				public static async Task<T> DeserializeMainEntryAsync<T>(RhythmBase.Global.Converters.JsonSerialization.IJsonDataSource dataSource, RhythmBase.Global.Converters.MetadataJsonSerializerOptions options, CancellationToken cancellationToken = default)
						where T : new()
				{
					Utf8JsonReader reader = new((await dataSource.GetMemoryAsync(cancellationToken)).Span,
							new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
					return RhythmBase.{{registryId}}.Converters.ConverterHub.Read<T>(ref reader, options) ?? new();
				}
				public static void SerializeMainEntry<T>(T mainEntry, Stream stream, RhythmBase.Global.Converters.MetadataJsonSerializerOptions options)
				{
					using Utf8JsonWriter writer = new(stream, new()
					{
						Indented = options.JsonSerializerOptions.WriteIndented,
						Encoder = options.JsonSerializerOptions.Encoder,
						IndentCharacter = options.JsonSerializerOptions.IndentCharacter,
						IndentSize = options.JsonSerializerOptions.IndentSize,
					});
					RhythmBase.{{registryId}}.Converters.ConverterHub.Write(writer, mainEntry, options);
					writer.Flush();
				}
			}
			
			""";
			context.AddSource($"ConverterRegistry.g.cs", src);
		});
	}

	private static ClassGenCvtrInfo GetClassGenCvtrInfo(Compilation compilation, INamedTypeSymbol? enumType, string enumPropertyName, INamedTypeSymbol? symbol)
	{
		ClassGenCvtrInfo info;
		ISymbol? enumValueSymbol = GetEnumTypeMember(compilation, enumType, enumPropertyName, symbol);
		info = new()
		{
			ClassType = symbol,
			ClassTypeEnum = enumValueSymbol,
			Properties = [.. symbol.GetMembers()
				.Where(m =>
					m.Kind == SymbolKind.Property &&
					!m.IsOverride &&
					m.ContainingType.Equals(symbol, SymbolEqualityComparer.Default))
				.Cast<IPropertySymbol>()
				.Select(p => new PropertyGenerateConverterInfo
				{
						Property = p,
				})],
		};
		return info;
	}

	private static ISymbol? GetEnumTypeMember(Compilation compilation, INamedTypeSymbol? enumType, string enumPropertyName, INamedTypeSymbol? symbol)
	{
		return symbol?.GetMembers()
			.FirstOrDefault(m =>
				m.Name == enumPropertyName
				&& m.Kind == SymbolKind.Property
				&& ((IPropertySymbol)m).Type.Equals(enumType, SymbolEqualityComparer.Default))
			is not IPropertySymbol typeProperty
			? null
			: GetEnumInitializerValue(typeProperty, compilation);
	}

	private static void GenerateException(IncrementalGeneratorInitializationContext context, string id, string message, Exception ex)
	{
		context.RegisterPostInitializationOutput(ctx =>
		{
			ctx.AddSource($"ConverterGenerator_{id}_Error.g.cs", $"""
            /* This file is generated because an error was encountered during source generation. The converter source generation is skipped, and the errors should be investigated by debugging the generator. * /
            /* 
            {message}
            */
            """);
		});
	}
	private readonly struct ConverterRegistryScanResult(
		INamedTypeSymbol? ConverterType,
		ImmutableArray<ITypeSymbol> TargetTypes,
		Location? Location,
		string? Error)
	{
		public INamedTypeSymbol? ConverterType { get; } = ConverterType;
		public ImmutableArray<ITypeSymbol> TargetTypes { get; } = TargetTypes;
		public Location? Location { get; } = Location;
		public string? Error { get; } = Error;
	}

	private static bool HasAttribute(SyntaxList<AttributeListSyntax> list, string attributeFullName)
	{
		string attributeFullNameWithoutPostfix = attributeFullName.Remove(attributeFullName.Length - "Attribute".Length);
		string attributeShortName = attributeFullName.Split('.').Last();
		string attributeShortNameWithoutPostfix = attributeShortName.Remove(attributeShortName.Length - "Attribute".Length);
		return list.Any(i => i.Attributes.Any(j =>
		{
			string name = j.Name.ToString();
			return
				name == attributeShortName ||
				name == attributeFullName ||
				name == attributeShortNameWithoutPostfix ||
				name == attributeFullNameWithoutPostfix;
		}));
	}
	private static bool HasAttribute(INamedTypeSymbol symbol, string attributeFullName)
	{
		string attributeFullNameWithoutPostfix = attributeFullName.Remove(attributeFullName.Length - "Attribute".Length);
		string attributeShortName = attributeFullName.Split('.').Last();
		string attributeShortNameWithoutPostfix = attributeShortName.Remove(attributeShortName.Length - "Attribute".Length);
		return symbol.GetAttributes().Any(i =>
		{
			string name = i.AttributeClass?.ToDisplayString() ?? "";
			return
				name == attributeShortName ||
				name == attributeFullName ||
				name == attributeShortNameWithoutPostfix ||
				name == attributeFullNameWithoutPostfix;
		});
	}

	private static bool IsAttribute(AttributeData attribute, string attributeFullName)
	{
		string attributeFullNameWithoutPostfix = attributeFullName.Remove(attributeFullName.Length - "Attribute".Length);
		string attributeShortName = attributeFullName.Split('.').Last();
		string attributeShortNameWithoutPostfix = attributeShortName.Remove(attributeShortName.Length - "Attribute".Length);
		string name = attribute.AttributeClass?.ToDisplayString() ?? string.Empty;
		return
			name == attributeShortName ||
			name == attributeFullName ||
			name == attributeShortNameWithoutPostfix ||
			name == attributeFullNameWithoutPostfix;
	}

	private static bool IsJsonConverterType(INamedTypeSymbol symbol)
	{
		for (INamedTypeSymbol? current = symbol; current is not null; current = current.BaseType)
		{
			string baseName = current.ConstructedFrom?.ToDisplayString() ?? current.ToDisplayString();
			if (baseName == "System.Text.Json.Serialization.JsonConverter" ||
				baseName.StartsWith("System.Text.Json.Serialization.JsonConverter<", StringComparison.Ordinal))
				return true;
		}
		return false;
	}

	private static bool HasAccessibleParameterlessCtor(INamedTypeSymbol symbol)
	{
		return symbol.InstanceConstructors.Any(i =>
			i.Parameters.Length == 0 &&
			i.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal);
	}


	private record struct FieldName(string Name, string FullName, string? Alias = null);
	private struct EnumInfo
	{
		public bool PascalCase;
		public FieldName Symbol;
		public FieldName[] Fields;
	}

	#region Event Converter 生成

	private record struct PropertyInfo
	{
		public string? Alias;
		public IPropertySymbol Symbol;
		public string? Condition;
		public string? Converter;
		//public ISymbol? ConverterSymbol;
		public int? TimeType;
	}
	private record struct ClassInfo
	{
		public string Name;
		public INamedTypeSymbol Type;
		public string BaseTypeName;
		//public string? SpecialID;
		//public string? Alias;
		//public bool SerializerIgnore;
		public bool NeedSerializer;
		public bool HasSerializer;
		public PropertyInfo[] Properties;
	}
	private struct GenerateSettings()
	{
		public bool WithTypeEnum = false;
	}
	private static AttributeSyntax? GetAttribute(SyntaxList<AttributeListSyntax> list, string attributeFullName)
	{
		string attributeFullNameWithoutPostfix = attributeFullName.Remove(attributeFullName.Length - "Attribute".Length);
		string attributeShortName = attributeFullName.Split('.').Last();
		string attributeShortNameWithoutPostfix = attributeShortName.Remove(attributeShortName.Length - "Attribute".Length);
		foreach (var attrList in list)
		{
			foreach (var attr in attrList.Attributes)
			{
				string name = attr.Name.ToFullString();
				if (name == attributeShortName ||
					name == attributeFullName ||
					name == attributeShortNameWithoutPostfix ||
					name == attributeFullNameWithoutPostfix)
				{
					return attr;
				}
			}
		}
		return null;
	}
	static string ToShorter(string name)
	{
		string[] splitted = name.Split('.');
		return splitted[splitted.Length - 1];
	}
	static string ToLowerCamelCase(string name)
	{
		if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
			return name;
		if (name.Length == 1)
			return name.ToLower();
		return char.ToLower(name[0]) + name.Substring(1);
	}
	static ITypeSymbol? IsConcreteEnumerable(ITypeSymbol type)
	{
		if (type is IArrayTypeSymbol arrayType)
			return arrayType.ElementType;
		if (type.SpecialType == SpecialType.System_String)
			return null;
		if(type is INamedTypeSymbol namedType)
		{
			var enumerableInterface = namedType.AllInterfaces.FirstOrDefault(i =>
				i.IsGenericType &&
				i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T);
			if (enumerableInterface != null)
				return enumerableInterface.TypeArguments[0];
		}
		return null;
	}
	static ITypeSymbol WithoutNullable(ITypeSymbol type)
	{
		if (type.IsReferenceType)
		{
			return type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
		}
		else if (type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
		{
			return ((INamedTypeSymbol)type).TypeArguments[0];
		}
		return type;
	}

	private static bool IsSerializableEventProperty(PropertyDeclarationSyntax syntax, SemanticModel semanticModel)
	{
		IPropertySymbol propSymbol = semanticModel.GetDeclaredSymbol(syntax) ?? throw new NotImplementedException();
		bool hasAlias = HasAttribute(syntax.AttributeLists, JsonAliasAttrName);
		bool isPublic = propSymbol.DeclaredAccessibility == Accessibility.Public;
		bool isIgnored = HasAttribute(syntax.AttributeLists, JsonIgnoreAttrName);
		return !propSymbol.IsStatic && !isIgnored && (isPublic || hasAlias);
	}

	private static PropertyInfo BuildPropertyInfo(PropertyDeclarationSyntax syntax, SemanticModel semanticModel)
	{
		IPropertySymbol propSymbol = semanticModel.GetDeclaredSymbol(syntax) ?? throw new NotImplementedException();
		PropertyInfo prop = new();
		var aliasAttr = GetAttribute(syntax.AttributeLists, JsonAliasAttrName);
		var conditionAttr = GetAttribute(syntax.AttributeLists, JsonConditionAttrName);
		var converterAttr = GetAttribute(syntax.AttributeLists, JsonConverterAttrName);
		var timeAttr = GetAttribute(syntax.AttributeLists, JsonTimeAttrName);

		if (aliasAttr is AttributeSyntax aliasAttrNotNull)
		{
			var arg = aliasAttrNotNull.ArgumentList?.Arguments.FirstOrDefault();
			if (arg?.Expression is LiteralExpressionSyntax les && les.IsKind(SyntaxKind.StringLiteralExpression))
			{
				prop.Alias = les.Token.ValueText;
			}
		}

		if (conditionAttr is AttributeSyntax conditionAttrNotNull)
		{
			var arg = conditionAttrNotNull.ArgumentList?.Arguments.FirstOrDefault();
			if (arg?.Expression is ExpressionSyntax les)
			{
				Optional<object?> constant = semanticModel.GetConstantValue(les);
				if (constant.HasValue && constant.Value is string str)
					prop.Condition = str;
			}
		}

		if (converterAttr is AttributeSyntax converterAttrNotNull)
		{
			var arg = converterAttrNotNull.ArgumentList?.Arguments.FirstOrDefault();
			if (arg?.Expression is TypeOfExpressionSyntax toes)
			{
				prop.Converter = semanticModel.GetSymbolInfo(toes.Type).Symbol?.ToDisplayString();
			}
		}

		if (timeAttr is AttributeSyntax timeAttrNotNull)
		{
			var arg = timeAttrNotNull.ArgumentList?.Arguments.FirstOrDefault();
			if (arg?.Expression is MemberAccessExpressionSyntax maes)
			{
				prop.TimeType = (int?)semanticModel.GetConstantValue(maes).Value;
			}
		}

		prop.Symbol = propSymbol;
		return prop;
	}

	private static void AppendWriteLinesWithCondition(StringBuilder target, StringBuilder content, string? conditionRaw, bool multiline)
	{
		if (string.IsNullOrEmpty(conditionRaw))
		{
			target.Append(content);
			return;
		}

		string condition = conditionRaw!
			.Replace("$&", "value")
			.Replace("$r", "_rs")
			.Replace("$w", "_ws");

		string[] lines = [.. content.ToString()
			.Split(['\n'], StringSplitOptions.RemoveEmptyEntries)
			.Where(i => !string.IsNullOrWhiteSpace(i))
			.Select(i => "\t" + i)];

		if (multiline)
		{
			target.AppendLine($$"""
		if ({{condition}})
		{
{{string.Concat(lines)}}
		}
""");
		}
		else
		{
			target.AppendLine($$"""
		if ({{condition}})
			{{string.Concat(lines).Trim()}}
""");
		}
	}

	static List<string> marks = [];
	private record struct GenerationConfig
	{
		// RD
		// AD
		// AD_Filter
		internal string Id;
		// RhythmBase.RhythmDoctor.Events
		// RhythmBase.Adofai.Events
		// RhythmBase.Adofai.Components.Filters
		internal string SourceNamespace;
		// RhythmBase.RhythmDoctor.Converters
		// RhythmBase.Adofai.Converters
		// RhythmBase.Adofai.Converters.Filters
		internal string TargetConverterNamespace;
		// RhythmBase.RhythmDoctor.Utils
		// RhythmBase.Adofai.Utils
		internal string TargetUtilsNamespace;
		internal string TargetUtilsClassName;
		// EventInstanceConverter
		// FilterInstanceConverter
		internal string BaseConverterClassName;
		// RhythmBase.RhythmDoctor.Events.IBaseEvent
		// RhythmBase.Adofai.Events.IBaseEvent
		// RhythmBase.Adofai.Components.Filters.IFilter
		internal string BaseInterfaceFullName;
		// RhythmBase.RhythmDoctor.EventType
		// RhythmBase.Adofai.EventType
		// RhythmBase.Adofai.Components.Filters.FilterType
		internal string ClassTypeEnumFullname;
		internal string ClassTypeEnumUnknownMemberName;
	}

	#endregion

	#region Filter Converter 生成

	#endregion

	#region 工具方法

	private static string TypeNameOf(ITypeSymbol symbol)
	{
		if (symbol is INamedTypeSymbol namedTypeSymbol)
		{
			if (namedTypeSymbol.TypeArguments.Length > 0)
			{
				string typeArgs = string.Join(", ", namedTypeSymbol.TypeArguments.Select(t => t.ToDisplayString()));
				return $"{namedTypeSymbol.Name}<{typeArgs}>";
			}
			else
			{
				return namedTypeSymbol.Name;
			}
		}
		else if (symbol is IArrayTypeSymbol)
		{
			return symbol.ToDisplayString();
		}
		return "";
	}

	private static string LastPartOf(string str) => str.Split('.').Last();
	#endregion
}