using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// 这坨写得太史了

namespace RhythmBase.Generator;

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
	// 泛型类必须指定自定义转换器
	private static readonly DiagnosticDescriptor GenericTypeMissingConverterRule = new(
		"RD0011",
		"Generic type missing converter",
		"Generic type '{0}' must specify a custom converter using RhythmBase.JsonObjectHasSerializerAttribute",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
	// 泛型成员转换器的基类必须是非泛型
	private static readonly DiagnosticDescriptor GenericConverterBaseMustBeNonGenericRule = new(
		"RD0012",
		"Generic converter base must be non-generic",
		"Base converter '{0}' for generic converter '{1}' must be a non-generic type",
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
	List<Exception> exceptions = new();
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var errors = new HashSet<Diagnostic>();
		IncrementalValueProvider<string?> registryInfo = context.CompilationProvider // checked
				.Select((compilation, ct) =>
		{
			try
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
			}
			catch (Exception ex)
			{
				exceptions.Add(ex);
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
							if (args[0].Value is not INamedTypeSymbol type) return default;
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
		IncrementalValueProvider<IAssemblySymbol[]> allasms = context.CompilationProvider.Select((compilation, ct) =>
		{
			return compilation.References.Select(i => compilation.GetAssemblyOrModuleSymbol(i)).OfType<IAssemblySymbol>().Concat([compilation.Assembly]).ToArray();
		});
		IncrementalValueProvider<EventTypeRegistryGenerationInfo[]> EventTypeRegistryInfo = classEnumAttrPairInfo // checked
				.Select((input, ct) =>
		{
			(
				Compilation compilation,
				ClassGenAttr[] types,
				var errors
				) = input;
			INamedTypeSymbol eventTypeInterface = compilation.GetTypeByMetadataName(IEventTypeName) ?? throw new NotImplementedException("1111");
			var fallbackAttr = compilation.GetTypeByMetadataName(JsonObjectSerializationFallbackAttrName);
			List<EventTypeRegistryGenerationInfo> typesToGenerate = [];
			foreach (var classGen in types)
			{
				INamedTypeSymbol baseType = classGen.RootType;
				INamedTypeSymbol enumType = classGen.EnumType;
				string enumPropertyName = classGen.TypePropertyName;
				Dictionary<INamedTypeSymbol, HashSet<ISymbol>> dict = new(SymbolEqualityComparer.Default);
				if (baseType is null || enumType is null)
					continue;
				ISymbol[] enumMembers = enumType?.GetMembers().Where(m => m.Kind == SymbolKind.Field).ToArray() ?? [];
				ISymbol? fallbackEnumMember = null;
				INamedTypeSymbol[] subTypes = [.. GetDerivedTypes(baseType, compilation)];
				INamedTypeSymbol[] subEventClasses = [.. subTypes.Where(t => (t.TypeKind is TypeKind.Class or TypeKind.Struct) && !t.IsAbstract)];
				IEnumerable<INamedTypeSymbol> fallbackTypes = subEventClasses.Where(c => c.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, fallbackAttr)));
				INamedTypeSymbol? fallbackType = null;
				switch (fallbackTypes.Count())
				{
					case 0:
						break;
					case > 1:
						errors.Add(Diagnostic.Create(MultipleFallbackModelsRule, baseType.Locations.FirstOrDefault(), baseType.ToDisplayString(), string.Join(", ", fallbackTypes.Select(i => i.ToDisplayString()))));
						continue;
					default:
						fallbackType = fallbackTypes.FirstOrDefault();
						break;
				}
				subEventClasses = subEventClasses.Where(c =>
								!SymbolEqualityComparer.Default.Equals(c, fallbackType) &&
								!IsDerivedFrom(c, fallbackType)).ToArray();
				Dictionary<INamedTypeSymbol, ISymbol> EventTypeRegistry = new(SymbolEqualityComparer.Default);
				foreach (var subClass in subEventClasses)
				{
					var property = subClass.GetMembers().FirstOrDefault(m => m.Kind == SymbolKind.Property && m.Name == enumPropertyName) as IPropertySymbol;
					ISymbol? enumSymbol = null;
					if (property is not null && SymbolEqualityComparer.Default.Equals((ITypeSymbol?)property.Type, enumType))
					{
						enumSymbol = GetEnumInitializerValue(property, compilation);
					}
					if (enumSymbol != null)
						EventTypeRegistry[subClass] = enumSymbol;
					else errors.Add(Diagnostic.Create(
						MissingEnumInitializerRule,
						property?.Type.Locations.FirstOrDefault() ?? subClass.Locations.FirstOrDefault(),
						property?.Name ?? subClass.Name,
						property?.ContainingType?.ToDisplayString() ?? subClass.ContainingType?.ToDisplayString()));
				}
				if(fallbackType is not null)
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
						enumMap.UnionWith(subTypeSubEventClasses.Where(c => EventTypeRegistry.ContainsKey(c)).Select(c => EventTypeRegistry[c]));
					else
						dict[subType] = new HashSet<ISymbol>(subTypeSubEventClasses.Where(c => EventTypeRegistry.ContainsKey(c)).Select(c => EventTypeRegistry[c]), SymbolEqualityComparer.Default);
				}
				typesToGenerate.Add(new()
				{
					ClassType = classGen.RootType,
					ClassTypeEnum = enumType,
					FallbackClassType = fallbackType,
					FallbackClassTypeEnum = fallbackEnumMember,
					Classes = subEventClasses,
					EventTypeRegistry = dict,
					ClassEnumDoubleMap = EventTypeRegistry,
				});
			}
			return typesToGenerate.ToArray();
		});
		IncrementalValueProvider<INamedTypeSymbol?> classConverterBaseInfo = context.CompilationProvider
				.Select((compilation, ct) => compilation.GetTypeByMetadataName(MemberConverterTypeName));

		var enumNeedCvtr = context.CompilationProvider.Combine(allasms).Select((compilationAndAsms, ct) =>
		{
			try
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
			}
			catch (Exception ex)
			{
				exceptions.Add(ex);
			}
			return default;
		});
		var jsonClassExistedConverterInfor = context.SyntaxProvider.ForAttributeWithMetadataName(JsonObjectHasSerializerAttrName,
			static (node, ct) => node is ClassDeclarationSyntax or StructDeclarationSyntax or RecordDeclarationSyntax,
			(ctx, ct) =>
			{
				try
				{
					if (ctx.TargetSymbol is not INamedTypeSymbol typeSymbol) return default;
					return typeSymbol;

				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
				}
				return default;
			}).Collect();

		var jsonClassCvtrMapGenInfo = classEnumAttrPairInfo
			.Select((classEnumAttrPairs, ct) =>
			{
				try
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
						var subTypes = GetAllTypes(type.ContainingAssembly)
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

				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
				}
				return default;
			});

		var jsonObjSlzForInfo = allasms.Combine(context.CompilationProvider).SelectMany((allasmsAndCompilation, ct) =>
		{
			try
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

			}
			catch (Exception ex)
			{
				exceptions.Add(ex);
			}
			return default;
		})
		.Collect();

		var jsonObjSlzLnkInfo = context.CompilationProvider.Combine(allasms).Select((compilationAndAsms, ct) =>
		{
			try
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
			}
			catch (Exception ex)
			{
				exceptions.Add(ex);
			}
			return default;
		});
		var jsonObjSlzLnksInfo = jsonObjSlzForInfo.Combine(jsonObjSlzLnkInfo).Select((ctx, ct) =>
		{
			try
			{
				List<(ITypeSymbol, INamedTypeSymbol)> gens = [];
				foreach (var pair in ctx.Left)
					gens.Add((pair.Item1, pair.Item2));
				foreach (var pair in ctx.Right)
					gens.Add((pair.Item1, pair.Item2));
				return gens.ToArray();

			}
			catch (Exception ex)
			{
				exceptions.Add(ex);
			}
			return default;
		});

		GenerateTypeConverterRegistry(context, registryInfo.Combine(jsonObjSlzLnksInfo));
		GenerateConverter(context, (registryInfo.Combine(context.CompilationProvider)).Combine((jsonClassCvtrMapGenInfo.Combine(jsonObjSlzLnksInfo))));
		GenerateEventTypeRegistry(context, registryInfo.Combine(context.CompilationProvider).Combine(EventTypeRegistryInfo), errors);
		GenerateEnumConverter(context, registryInfo);
		GenerateOtherFiles(context, registryInfo);

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
				private static readonly JsonReaderOptions _readerOptions = new();
				[global::System.Diagnostics.DebuggerHidden]
				[global::System.Diagnostics.StackTraceHidden]
				private static void WrapAndThrow(JsonException ex, RhythmBase.Global.Converters.JsonSerialization.IJsonDataSource dataSource, long bytesConsumed)
				{
					long originalPos = dataSource.MapToInputPosition(bytesConsumed);
					if (originalPos >= 0)
						throw new JsonException($"{ex.Message}\n  at original stream byte position ~{originalPos}", ex);
					throw new JsonException($"{ex.Message}\n  at processed byte position {bytesConsumed}", ex);
				}
				public static T DeserializeMainEntry<T>(RhythmBase.Global.Converters.JsonSerialization.IJsonDataSource dataSource, RhythmBase.Global.Converters.MetadataJsonSerializerOptions options)
						where T : new()
				{
					var seq = dataSource.GetSequence();
					Utf8JsonReader reader = seq.IsSingleSegment
						? new Utf8JsonReader(seq.First.Span, _readerOptions)
						: new Utf8JsonReader(seq, _readerOptions);
					return RhythmBase.{{registryId}}.Converters.TypeConverterRegistry.Read<T>(ref reader, options) ?? new();
				}
				public static async Task<T> DeserializeMainEntryAsync<T>(RhythmBase.Global.Converters.JsonSerialization.IJsonDataSource dataSource, RhythmBase.Global.Converters.MetadataJsonSerializerOptions options, CancellationToken cancellationToken = default)
						where T : new()
				{
					var seq = await dataSource.GetSequenceAsync(cancellationToken);
					Utf8JsonReader reader = seq.IsSingleSegment
						? new Utf8JsonReader(seq.First.Span, _readerOptions)
						: new Utf8JsonReader(seq, _readerOptions);
					return RhythmBase.{{registryId}}.Converters.TypeConverterRegistry.Read<T>(ref reader, options) ?? new();
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
					RhythmBase.{{registryId}}.Converters.TypeConverterRegistry.Write(writer, mainEntry, options);
					writer.Flush();
				}
			}
			
			""";
			context.AddSource($"FileMainEntryConverter.{registryId}.g.cs", src);
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
	#region Event Converter 生成
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
		if (type is INamedTypeSymbol namedType)
		{
			var enumerableInterface = namedType.AllInterfaces.FirstOrDefault(i =>
				i.IsGenericType &&
				i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T);
			if (enumerableInterface != null)
				return enumerableInterface.TypeArguments[0];
		}
		return null;
	}
	static List<string> marks = [];
	#endregion
}