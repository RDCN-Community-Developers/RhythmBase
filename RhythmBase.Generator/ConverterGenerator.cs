using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

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
		IncrementalValueProvider<(string?, List<Diagnostic>)> registryInfo = context.CompilationProvider
				.Select((compilation, ct) =>
		{
			List<Diagnostic> diagnostics = new();
			try
			{
				var attrType = compilation.GetTypeByMetadataName(JsonConverterIdAttrName);
				if (attrType == null) return (default(string?), diagnostics);
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
							diagnostics.Add(Diagnostic.Create(InvalidNamespaceIdRule, attr.ApplicationSyntaxReference?.GetSyntax().GetLocation(), namespaceId));
						}
						return (namespaceId, diagnostics);
					}
				}
			}
			catch (Exception ex)
			{
				exceptions.Add(ex);
			}
			return (default(string?), diagnostics);
		});
		var classEnumAttrPairInfo = context.CompilationProvider
			.Select((compilation, ct) =>
			{
				var attrType = compilation.GetTypeByMetadataName(JsonConverterSourceTypeAttrName);
				if (attrType == null) return default;
				var assembly = compilation.Assembly;
				List<ClassGenAttr> types = [];
				List<Diagnostic> diagnostics = new();
				foreach (var attr in assembly.GetAttributes())
				{
					if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attrType))
					{
						var args = attr.ConstructorArguments;
						if (args[0].Value is not INamedTypeSymbol type) return default;
						if (args[1].Value is not INamedTypeSymbol enumType || enumType.TypeKind != TypeKind.Enum)
						{
							diagnostics.Add(Diagnostic.Create(InvalidEnumTypeRule, type?.Locations.FirstOrDefault(), args[1].Value?.ToString() ?? "null"));
							continue;
						}
						if (args[2].Value is not INamedTypeSymbol converterBaseType)
						{
							continue;
						}
						if (args[3].Value is not string enumPropertyName) return default;
						types.Add(new()
						{
							RootType = type,
							EnumType = enumType,
							RootConverterType = converterBaseType,
							TypePropertyName = enumPropertyName,
						});
					}
				}
				return (compilation, types.ToArray(), diagnostics);
			});
		IncrementalValueProvider<IAssemblySymbol[]> allasms = context.CompilationProvider.Select((compilation, ct) =>
		{
			return compilation.References.Select(i => compilation.GetAssemblyOrModuleSymbol(i)).OfType<IAssemblySymbol>().Concat([compilation.Assembly]).ToArray();
		});
		var tickTimeGenInfo = context.CompilationProvider.Select((compilation, ct) =>
		{
			var tickTimeAttr = compilation.GetTypeByMetadataName(AdapterTypeAttrName);
			if (tickTimeAttr == null) return default;
			List<INamedTypeSymbol> types = [];
			foreach (var attr in compilation.Assembly.GetAttributes())
			{
				if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, tickTimeAttr) && attr.ConstructorArguments.Length == 5)
				{
					(
						INamedTypeSymbol? chartType,
						INamedTypeSymbol? calculatorType,
						INamedTypeSymbol? tickTimeType,
						INamedTypeSymbol? typeEnumType,
						INamedTypeSymbol? typeInterfaceType
						) result
						= (
							attr.ConstructorArguments[0].Value as INamedTypeSymbol ?? null, // 实现 IChart<>
							attr.ConstructorArguments[1].Value as INamedTypeSymbol ?? null, // 实现 ICalculator
							attr.ConstructorArguments[2].Value as INamedTypeSymbol ?? null,  // 实现 ITickTime
							attr.ConstructorArguments[3].Value as INamedTypeSymbol ?? null, // 枚举类型
							attr.ConstructorArguments[4].Value as INamedTypeSymbol ?? null  // 类型接口类型
							);
					return result;
				}
			}
			return (null, null, null, null, null);
		});
		IncrementalValueProvider<EventTypeRegistryGenerationInfo[]> EventTypeRegistryInfo = classEnumAttrPairInfo
				.Select((input, ct) =>
		{
			(
				Compilation compilation,
				ClassGenAttr[] types,
				var incomingDiagnostics
				) = input;
			List<Diagnostic> diagnostics = new(incomingDiagnostics);
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
						diagnostics.Add(Diagnostic.Create(MultipleFallbackModelsRule, baseType.Locations.FirstOrDefault(), baseType.ToDisplayString(), string.Join(", ", fallbackTypes.Select(i => i.ToDisplayString()))));
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
					else diagnostics.Add(Diagnostic.Create(
						MissingEnumInitializerRule,
						property?.Type.Locations.FirstOrDefault() ?? subClass.Locations.FirstOrDefault(),
						property?.Name ?? subClass.Name,
						property?.ContainingType?.ToDisplayString() ?? subClass.ContainingType?.ToDisplayString()));
				}
				if (fallbackType is not null)
				{
					var property = fallbackType.GetMembers().FirstOrDefault(m => m.Kind == SymbolKind.Property && m.Name == enumPropertyName);
					if (property is IPropertySymbol propSymbol)
					{
						var typeOfEnum = propSymbol.Type;
						if (SymbolEqualityComparer.Default.Equals(typeOfEnum, enumType))
						{
							var enumSymbol = GetEnumInitializerValue(propSymbol, compilation);
							if (enumSymbol == null)
								diagnostics.Add(Diagnostic.Create(MissingEnumInitializerRule, propSymbol.Locations.FirstOrDefault(), propSymbol.Name, propSymbol.ContainingType?.ToDisplayString()));
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
					RootClassType = classGen.RootType,
					ClassTypeEnum = enumType,
					FallbackClassType = fallbackType,
					FallbackClassTypeEnum = fallbackEnumMember,
					Classes = subEventClasses,
					EventTypeRegistry = dict,
					ClassEnumDoubleMap = EventTypeRegistry,
					Diagnostics = diagnostics,
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
		GenerateEventTypeRegistry(context, registryInfo.Combine(context.CompilationProvider).Combine(EventTypeRegistryInfo));
		GenerateEnumConverter(context, registryInfo);
		GenerateOtherFiles(context, registryInfo);
		GenerateUpgrater(context, registryInfo.Combine(EventTypeRegistryInfo));
		GenerateTickTimeType(context, registryInfo.Combine(tickTimeGenInfo).Combine(EventTypeRegistryInfo));

	}

	private void GenerateTickTimeType(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<((
		(string?, List<Diagnostic>) Left, (
			INamedTypeSymbol? chartType,
			INamedTypeSymbol? calculatorType,
			INamedTypeSymbol? tickTimeType,
			INamedTypeSymbol? typeEnumType,
			INamedTypeSymbol? typeInterfaceType
		) Right) baseData, EventTypeRegistryGenerationInfo[] registryGenerationInfos)> incrementalValueProvider)
	{
		context.RegisterSourceOutput(incrementalValueProvider, (context, data) =>
		{
			(var values, EventTypeRegistryGenerationInfo[] registryGenerationInfos) = data;
			((var registryId, var diagnostics), var s) = values;
			foreach (var diag in diagnostics)
				context.ReportDiagnostic(diag);
			if (string.IsNullOrEmpty(registryId) || registryId == CoreNs)
				return;
			if (
				s.tickTimeType is not INamedTypeSymbol tickTimeSymbol ||
				s.chartType is not INamedTypeSymbol chartSymbol ||
				s.calculatorType is not INamedTypeSymbol calculatorSymbol ||
				s.typeEnumType is not INamedTypeSymbol typeEnumSymbol ||
				s.typeInterfaceType is not INamedTypeSymbol typeInterfaceSymbol
				)
				return;
			string TickTimeName = tickTimeSymbol.Name;
			string TickTime = tickTimeSymbol.ToDisplayString();
			string Chart = chartSymbol.ToDisplayString();
			string Calculator = calculatorSymbol.ToDisplayString();
			string TickRangeName = $"{tickTimeSymbol.Name}Range";
			string TickTimeRange = $"{tickTimeSymbol.ContainingNamespace.ToDisplayString()}.{TickRangeName}";
			string EventType = typeEnumSymbol.ToDisplayString();
			string IBaseEvent = typeInterfaceSymbol.ToDisplayString();
			string srcTickTime = $$"""
			// <auto-generated/>
			#nullable enable
			namespace {{tickTimeSymbol.ContainingNamespace.ToDisplayString()}};

			/// <summary>
			/// A tick.
			/// </summary>
			public partial struct {{TickTimeName}} : ITickTime<{{TickTimeName}}>
			#if NET7_0_OR_GREATER
				, System.Numerics.IComparisonOperators<{{TickTimeName}}, {{TickTimeName}}, bool>
			#endif
			{
				internal readonly {{Chart}}? BaseChart => _calculator?.Collection;
				/// <summary>
				/// Gets a value indicating whether the current instance contains no loaded data or associated <see cref="{{Calculator}}"/>.
				/// </summary>
				/// <remarks>The property returns <see langword="true"/> if the <see cref="{{Calculator}}"/> is not set or if none of the data
				/// components are loaded. Use this property to check whether the instance is in an uninitialized or empty state before
				/// performing operations that require loaded data.</remarks>
				[System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, nameof(_calculator))]
				public readonly partial bool IsEmpty { get; }
				internal static bool MustFromCache { get; } = false;
				/// <summary>
				/// The total number of beats from this moment to the beginning of the <see cref="{{Chart}}"/>.
				/// </summary>
				public partial float Tick { get; }
				/// <summary>
				/// The total amount of time from the beginning of the <see cref="{{Chart}}"/> to this beat.
				/// </summary>
				public partial TimeSpan TimeSpan { get; }
				/// <summary>
				/// The number of beats per minute followed at this moment.
				/// </summary>
				public float Bpm
				{
					get
					{
						if (!_isBPMLoaded)
						{
							_BPM = _calculator?.BeatsPerMinuteOf(this) ?? 0;
							_isBPMLoaded = true;
						}
						return _BPM;
					}
				}
				/// <summary>
				/// Initializes a new instance of the <see cref="{{TickTimeName}}"/> class with default values.
				/// </summary>
				/// <remarks>This constructor sets the initial state of the <see cref="{{TickTimeName}}"/> instance, including default
				/// values for internal fields. The instance is initialized with a beat value of 1, a bar-beat tuple of (1, 1f), a
				/// zero <see cref="System.TimeSpan"/>, and flags indicating that the beat, bar-beat, and time span are loaded.</remarks>
				public {{TickTimeName}}()
				{
					InitDefault();
				}
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				partial void InitDefault();
				/// <summary>
				/// Construct an instance without specifying a <see cref="{{Calculator}}"/>.
				/// </summary>
				/// <param name="tick">The total number of beats from this moment to the beginning of the <see cref="{{Chart}}"/>.</param>
				public {{TickTimeName}}(float tick)
				{
					this = default;
					_tick = tick;
					NormalizeTick();
					_isTickLoaded = true;
				}
				/// <summary>
				/// Constructs an instance of RDBeat with the specified time span.
				/// </summary>
				/// <param name="timeSpan">The total amount of time from the start of the <see cref="{{Chart}}"/> to the moment.</param>
				public {{TickTimeName}}(TimeSpan timeSpan)
				{
					this = default;
					_TimeSpan = timeSpan;
					NormalizeTimeSpan();
					_isTimeSpanLoaded = true;
				}
				/// <summary>
				/// Construct an instance with specifying a <see cref="{{Calculator}}"/>.
				/// </summary>
				/// <param name="calculator">Specified <see cref="{{Calculator}}"/>.</param>
				/// <param name="tick">The total number of beats from this moment to the beginning of the <see cref="{{Chart}}"/>.</param>
				public {{TickTimeName}}({{Calculator}} calculator, float tick)
				{
					this = new {{TickTimeName}}(tick);
					_calculator = calculator;
				}
				/// <summary>
				/// Construct an instance with specifying a <see cref="{{Calculator}}"/>.
				/// </summary>
				/// <param name="calculator">Specified <see cref="{{Calculator}}"/>.</param>
				/// <param name="timeSpan">The total amount of time from the start of the <see cref="{{Chart}}"/> to the moment</param>
				public {{TickTimeName}}({{Calculator}} calculator, TimeSpan timeSpan)
				{
					this = new {{TickTimeName}}(timeSpan);
					_calculator = calculator;
					_tick = _calculator.TimeSpanToTick(timeSpan) - 1f;
				}
				/// <summary>
				/// Construct an instance with specifying a <see cref="{{Calculator}}"/>.
				/// </summary>
				/// <param name="calculator">Specified <see cref="{{Calculator}}"/>.</param>
				/// <param name="beat">Another instance.</param>
				public partial {{TickTimeName}}({{Calculator}} calculator, {{TickTimeName}} beat);
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				partial void NormalizeTick();
				partial void NormalizeTimeSpan();
				/// <summary>
				/// Construct a beat of the 1st beat from the <see cref="{{Calculator}}"/>
				/// </summary>
				/// <param name="calculator">Specified <see cref="{{Calculator}}"/>.</param>
				/// <returns>The first beat tied to the <see cref="{{Chart}}"/>.</returns>
				public static {{TickTimeName}} Default({{Calculator}} calculator)
				{
					{{TickTimeName}} Default = new(calculator, 1f);
					return Default;
				}
				/// <summary>
				/// Determine if two beats come from the same <see cref="{{Chart}}"/>
				/// </summary>
				/// <param name="a">A beat.</param>
				/// <param name="b">Another beat.</param>
				/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same <see cref="{{Chart}}"/>.</param>
				/// <returns><c>true</c> if both beats share the same <see cref="{{Chart}}"/>; otherwise, <c>false</c>.</returns>
				public static bool FromSameChart({{TickTimeName}} a, {{TickTimeName}} b, bool @throw = false) =>
					(a._calculator?.Equals(b._calculator) ?? true)
					|| (@throw ? throw new InvalidOperationException("Beats must come from the same Chart.") : false);
				/// <summary>
				/// Determine if two beats are from the same <see cref="{{Chart}}"/>.
				/// <br />
				/// If any of them does not come from any <see cref="{{Chart}}"/>, it will also return true.
				/// </summary>
				/// <param name="a">A beat.</param>
				/// <param name="b">Another beat.</param>
				/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same <see cref="{{Chart}}"/>.</param>
				/// <returns><c>true</c> if both beats share the same <see cref="{{Chart}}"/> or if either beat is unbound; otherwise, <c>false</c>.</returns>
				public static bool FromSameChartOrNull({{TickTimeName}}? a, {{TickTimeName}}? b, bool @throw = false) => a?._calculator == null || b?._calculator == null || FromSameChart(a.Value, b.Value, @throw);
				/// <summary>
				/// Determine if two beats are from the same <see cref="{{Chart}}"/>.
				/// </summary>
				/// <param name="b">Another beat.</param>
				/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same <see cref="{{Chart}}"/>.</param>
				/// <returns><c>true</c> if both beats share the same <see cref="{{Chart}}"/>; otherwise, <c>false</c>.</returns>
				[System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true)]
				public readonly bool FromSameChart({{TickTimeName}} b, bool @throw = false) => FromSameChart(this, b, @throw);
				/// <summary>
				/// Determine if two beats are from the same <see cref="{{Chart}}"/>.
				/// <br />
				/// If any of them does not come from any <see cref="{{Chart}}"/>, it will also return true.
				/// </summary>
				/// <param name="b">Another beat.</param>
				/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same <see cref="{{Chart}}"/>.</param>
				/// <returns><c>true</c> if both beats share the same <see cref="{{Chart}}"/> or if either beat is unbound; otherwise, <c>false</c>.</returns>
				public readonly bool FromSameChartOrNull({{TickTimeName}} b, bool @throw = false) => BaseChart == null || b.BaseChart == null || FromSameChart(b, @throw);
				/// <summary>
				/// Returns a new instance of unbinding the <see cref="{{Chart}}"/>.
				/// </summary>
				/// <returns>A new instance of unbinding the <see cref="{{Chart}}"/>.</returns>
				public readonly {{TickTimeName}} WithoutLink()
				{
					{{TickTimeName}} result = this;
					if (result._calculator != null)
						result.Cache();
					result._calculator = null;
					return result;
				}
				/// <summary>
				/// Links the current beat to the specified <see cref="{{Calculator}}"/>, enabling it to be used for beat calculations.
				/// </summary>
				/// <remarks>If the current beat is already linked to a <see cref="{{Calculator}}"/>, providing a different
				/// <see cref="{{Calculator}}"/> will result in an exception. This method does not modify the original instance if it is already linked
				/// to the specified <see cref="{{Calculator}}"/>.</remarks>
				/// <param name="calculator">The <see cref="{{Calculator}}"/> instance to associate with this beat. This parameter cannot be null.</param>
				/// <returns>The current <see cref="{{TickTimeName}}"/> instance with the linked <see cref="{{Calculator}}"/>.</returns>
				/// <exception cref="InvalidOperationException">Thrown if the beat is already linked to a different <see cref="{{Calculator}}"/>.</exception>
				public readonly {{TickTimeName}} WithLink({{Calculator}} calculator)
				{
					{{TickTimeName}} result = this;
					if (result._calculator == null)
						result._calculator = calculator;
					else if (!ReferenceEquals(result._calculator, calculator))
						throw new InvalidOperationException("The beat is already linked to another {{chartSymbol.Name}}.");
					return result;
				}
				/// <summary>
				/// Returns a new RDBeat instance with the specified <see cref="{{Calculator}}"/> assigned if the current instance does not already have
				/// a <see cref="{{Calculator}}"/>.
				/// </summary>
				/// <remarks>Use this method to ensure that an RDBeat instance has an associated <see cref="{{Calculator}}"/> without overwriting
				/// an existing one.</remarks>
				/// <param name="calculator">The <see cref="{{Calculator}}"/> to assign if the current instance's <see cref="{{Calculator}}"/> is null.</param>
				/// <returns>An RDBeat instance with the calculator set to the specified value if it was previously null; otherwise, the current
				/// instance.</returns>
				public readonly {{TickTimeName}} WithLinkIfNull({{Calculator}} calculator)
				{
					{{TickTimeName}} result = this;
					if (result._calculator == null)
						result._calculator = calculator;
					return result;
				}
				/// <summary>
				/// Creates a new RDBeat instance that is linked to the specified <see cref="{{Chart}}"/>.
				/// </summary>
				/// <param name="chart">The <see cref="{{Chart}}"/> to which the new RDBeat instance will be linked. Cannot be null.</param>
				/// <returns>A new RDBeat instance associated with the provided <see cref="{{Chart}}"/>.</returns>
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				public readonly {{TickTimeName}} WithLink({{Chart}} chart) => WithLink(chart.Calculator);
				/// <summary>
				/// Returns a new RDBeat instance that is linked to the specified <see cref="{{Chart}}"/>'s <see cref="{{Calculator}}"/> if it is not already linked.
				/// </summary>
				/// <param name="chart">The <see cref="{{Chart}}"/> whose <see cref="{{Calculator}}"/> will be used to link the RDBeat instance. This parameter must not be null.</param>
				/// <returns>A new RDBeat instance linked to the <see cref="{{Calculator}}"/> of the specified <see cref="{{Chart}}"/>.</returns>
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				public readonly {{TickTimeName}} WithLinkIfNull({{Chart}} chart) => WithLinkIfNull(chart.Calculator);
				[System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(_calculator))]
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				internal readonly void IfNullThrowException()
				{
					if (IsEmpty)
					{
						throw new InvalidRDBeatException();
					}
				}
				internal partial void ResetBPM();
				/// <summary>
				/// Refresh the cache.
				/// </summary>
				public partial void ResetCache();
				/// <summary>
				/// Caches the current state of the beat by accessing its properties.
				/// </summary>
				/// <exception cref="InvalidRDBeatException">Thrown when the beat cannot be calculated.</exception>
				public partial void Cache();
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				private static partial int CompareInternal({{TickTimeName}} left, {{TickTimeName}} right);
				/// <summary>
				/// Adds a specified number of beats to the current beat and caches the result.
				/// </summary>
				/// <param name="left">The current beat instance.</param>
				/// <param name="right">The number of beats to add.</param>
				/// <param name="result">The result beat instance.</param>
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				private static partial void AddTickAndCache({{TickTimeName}} left, float right, ref {{TickTimeName}} result);
				/// <summary>
				/// Adds a specified <see cref="TimeSpan"/> to the current beat and caches the result.
				/// </summary>
				/// <param name="left">The current beat instance.</param>
				/// <param name="right">The time span to add.</param>
				/// <param name="result">The result beat instance.</param>
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				private static partial void AddTimeSpanAndCache({{TickTimeName}} left, TimeSpan right, ref {{TickTimeName}} result);
				/// <summary>
				/// Subtracts a specified number of beats from the current beat and caches the result.
				/// </summary>
				/// <param name="left">The current beat instance.</param>
				/// <param name="right">The number of beats to subtract.</param>
				/// <param name="result">The result beat instance.</param>
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				private static partial void SubstractTickAndCache({{TickTimeName}} left, float right, ref {{TickTimeName}} result);
				/// <summary>
				/// Subtracts a specified <see cref="TimeSpan"/> from the current beat and caches the result.
				/// </summary>
				/// <param name="left">The current beat instance.</param>
				/// <param name="right">The time span to subtract.</param>
				/// <param name="result">The result beat instance.</param>
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				private static partial void SubstractTimeSpanAndCache({{TickTimeName}} left, TimeSpan right, ref {{TickTimeName}} result);
				/// <inheritdoc/>
				public static {{TickTimeName}} operator +({{TickTimeName}} left, float right)
				{
					if (right == 0) return left;
					{{TickTimeName}} result = new();
					if (!left.IsEmpty)
						result = new {{TickTimeName}}(left._calculator!, left.Tick + right);
					else
					{
						AddTickAndCache(left, right, ref result);
						result._isTimeSpanLoaded = false;
					}
					return result;
				}
				/// <inheritdoc/>
				public static {{TickTimeName}} operator +({{TickTimeName}} left, TimeSpan right)
				{
					if (right == TimeSpan.Zero) return left;
					{{TickTimeName}} result = new();
					if (!left.IsEmpty)
						result = new {{TickTimeName}}(left._calculator!, left.TimeSpan + right);
					else
					{
						AddTimeSpanAndCache(left, right, ref result);
						result._isTickLoaded = false;
					}
					return result;
				}
				/// <inheritdoc/>
				public static {{TickTimeName}} operator -({{TickTimeName}} left, float right)
				{
					if (right == 0f) return left;
					{{TickTimeName}} result = new();
					if (!left.IsEmpty)
						result = new {{TickTimeName}}(left._calculator!, left.Tick - right);
					else
					{
						SubstractTickAndCache(left, right, ref result);
						result._isTimeSpanLoaded = false;
					}
					return result;
				}
				/// <inheritdoc/>
				public static {{TickTimeName}} operator -({{TickTimeName}} left, TimeSpan right)
				{
					if (right == TimeSpan.Zero) return left;
					{{TickTimeName}} result = new();
					if (!left.IsEmpty)
						result = new {{TickTimeName}}(left._calculator!, left.TimeSpan - right);
					else
					{
						SubstractTimeSpanAndCache(left, right, ref result);
						result._isTickLoaded = false;
					}
					return result;
				}
				/// <inheritdoc/>
				public static bool operator >({{TickTimeName}} a, {{TickTimeName}} b) => CompareInternal(a, b) > 0;
				/// <inheritdoc/>
				public static bool operator <({{TickTimeName}} a, {{TickTimeName}} b) => CompareInternal(a, b) < 0;
				/// <inheritdoc/>
				public static bool operator >=({{TickTimeName}} a, {{TickTimeName}} b) => CompareInternal(a, b) >= 0;
				/// <inheritdoc/>
				public static bool operator <=({{TickTimeName}} a, {{TickTimeName}} b) => CompareInternal(a, b) <= 0;
				/// <inheritdoc/>
				public static bool operator ==({{TickTimeName}} a, {{TickTimeName}} b) => CompareInternal(a, b) == 0;
				/// <inheritdoc/>
				public static bool operator !=({{TickTimeName}} a, {{TickTimeName}} b) => CompareInternal(a, b) != 0;
				/// <inheritdoc/>
				public override partial string ToString();
				/// <inheritdoc/>
				public readonly override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj) => obj is {{TickTimeName}} e && Equals(e);
				/// <inheritdoc/>
				public static partial {{TickTimeName}} Min({{TickTimeName}} left, {{TickTimeName}} right);
				/// <inheritdoc/>
				public static partial {{TickTimeName}} Max({{TickTimeName}} left, {{TickTimeName}} right);
				/// <inheritdoc/>
				public readonly bool Equals({{TickTimeName}} other) => this == other;
				/// <inheritdoc/>
			#if NETSTANDARD
				public override int GetHashCode()
				{
					int hash = 17;
					hash = hash * 31 + (_calculator?.GetHashCode() ?? 0);
					hash = hash * 31 + _isTickLoaded.GetHashCode();
					hash = hash * 31 + _isTimeSpanLoaded.GetHashCode();
					hash = hash * 31 + _tick.GetHashCode();
					hash = hash * 31 + _TimeSpan.GetHashCode();
					return hash;
				}
			#else
				public override int GetHashCode() => HashCode.Combine(Tick, BaseChart);
			#endif
				/// <inheritdoc/>
				public readonly int CompareTo({{TickTimeName}} other) => this > other ? 1 : this == other ? 0 : -1;

				internal {{Calculator}}? _calculator;
				private bool _isTickLoaded;
				private bool _isTimeSpanLoaded;
				private bool _isBPMLoaded;
				private float _tick;
				private TimeSpan _TimeSpan;
				private float _BPM;
			}
			""";
			string srcCalculator = $$"""
			// <auto-generated/>
			#nullable enable
			namespace {{calculatorSymbol.ContainingNamespace.ToDisplayString()}};

			internal record struct BpmCache(float Tick, TimeSpan TimeSpan, float Bpm) : IComparable<BpmCache>
			{
				public static readonly BpmCache Default = new(1, TimeSpan.Zero, RhythmBase.{{registryId}}.Constants.DefaultBpm);
				public readonly int CompareTo(BpmCache other) => Tick.CompareTo(other.Tick);
			}

			/// <summary>
			/// Beat calculator that converts between absolute ticks and time spans, while reacting to tempo and BPM changes.
			/// </summary>
			public partial class {{calculatorSymbol.Name}}
			{
				internal readonly {{Chart}} Collection;
				private BpmCache[] _bpmCache = [];
				internal {{calculatorSymbol.Name}}({{Chart}} chart)
				{
					Collection = chart;
				}
				internal void AddBpmAt(BpmCache bpm)
				{
					if (_bpmCache.Length == 0)
					{
						_bpmCache = [bpm];
						return;
					}
					int index = _bpmCache.BinarySearch(bpm);
					if (index < 0) index = ~index - 1;

					BpmCache a, b;
					if (index < 0) a = BpmCache.Default;
					else a = _bpmCache[index];
					if (a.Tick == bpm.Tick && (a.Bpm == bpm.Bpm)) return;
					if (index >= _bpmCache.Length - 1)
					{
						_bpmCache = [.. _bpmCache, bpm];
						return;
					}
					else
					{
						b = _bpmCache[index + 1];
						TimeSpan diff = TimeSpan.FromMinutes((b.Tick - bpm.Tick) / bpm.Bpm) - (b.TimeSpan - bpm.TimeSpan);
						for (int i = index + 1; i < _bpmCache.Length; ++i)
						{
							BpmCache ti = _bpmCache[i];
							ti.TimeSpan += diff;
							_bpmCache[i] = ti;
						}
						if (a.Tick != bpm.Tick)
							_bpmCache = [.. _bpmCache[..(index + 1)], bpm, .. _bpmCache[(index + 1)..]];
						else if (index < 0)
							_bpmCache = [bpm, .. _bpmCache];
						else
							_bpmCache[index] = bpm;
						return;
					}
				}
				internal void RemoveBpmAt(BpmCache bpm)
				{
					if (_bpmCache.Length == 0)
						return;
					int index = _bpmCache.BinarySearch(bpm);
					if (index < 0) return;
					BpmCache a, b;
					if (index == 0) a = BpmCache.Default;
					else a = _bpmCache[index - 1];
					if (index == _bpmCache.Length - 1)
					{
						_bpmCache = [.. _bpmCache[..(_bpmCache.Length - 1)]];
						return;
					}
					else
					{
						b = _bpmCache[index + 1];
						TimeSpan diff = TimeSpan.FromMinutes((b.Tick - bpm.Tick) / a.Bpm) - (b.TimeSpan - bpm.TimeSpan);
						for (int i = index + 1; i < _bpmCache.Length; ++i)
						{
							BpmCache ti = _bpmCache[i];
							ti.TimeSpan += diff;
							_bpmCache[i] = ti;
						}
						_bpmCache = [.. _bpmCache[..index], .. _bpmCache[(index + 1)..]];
						return;
					}
				}
				/// <summary>
				/// Refreshes the caches so subsequent conversions use up-to-date values.
				/// </summary>
				public partial void Refresh();

				/// <summary>
				/// Converts an absolute tick position to an absolute timespan using the current cache state.
				/// </summary>
				/// <param name="tick">The absolute tick position.</param>
				/// <returns>The corresponding absolute time.</returns>
				public TimeSpan TickToTimeSpan(float tick) => TickToTimeSpan(tick, in _bpmCache);
							
				/// <summary>
				/// Converts an absolute timespan to an absolute beat position using the current cache state.
				/// </summary>
				/// <param name="timeSpan">The absolute time to convert.</param>
				/// <returns>The corresponding absolute beat.</returns>
				public float TimeSpanToTick(TimeSpan timeSpan) => TimeSpanToTick(timeSpan, in _bpmCache);

				private static TimeSpan TickToTimeSpan(float tick, in BpmCache[] cacheSet)
				{
					BpmCache last = BpmCache.Default;
					foreach (BpmCache cache in cacheSet)
					{
						float cbeat = cache.Tick;
						if (cbeat < tick)
						{
							last = cache;
							continue;
						}
						if (cbeat == tick)
							return cache.TimeSpan;
						break;
					}
					float durationFromLast = (tick - last.Tick) / last.Bpm;
					TimeSpan result = last.TimeSpan + TimeSpan.FromMinutes(durationFromLast);
					return result;
				}
				private static float TimeSpanToTick(TimeSpan timeSpan, in BpmCache[] cacheSet)
				{
					BpmCache last = BpmCache.Default;
					foreach (BpmCache cache in cacheSet)
					{
						TimeSpan ctime = cache.TimeSpan;
						if (ctime < timeSpan)
						{
							last = cache;
							continue;
						}
						if (ctime == timeSpan)
							return cache.Tick;
						break;
					}
					float tick = last.Tick + (float)((timeSpan - last.TimeSpan).TotalMinutes * last.Bpm);
					return tick;
				}

				/// <summary>
				/// Creates an <see cref="{{TickTime}}"/> from an absolute tick value.
				/// </summary>
				/// <param name="tick">The absolute tick position.</param>
				/// <returns>An <see cref="{{TickTime}}"/> bound to this calculator.</returns>
				public {{TickTime}} TickOf(float tick) => new(this, tick);

				/// <summary>
				/// Creates an <see cref="{{TickTime}}"/> from an absolute timespan.
				/// </summary>
				/// <param name="timeSpan">The absolute time to convert.</param>
				/// <returns>An <see cref="{{TickTime}}"/> bound to this calculator.</returns>
				public {{TickTime}} TickOf(TimeSpan timeSpan) => new(this, timeSpan);

				/// <summary>
				/// Creates a <see cref="{{TickTimeRange}}"/> representing the interval between two ticks.
				/// </summary>
				/// <param name="tick1">The starting tick.</param>
				/// <param name="tick2">The ending tick.</param>
				/// <returns>The resulting interval.</returns>
				public {{TickTimeRange}} IntervalOf({{TickTime}} tick1, {{TickTime}} tick2) => new(new(this, tick1), new(this, tick2));

				/// <summary>
				/// Creates a <see cref="{{TickTimeRange}}"/> representing the interval between two timespans.
				/// </summary>
				/// <param name="timeSpan1">The start time.</param>
				/// <param name="timeSpan2">The end time.</param>
				/// <returns>The resulting interval.</returns>
				public {{TickTimeRange}} IntervalOf(TimeSpan timeSpan1, TimeSpan timeSpan2) => IntervalOf(TickOf(timeSpan1), TickOf(timeSpan2));
				/// <summary>
				/// Gets the BPM in effect at the specified tick.
				/// </summary>
				/// <param name="beat">The tick whose BPM should be retrieved.</param>
				/// <returns>The BPM active at the tick.</returns>
				public float BeatsPerMinuteOf({{TickTime}} beat)
				{
					BpmCache last = BpmCache.Default;
					foreach (BpmCache cache in _bpmCache)
					{
						float cbeat = cache.Tick;
						if (cbeat < beat.Tick)
						{
							last = cache;
							continue;
						}
						if (cbeat == beat.Tick)
							return cache.Bpm;
						break;
					}
					return last.Bpm;
				}
			}
			""";
			string srcTickRange = $$"""
			// <auto-generated/>
			namespace {{tickTimeSymbol.ContainingNamespace.ToDisplayString()}};

			/// <summary>
			/// Beat range.
			/// </summary>
			public struct {{TickRangeName}}
			{
				/// <summary>
				/// Start beat.
				/// </summary>
				public {{TickTime}}? Start { get; }
				/// <summary>
				/// End beat.
				/// </summary>
				public {{TickTime}}? End { get; }
				/// <summary>
				/// Beat interval.
				/// </summary>
				public readonly float BeatInterval
				{
					get
					{
						float BeatInterval = Start is {{TickTime}} startNotNull && End is {{TickTime}} endNotNull
								? endNotNull.Tick - startNotNull.Tick
								: float.PositiveInfinity;
						return BeatInterval;
					}
				}
				/// <summary>
				/// Time interval.
				/// </summary>
				public readonly TimeSpan TimeInterval
				{
					get
					{
						bool flag = Start != null && End != null;
						TimeSpan TimeInterval;
						if (flag)
						{
							if (Start!.Value.Tick == End!.Value.Tick)
							{
								TimeInterval = TimeSpan.Zero;
							}
							else
							{
								TimeInterval = End.Value.TimeSpan - Start.Value.TimeSpan;
							}
						}
						else
						{
							TimeInterval = TimeSpan.MaxValue;
						}
						return TimeInterval;
					}
				}
				/// <summary>
				/// Initializes a new instance of the <see cref="{{TickRangeName}}"/> struct with the specified start and end beats.
				/// </summary>
				/// <param name="start">Start beat.</param>
				/// <param name="end">End beat.</param>
				public {{TickRangeName}}({{TickTime}}? start, {{TickTime}}? end)
				{
					this = default;
					if (start != null && end != null && !(({{TickTime}})start).FromSameChartOrNull(({{TickTime}})end))
					{
						throw new InvalidOperationException("RDIndexes must come from the same RDLevel.");
					}
					if (start != null && end != null && start > end)
					{
						Start = end;
						End = start;
					}
					else
					{
						Start = start;
						End = end;
					}
				}
				/// <summary>
				/// Determines whether the specified beat is within the range.
				/// </summary>
				/// <param name="b">The beat to check.</param>
				/// <returns>True if the beat is within the range; otherwise, false.</returns>
				public readonly bool Contains({{TickTime}} b) => (Start == null || Start <= b) && (End == null || b < End);
				/// <summary>
				/// Computes the intersection of the current range with another specified range.
				/// </summary>
				/// <remarks>The intersection of two ranges is the range that contains all elements common to both ranges.  If
				/// either range is unbounded (i.e., has a null start or end), the resulting range will reflect  the bounds of the
				/// other range where applicable. If the resulting range is invalid  (i.e., the start is greater than the end), an
				/// empty range is returned.</remarks>
				/// <param name="other">The range to intersect with the current range.</param>
				/// <returns>A new <see cref="{{TickRangeName}}"/> representing the intersection of the two ranges.  If the ranges do not overlap,
				/// returns an empty range.</returns>
				public readonly {{TickRangeName}} Intersect({{TickRangeName}} other)
				{
					{{TickTime}}? newStart;
					if (Start == null || (other.Start != null && other.Start > Start))
						newStart = other.Start;
					else
						newStart = Start;
					{{TickTime}}? newEnd;
					if (End == null || (other.End != null && other.End < End))
						newEnd = other.End;
					else
						newEnd = End;
					return newStart != null && newEnd != null && newStart > newEnd ? Empty : new {{TickRangeName}}(newStart, newEnd);
				}
				/// <summary>
				/// Creates a new <see cref="{{TickRangeName}}"/> that represents the union of the current range and the specified range.
				/// </summary>
				/// <remarks>The union operation considers null values for the start or end of a range as unbounded.  If both
				/// ranges have null start or end values, the resulting range will also have null for those bounds.</remarks>
				/// <param name="other">The <see cref="{{TickRangeName}}"/> to combine with the current range.</param>
				/// <returns>A new <see cref="{{TickRangeName}}"/> that spans from the earliest start point to the latest end point of the two ranges. If
				/// either range has a null start or end, the resulting range will use the non-null value, if available.</returns>
				public readonly {{TickRangeName}} Union({{TickRangeName}} other)
				{
					{{TickTime}}? newStart;
					if (Start == null || (other.Start != null && other.Start > Start))
						newStart = Start;
					else
						newStart = other.Start;
					{{TickTime}}? newEnd;
					if (End == null || (other.End != null && other.End < End))
						newEnd = End;
					else
						newEnd = other.End;
					return new {{TickRangeName}}(newStart, newEnd);
				}
				/// <summary>
				/// Gets the start and end beats for the current range, using the specified default values if not explicitly
				/// set.
				/// </summary>
				/// <param name="endTime">The default end time to use if the current range does not specify an end value.</param>
				/// <returns>A tuple containing the start and end beats. The start beat is set to the current range's start value if defined;
				/// otherwise, it defaults to 1. The end beat is set to the current range's end value if
				/// defined; otherwise, it defaults to <paramref name="endTime"/>.</returns>
				public readonly ({{TickTime}} Start, {{TickTime}} End) GetStartAndEnd({{TickTime}} endTime)
						=> (Start is {{TickTime}} startNotNull ? startNotNull : new(1), End is {{TickTime}} endNotNull ? endNotNull : endTime);
				/// <summary>
				/// Gets a range that represents an infinite range with no upper or lower bounds.
				/// </summary>
				/// <remarks>This property can be used to represent a range that is unbounded in both directions.</remarks>
				public static {{TickRangeName}} Infinity => new(null, null);
				/// <summary>
				/// Gets an empty range with no defined start or end values.
				/// </summary>
				public static {{TickRangeName}} Empty => new(new(), new());
			}
			
			""";
			string srcOtherFiles = $$"""
			// <auto-generated/>
			#nullable enable
			namespace RhythmBase.{{registryId}}.Components;


			/// <summary>
			/// Represents a collection of events that can be enumerated.
			/// </summary>
			/// <typeparam name="TEvent">The type of events in the collection. Must implement <see cref="{{IBaseEvent}}"/>.</typeparam>
			public interface IEventEnumerable<out TEvent>
					: IEnumerable<TEvent>
					where TEvent : {{IBaseEvent}}
			{
				/// <summary>
				/// Gets the events organized by beat order.
				/// </summary>
				RhythmBase.Global.Components.RedBlackTree<{{TickTime}}, TypedEventCollection> EventsBeatOrder { get; }
				/// <summary>
				/// Gets the bounds of the events in the collection.
				/// </summary>
				{{TickTimeRange}} Range { get; }
				/// <summary>
				/// Gets the type of events in the collection.
				/// </summary>
				RhythmBase.Global.Components.ReadOnlyEnumCollection<{{EventType}}> Types { get; }
			}


			/// <summary>
			/// Represents a collection of typed events.
			/// </summary>
			public class TypedEventCollection : IEnumerable<{{IBaseEvent}}>
			{
				/// <summary>
				/// Gets the number of events in the collection.
				/// </summary>
				public int Count => list.Count;
				/// <inheritdoc />
				public RhythmBase.Global.Components.ReadOnlyEnumCollection<{{EventType}}> Types => _types.AsReadOnly();
				/// <summary>
				/// Initializes a new instance of the <see cref="TypedEventCollection"/> class.
				/// </summary>
				public TypedEventCollection() { }
				/// <summary>
				/// Adds an event to the collection.
				/// </summary>
				/// <param name="item">The event to add.</param>
				/// <returns>true if the event was added; otherwise, false if it already exists.</returns>
				public virtual bool Add({{IBaseEvent}} item)
				{
					if (list.Contains(item))
						return false;
					list.Add(item);
					_types.Add(item.Type);
					return true;
				}
				/// <summary>
				/// Removes an event from the collection.
				/// </summary>
				/// <param name="item">The event to remove.</param>
				/// <returns>true if the event was removed; otherwise, false.</returns>
				public virtual bool Remove({{IBaseEvent}} item)
				{
					bool result = list.Remove(item);
					if (!result)
						return false;
					if (!list.Any(i => i.Type == item.Type))
						_types.Remove(item.Type);
					return true;
				}
				/// <summary>Determines whether this bucket contains any event of the specified type.</summary>
				[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
				public bool ContainsType({{EventType}} type) => _types.Contains(type);
				/// <summary>Determines whether this bucket contains any event of the specified types.</summary>
				[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
				public bool ContainsTypes({{EventType}}[] types) => _types.ContainsAny(types);
				/// <summary>Determines whether this bucket contains any event matching the given type collection.</summary>
				[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
				public bool ContainsTypes(ReadOnlyEnumCollection<{{EventType}}> types) => _types.ContainsAny(types);
				/// <summary>Compares the insertion order of two events within this bucket.</summary>
				[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
				public bool CompareTo({{IBaseEvent}} item1, {{IBaseEvent}} item2) =>
						list.IndexOf(item1) < list.IndexOf(item2);
				/// <summary>
				/// Returns a string that represents the current collection.
				/// </summary>
				/// <returns>A string that represents the current collection.</returns>
				public override string ToString()
				{
					string result = $"Count={list.Count}";
					return result;
				}
				/// <summary>
				/// Returns an enumerator that iterates through the collection.
				/// </summary>
				/// <returns>An enumerator that can be used to iterate through the collection.</returns>
				public IEnumerator<{{IBaseEvent}}> GetEnumerator() =>
						list.GetEnumerator();
				System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
						list.GetEnumerator();
				private readonly List<{{IBaseEvent}}> list = [];
				private readonly RhythmBase.Global.Components.EnumCollection<{{EventType}}> _types = [];
			}

			/// <summary>
			/// Provides an enumerator over events in a <see cref="RhythmBase.Global.Components.RedBlackTree{TKey, TValue}"/> collection, filtered by
			/// event types and an optional tick range.
			/// </summary>
			/// <typeparam name="TEvent">The event interface type to enumerate.</typeparam>
			public class EventEnumerator<TEvent>(RhythmBase.Global.Components.RedBlackTree<{{TickTime}}, TypedEventCollection> collection, RhythmBase.Global.Components.ReadOnlyEnumCollection<{{EventType}}> types, {{TickTimeRange}} range)
					: IEventEnumerable<TEvent>, IEnumerator<TEvent>
				where TEvent : {{IBaseEvent}}
			{
				/// <summary>
				/// The enumerator over beat-keyed buckets in the tree.
				/// </summary>
				protected readonly IEnumerator<KeyValuePair<{{TickTime}}, TypedEventCollection>> beats = collection.GetEnumerator();
				/// <summary>
				/// The enumerator over events within the current bucket, or <c>null</c> if no bucket is active.
				/// </summary>
				protected IEnumerator<{{IBaseEvent}}>? events;
				/// <summary>
				/// The underlying tree collection.
				/// </summary>
				protected readonly RhythmBase.Global.Components.RedBlackTree<{{TickTime}}, TypedEventCollection> collection = collection;
				private RhythmBase.Global.Components.ReadOnlyEnumCollection<{{EventType}}> types = types;
				private {{TickTimeRange}} range = range;
				RhythmBase.Global.Components.ReadOnlyEnumCollection<{{EventType}}> IEventEnumerable<TEvent>.Types => types;
				{{TickTimeRange}} IEventEnumerable<TEvent>.Range => range;
				/// <inheritdoc/>
				public TEvent Current => ((TEvent?)events?.Current) ?? throw new InvalidOperationException();
				/// <inheritdoc/>
				object System.Collections.IEnumerator.Current => Current;
				RhythmBase.Global.Components.RedBlackTree<{{TickTime}}, TypedEventCollection> IEventEnumerable<TEvent>.EventsBeatOrder => collection;
				/// <inheritdoc/>
				public bool MoveNext()
				{
					if (events != null)
					{
						while (events.MoveNext())
						{
							if (types.Contains(events.Current.Type))
								return true;
						}
						events = null;
					}
					while (beats.MoveNext())
					{
						var currentKey = beats.Current.Key;

						if (range.Start.HasValue && currentKey.CompareTo(range.Start.Value) < 0)
							continue;
						if (range.End.HasValue && currentKey.CompareTo(range.End.Value) >= 0)
							return false;
						if (!beats.Current.Value.ContainsTypes(types))
							continue;
						events = beats.Current.Value.GetEnumerator();
						while (events.MoveNext())
						{
							if (types.Contains(events.Current.Type))
								return true;
						}
						events = null;
					}
					return false;
				}
				private bool _disposed = false;
				/// <summary>
				/// Disposes the enumerator, releasing any resources held by the beat and event enumerators.
				/// </summary>
				/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
				protected virtual void Dispose(bool disposing)
				{
					if (_disposed) return;
					if (disposing)
					{
						beats.Dispose();
						events?.Dispose();
					}
					_disposed = true;
				}
				/// <inheritdoc/>
				public void Dispose()
				{
					Dispose(true);
					GC.SuppressFinalize(this);
				}
				/// <inheritdoc/>
				public void Reset() => throw new NotSupportedException();
				/// <summary>
				/// Narrows the enumeration to events of the specified target type, intersecting the current type filter.
				/// </summary>
				/// <typeparam name="TTarget">The more specific event type to filter for.</typeparam>
				/// <returns>A new <see cref="IEventEnumerable{TTarget}"/> with the narrowed filter.</returns>
				public IEventEnumerable<TTarget> OfEvent<TTarget>() where TTarget : {{IBaseEvent}}
				{
					RhythmBase.Global.Components.ReadOnlyEnumCollection<{{EventType}}> types = this.types.Intersect(RhythmBase.{{registryId}}.Converters.EventTypeRegistry.ToEnums{{(registryGenerationInfos.Length > 1 ? typeEnumSymbol.Name : "")}}<TTarget>());
					this.types = types;
					return new EventEnumerator<TTarget>(collection, types, range);
				}
				/// <summary>
				/// Replaces the current type filter with the specified collection of event types.
				/// </summary>
				/// <param name="types">The event types to enumerate.</param>
				/// <returns>This enumerator with the updated filter.</returns>
				public IEventEnumerable<{{IBaseEvent}}> OfEvents(RhythmBase.Global.Components.ReadOnlyEnumCollection<{{EventType}}> types)
				{
					this.types = types;
					return new EventEnumerator<{{IBaseEvent}}>(collection, types, range);
				}
				/// <summary>
				/// Narrows the enumeration to the specified tick range, intersecting with the current range.
				/// </summary>
				/// <param name="range">The tick range to filter for.</param>
				/// <returns>This enumerator with the narrowed range.</returns>
				public IEventEnumerable<TEvent> InRange({{TickTimeRange}} range)
				{
					this.range = this.range.Intersect(range);
					return this;
				}
				/// <summary>
				/// Returns all events at the specified beat that match the current type filter.
				/// </summary>
				/// <param name="beat">The beat at which to retrieve events.</param>
				/// <returns>An enumerable of matching events at the given beat.</returns>
				public IEnumerable<TEvent> AtBeat({{TickTime}} beat)
				{
					if (!range.Contains(beat))
						yield break;
					if (!collection.TryGetValue(beat, out var events))
						yield break;
					if (!events.ContainsTypes(types))
						yield break;
					foreach (var ev in events)
						if (types.Contains(ev.Type))
							yield return (TEvent)ev;
				}
				/// <inheritdoc/>
				public IEnumerator<TEvent> GetEnumerator() => this;
				/// <inheritdoc/>
				System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
			}

			/// <summary>
			/// Represents a collection of ordered events.
			/// </summary>
			/// <typeparam name="TEvent">The type of event.</typeparam>
			public abstract class OrderedEventCollection<TEvent> : ICollection<TEvent>, IEventEnumerable<TEvent>
					where TEvent : {{IBaseEvent}}
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="OrderedEventCollection{TEvent}"/> class.
				/// </summary>
				public OrderedEventCollection()
				{
					EventsBeatOrder = [];
					IsReadOnly = false;
				}
				/// <summary>
				/// Concatenates all events in the collection.
				/// </summary>
				/// <returns>An <see cref="IEnumerable{TEvent}"/> that contains all events in the collection.</returns>
				public IEnumerable<TEvent> ConcatAll() => EventsBeatOrder.SelectMany(i => i.Value).Cast<TEvent>();
				void ICollection<TEvent>.Add(TEvent item) => Add(item);
				/// <inheritdoc/>  
				public override string ToString() => $"Count = {Count}";
				///// <inheritdoc/>
				//public override IEnumerator<{{IBaseEvent}}> GetEnumerator() => (IEnumerator<{{IBaseEvent}}>)new EventEnumerator<TEvent>(this);
				/// <inheritdoc/>  
				public IEnumerator<TEvent> GetEnumerator()
				{
					foreach (KeyValuePair<{{TickTime}}, TypedEventCollection> pair in EventsBeatOrder)
						foreach (TEvent item in pair.Value)
							yield return item;
				}
				/// <summary>
				/// Gets the total count of events in the collection.
				/// </summary>
				public virtual int Count => EventsBeatOrder.Sum(i => i.Value.Count);
				/// <summary>
				/// Gets a value indicating whether the collection is read-only.
				/// </summary>
				public bool IsReadOnly { get; }
				/// <summary>
				/// Returns the beat of the last event.
				/// </summary>
				/// <returns>The beat of the last event.</returns>
				public {{TickTime}} Duration => EventsBeatOrder.LastOrDefault().Key;

				/// <summary>
				/// Initializes a new instance of the <see cref="OrderedEventCollection{TEvent}"/> class with the specified items.
				/// </summary>
				/// <param name="items">The items to add to the collection.</param>
				public OrderedEventCollection(IEnumerable<TEvent> items)
				{
					EventsBeatOrder = [];
					IsReadOnly = false;
					foreach (TEvent item in items)
						Add(item);
				}
				/// <summary>
				/// Adds an event to the collection.
				/// </summary>
				/// <param name="item">The event to add.</param>
				public virtual bool Add(TEvent item)
				{
					if (EventsBeatOrder.TryGetValue(item.TickTime, out TypedEventCollection? value))
						return value.Add(item);
					EventsBeatOrder.Insert(item.TickTime, [item]);
					return true;
				}
				/// <summary>
				/// Clears all events from the collection.
				/// </summary>
				public void Clear() => EventsBeatOrder.Clear();
				/// <summary>
				/// Determines whether the collection contains a specific event.
				/// </summary>
				/// <param name="item">The event to locate in the collection.</param>
				/// <returns>true if the event is found in the collection; otherwise, false.</returns>
				public virtual bool Contains(TEvent item) => EventsBeatOrder.FindNode(item.TickTime)?.Value.Contains(item) ?? false;
				/// <summary>
				/// Copies the elements of the collection to an array, starting at a particular array index.
				/// </summary>
				/// <param name="array">The array to copy the elements to.</param>
				/// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
				public void CopyTo(TEvent[] array, int arrayIndex)
				{
					if (arrayIndex < 0 || arrayIndex > array.Length)
						throw new ArgumentOutOfRangeException(nameof(arrayIndex));
					if (array.Length - arrayIndex < Count)
						throw new ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");
					foreach (KeyValuePair<{{TickTime}}, TypedEventCollection> pair in EventsBeatOrder)
					{
						foreach (TEvent item in pair.Value)
						{
							array[arrayIndex++] = item;
						}
					}
				}
				/// <summary>
				/// Removes the first occurrence of a specific event from the collection.
				/// </summary>
				/// <param name="item">The event to remove from the collection.</param>
				/// <returns>true if the event was successfully removed; otherwise, false.</returns>
				public virtual bool Remove(TEvent item)
				{
					bool Remove;
					if (Contains(item))
					{
						bool result = EventsBeatOrder[item.TickTime].Remove(item);
						if (EventsBeatOrder[item.TickTime].Count == 0)
							EventsBeatOrder.Remove(item.TickTime);
						Remove = result;
					}
					else
						Remove = false;
					return Remove;
				}
				internal IEnumerator<TEvent> GetEnumerator({{TickTime}}? start, {{TickTime}}? end) => new EventEnumerator<TEvent>(EventsBeatOrder, Types, new {{TickTimeRange}}(start, end));
				/// <summary>
				/// Returns an enumerator that iterates through the collection.
				/// </summary>
				/// <returns>An enumerator for the collection.</returns>
				System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
				/// <summary>
				/// Removes the first occurrence of a specific event from the collection.
				/// </summary>
				/// <param name="item">The event to remove from the collection.</param>
				/// <returns>true if the event was successfully removed; otherwise, false.</returns>
				bool ICollection<TEvent>.Remove(TEvent item) => throw new NotImplementedException();
				/// <summary>
				/// The dictionary that maintains the order of events based on their beats.
				/// </summary>
				[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
				public RhythmBase.Global.Components.RedBlackTree<{{TickTime}}, TypedEventCollection> EventsBeatOrder = [];
				RhythmBase.Global.Components.RedBlackTree<{{TickTime}}, TypedEventCollection> IEventEnumerable<TEvent>.EventsBeatOrder => EventsBeatOrder;
				/// <summary>Gets the collection of event types supported by this collection.</summary>
				public RhythmBase.Global.Components.ReadOnlyEnumCollection<{{EventType}}> Types => RhythmBase.{{registryId}}.Converters.EventTypeRegistry.ToEnums{{(registryGenerationInfos.Length > 1 ? typeEnumSymbol.Name : "")}}<TEvent>();
				{{TickTimeRange}} IEventEnumerable<TEvent>.Range => {{TickTimeRange}}.Infinity;
			}

			
			""";
			string srcConstants = $$"""
			// <auto-generated/>
			#nullable enable
			namespace RhythmBase.{{registryId}};

			/// <summary>
			/// Provides constant values in the game.
			/// </summary>
			public static partial class Constants
			{
				/// <summary>
				/// The default beats per minute for a new chart.
				/// </summary>
				public static partial float DefaultBpm { get; }
			}
			""";
			context.AddSource($"{tickTimeSymbol.Name}.{registryId}.g.cs", srcTickTime);
			context.AddSource($"{calculatorSymbol.Name}.{registryId}.g.cs", srcCalculator);
			context.AddSource($"{TickRangeName}.{registryId}.g.cs", srcTickRange);
			context.AddSource($"OtherFiles.{registryId}.g.cs", srcOtherFiles);
			context.AddSource($"Constants.{registryId}.g.cs", srcConstants);

		});
	}

	private const string CoreNs = "Global";
	private void GenerateOtherFiles(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<(string?, List<Diagnostic>)> registryInfo)
	{
		context.RegisterSourceOutput(registryInfo, (context, registryInfoData) =>
		{
			var (registryId, diagnostics) = registryInfoData;
			foreach (var diag in diagnostics)
				context.ReportDiagnostic(diag);
			if (string.IsNullOrEmpty(registryId) || registryId == CoreNs)
				return;
			string src = $$"""
			using System.Text.Json;

			namespace RhythmBase.{{registryId}}.Converters;

			/// <summary>
			/// Provides entry point methods for deserializing and serializing level files
			/// for the <c>{{registryId}}</c> game adapter.
			/// </summary>
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
				/// <summary>
				/// Deserializes a level from the specified data source.
				/// </summary>
				/// <typeparam name="T">The level type to deserialize.</typeparam>
				/// <param name="dataSource">The JSON data source to read from.</param>
				/// <param name="options">The metadata-aware serializer options.</param>
				/// <returns>The deserialized level instance, or a new empty instance if deserialization fails.</returns>
				public static T DeserializeMainEntry<T>(RhythmBase.Global.Converters.JsonSerialization.IJsonDataSource dataSource, RhythmBase.Global.Converters.MetadataJsonSerializerOptions options)
						where T : new()
				{
					var seq = dataSource.GetSequence();
					Utf8JsonReader reader = seq.IsSingleSegment
						? new Utf8JsonReader(seq.First.Span, _readerOptions)
						: new Utf8JsonReader(seq, _readerOptions);
					return RhythmBase.{{registryId}}.Converters.TypeConverterRegistry.Read<T>(ref reader, options) ?? new();
				}
				/// <summary>
				/// Asynchronously deserializes a level from the specified data source.
				/// </summary>
				/// <typeparam name="T">The level type to deserialize.</typeparam>
				/// <param name="dataSource">The JSON data source to read from.</param>
				/// <param name="options">The metadata-aware serializer options.</param>
				/// <param name="cancellationToken">A token to cancel the operation.</param>
				/// <returns>The deserialized level instance, or a new empty instance if deserialization fails.</returns>
				public static async Task<T> DeserializeMainEntryAsync<T>(RhythmBase.Global.Converters.JsonSerialization.IJsonDataSource dataSource, RhythmBase.Global.Converters.MetadataJsonSerializerOptions options, CancellationToken cancellationToken = default)
						where T : new()
				{
					var seq = await dataSource.GetSequenceAsync(cancellationToken);
					Utf8JsonReader reader = seq.IsSingleSegment
						? new Utf8JsonReader(seq.First.Span, _readerOptions)
						: new Utf8JsonReader(seq, _readerOptions);
					return RhythmBase.{{registryId}}.Converters.TypeConverterRegistry.Read<T>(ref reader, options) ?? new();
				}
				/// <summary>
				/// Serializes a level to the specified stream.
				/// </summary>
				/// <typeparam name="T">The level type to serialize.</typeparam>
				/// <param name="mainEntry">The level instance to serialize.</param>
				/// <param name="stream">The output stream to write to.</param>
				/// <param name="options">The metadata-aware serializer options.</param>
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
	private static void GenerateUpgrater(IncrementalGeneratorInitializationContext cxt, IncrementalValueProvider<((string?, List<Diagnostic>) Left, EventTypeRegistryGenerationInfo[] Right)> incrementalValueProvider)
	{
		cxt.RegisterSourceOutput(incrementalValueProvider, (source, value) =>
		{
			((var registryId, var diagnostics), EventTypeRegistryGenerationInfo[]? gens) = value;
			foreach (var diag in diagnostics)
				source.ReportDiagnostic(diag);
			if (string.IsNullOrEmpty(registryId))
				return;
			bool multiple = gens?.Length > 1;
			StringBuilder sb = new();
			sb.AppendLine($"namespace RhythmBase.{registryId}.Converters;");
			foreach (var info in gens ?? [])
			{
				string mtpName = multiple ? $"{info.RootClassType.Name}" : "";
				string src = $$"""
				/// <summary>
				/// A JSON converter for <see cref="{{info.RootClassType.ToDisplayString()}}"/> that uses metadata-aware serializer options.
				/// </summary>
				internal abstract class BackwardCompatible{{mtpName}}MetadataJsonConverter : RhythmBase.Global.Converters.MetadataJsonConverter<{{info.RootClassType.ToDisplayString()}}>
				{
					protected class Upgrater
					{
						internal int MaxVersion { get; init; }
						internal required Action<{{info.RootClassType.ToDisplayString()}}> UpgrateFunc { get; init; }
						internal required {{info.ClassTypeEnum.ToDisplayString()}} Type { get; init; }
					}
					private readonly List<Upgrater> _upgraters = [];
					private readonly EnumCollection<{{info.ClassTypeEnum.ToDisplayString()}}> _typeHasUpgrater = [];
					private int _maxVersion;
					/// <summary>
					/// The maximum version that this converter can upgrade.
					/// </summary>
					internal int MaxVersion => _maxVersion;
					/// <summary>
					/// The types of events that this converter can upgrade.
					/// </summary>
					internal EnumCollection<{{info.ClassTypeEnum.ToDisplayString()}}> TypeHasUpgrater => _typeHasUpgrater;
					/// <summary>
					/// Registers an upgrader for a specific event type and version.
					/// </summary>
					/// <typeparam name="T">The type of the event to upgrade.</typeparam>
					/// <param name="version">
					/// The version for which to register the upgrader.
					/// Versions <b>equal to or lower than</b> this will be affected by this upgrader.
					/// </param>
					/// <param name="upgrateAction">The action to perform when upgrading the event.</param>
					protected void Register<T>(int version, Action<{{info.RootClassType.ToDisplayString()}}> upgrateAction) where T : {{info.RootClassType.ToDisplayString()}}, new()
					{
						var type = EventTypeRegistry.ToEnum{{(multiple ? info.ClassTypeEnum.Name : "")}}<T>();
						_maxVersion = int.Max(_maxVersion, version);
						_typeHasUpgrater.Add(type);
						_upgraters.Add(new Upgrater()
						{
							MaxVersion = version,
							Type = type,
							UpgrateFunc = upgrateAction
						});
					}
					/// <summary>
					/// Upgrades the specified event to the latest version if an upgrader is registered for its type and version.
					/// </summary>
					/// <param name="version">The version of the event to upgrade.</param>
					/// <param name="type">The type of the event to upgrade.</param>
					/// <returns>An enumerable of upgraders that can upgrade the event.</returns>
					protected IEnumerable<Upgrater> GetUpgraters(int version, {{info.ClassTypeEnum.ToDisplayString()}} type)
					{
						foreach (Upgrater upgrater in _upgraters)
							if (upgrater.Type == type && upgrater.MaxVersion >= version)
								yield return upgrater;
					}
					internal BackwardCompatible{{mtpName}}MetadataJsonConverter()
					{
						InitializeUpgraters();
					}
					/// <summary>
					/// Initializes the upgraders for this converter. This method is called once when the converter is first used.
					/// </summary>
					protected abstract void InitializeUpgraters();
				}
				""";
				sb.AppendLine(src);
			}
			source.AddSource($"Upgrader.{registryId}.g.cs", sb.ToString());
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