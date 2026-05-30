using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RhythmBase.Generator;

public partial class ConverterGenerator
{
	private static bool InheritsOrImplements(INamedTypeSymbol type, INamedTypeSymbol target)
	{
		static IEnumerable<INamedTypeSymbol> GetBaseTypes(INamedTypeSymbol type)
		{
			for (var current = type.BaseType;
					current != null && current.SpecialType != SpecialType.System_Object;
					current = current.BaseType)
			{
				yield return current;
			}
		}

		if (SymbolEqualityComparer.Default.Equals(type, target))
			return true;

		return target.TypeKind is TypeKind.Interface
				? type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, target))
				: GetBaseTypes(type).Any(i => SymbolEqualityComparer.Default.Equals(i, target));
	}
	private static void GenerateConverterRegistry(IncrementalGeneratorInitializationContext context)
	{
		var scans = context.SyntaxProvider.CreateSyntaxProvider(
				predicate: static (node, _) =>
						node is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
				transform: (ctx, _) =>
				{
					if (ctx.Node is not ClassDeclarationSyntax classDeclaration)
						return default(ConverterRegistryScanResult);

					if (ctx.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol symbol)
						return default;

					if (!HasAttribute(symbol, JsonConverterForAttrName))
						return default;

					Location? location = symbol.Locations.FirstOrDefault();
					string converterName = symbol.ToDisplayString();

					if (!IsJsonConverterType(symbol))
						return new(symbol, [], location, "must inherit System.Text.Json.Serialization.JsonConverter or JsonConverter<T>");

					if (symbol.IsAbstract)
						return new(symbol, [], location, "abstract converter cannot be registered");

					if (!HasAccessibleParameterlessCtor(symbol))
						return new(symbol, [], location, "converter must have an accessible parameterless constructor");

					HashSet<ITypeSymbol> targets = new(SymbolEqualityComparer.Default);
					foreach (var attr in symbol.GetAttributes().Where(i => IsAttribute(i, JsonConverterForAttrName)))
					{
						if (attr.ConstructorArguments.Length == 0)
							continue;

						TypedConstant arg0 = attr.ConstructorArguments[0];
						if (arg0.Kind == TypedConstantKind.Type && arg0.Value is ITypeSymbol targetType)
							targets.Add(targetType);
					}

					if (targets.Count == 0)
						return new(symbol, [], location, "at least one valid target type is required");

					return new(symbol, [.. targets], location, null);
				}).Collect();

		context.RegisterSourceOutput(scans, (spc, results) =>
		{
			List<(ITypeSymbol TargetType, INamedTypeSymbol ConverterType, Location? Location)> registrations = [];

			foreach (var result in results)
			{
				if (result.ConverterType is null)
					continue;

				if (result.Error is string err)
				{
					spc.ReportDiagnostic(Diagnostic.Create(
									InvalidConverterRegistrationRule,
									result.Location,
									result.ConverterType.ToDisplayString(),
									err));
					continue;
				}

				foreach (var targetType in result.TargetTypes)
					registrations.Add((targetType, result.ConverterType, result.Location));
			}

			foreach (var group in registrations.GroupBy(i => i.TargetType, SymbolEqualityComparer.Default).Distinct())
			{
				if (group.Count() <= 1)
					continue;

				string converterList = string.Join(", ", group.Select(i => i.ConverterType.ToDisplayString()).Distinct().OrderBy(i => i));
				foreach (var item in group)
				{
					spc.ReportDiagnostic(Diagnostic.Create(
									DuplicateConverterRegistrationRule,
									item.Location,
									group.Key?.ToDisplayString(),
									converterList));
				}
			}

			var validRegistrations = registrations
							.GroupBy(i => i.TargetType, SymbolEqualityComparer.Default)
							.Where(i => i.Count() == 1)
							.Select(i => i.First())
							.OrderBy(i => i.TargetType.ToDisplayString())
							.ToArray();

			StringBuilder sb = new();
			sb.AppendLine("""
			namespace RhythmBase.Global.Converters;

			partial class ConverterHub
			{
				private static void InitializeConverters()
				{
			""");
			for (int i = 0; i < validRegistrations.Length; i++)
			{
				(ITypeSymbol TargetType, INamedTypeSymbol ConverterType, Location? Location) reg = validRegistrations[i];
				sb.AppendLine($"\tConverterCache<{reg.TargetType.ToDisplayString()}>.Converter = new {reg.ConverterType.ToDisplayString()}();");
			}
			sb.AppendLine("""
				}
				private static int idOf<T>()
				{
			""");
			for (int i = 0; i < validRegistrations.Length; i++)
			{
				(ITypeSymbol TargetType, INamedTypeSymbol ConverterType, Location? Location) reg = validRegistrations[i];
				sb.AppendLine($"\t\tif (typeof(T) == typeof({reg.TargetType.ToDisplayString()})) return {i};");
			}
			sb.AppendLine("""
					return -1;
				}
			}
			""");

			spc.AddSource("GeneratedEntityConverterHub.g.cs", sb.ToString());
		});
	}
	private static string ReplaceVariableWithName(string template)
	{
		return template
			.Replace("$&", "value")
			.Replace("$r", "_rs")
			.Replace("$w", "_ws");
	}
	private static void GenerateClassEnumMap(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<(string, ClassEnumMapGenerationInfo[])> registryInfo, HashSet<Diagnostic> errors)
	{
		context.RegisterSourceOutput(registryInfo, GenerateEventTypeUtils);
	}
	private static void GenerateEnumConverter(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<string?> registryInfo)
	{
		var enums2 = context.SyntaxProvider.ForAttributeWithMetadataName(
				JsonEnumAttrName,
				predicate: (s, e) => s is EnumDeclarationSyntax enumDeclaration,
				transform: (ctx, e) =>
				{
					if (ctx.TargetNode is not EnumDeclarationSyntax enumDeclaration)
						return default;
					INamedTypeSymbol? symbol = ctx.SemanticModel.GetDeclaredSymbol(enumDeclaration);
					if (symbol is null)
						return default;
					var members = symbol.GetMembers().OfType<IFieldSymbol>().ToArray();
					return (symbol, members);
				}
				).Collect();

		context.RegisterSourceOutput(registryInfo.Combine(enums2), (spc, registryInfoAndEnumSymbols) =>
		{
			(var registryInfo, var enumSymbols) = registryInfoAndEnumSymbols;
			if (string.IsNullOrEmpty(registryInfo))
				return;
			StringBuilder sb1 = new();
			StringBuilder sb2 = new();
			sb1.AppendLine($$"""
				
					// <auto-generated/>
					#nullable enable
					using System;
					using System.Runtime.CompilerServices;
				
					namespace RhythmBase.{{registryInfo}}.Converters;
					public static class EnumConverterExtensions
					{
						/// <summary>
						/// Provides extension methods for converting enums to and from string representations.
						/// </summary>
						extension(RhythmBase.Global.Converters.EnumConverter)
						{
					""");
			foreach (var e in enumSymbols.OrderBy(i => i.symbol.Name))
			{
				static string GetStringName(IFieldSymbol symbol)
				{
					string? alias = symbol.GetAttributes().FirstOrDefault(a => IsAttribute(a, JsonAliasAttrName) && a.ConstructorArguments.Length == 1)?.ConstructorArguments[0].Value as string;
					if (alias is null)
						return symbol.Name; //ToLowerCamelCase(symbol.Name);
					else
						return alias;
				}
				string fullName = e.symbol.ToDisplayString();

				// TryParse(string)
				sb1.AppendLine($$"""
							/// <summary>
							/// Attempts to parse the specified string value to a <see cref="{{fullName}}"/> enum value.
							/// </summary>
							/// <param name="value">The string representation of the enum value.</param>
							/// <param name="result">When this method returns, contains the parsed <see cref="{{fullName}}"/> value if parsing succeeded; otherwise, the default value.</param>
							/// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
							[MethodImpl(MethodImplOptions.AggressiveInlining)]
							public static bool TryParse(string? value, out {{fullName}} result)
							{
								switch(value) {
					""");
				foreach (var field in e.members.OrderBy(i => i.Name))
				{
					sb1.AppendLine($$"""
									case "{{GetStringName(field)}}":
										result = {{field.ToDisplayString()}}; return true;
					""");
				}
				sb1.AppendLine("""
									default:
										result = default; return false;
								}
							}
					
					""");
				sb1.AppendLine();

				// TryParse(ReadOnlySpan<byte>)
				sb1.AppendLine($$"""
							/// <summary>
							/// Attempts to parse the specified UTF-8 byte span to a <see cref="{{fullName}}"/> enum value.
							/// </summary>
							/// <param name="value">The UTF-8 byte span representing the enum value.</param>
							/// <param name="result">When this method returns, contains the parsed <see cref="{{fullName}}"/> value if parsing succeeded; otherwise, the default value.</param>
							/// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
							[MethodImpl(MethodImplOptions.AggressiveInlining)]
							public static bool TryParse(ReadOnlySpan<byte> value, out {{fullName}} result)
							{
					""");
				bool isFirst = true;
				foreach (var field in e.members.OrderBy(i => i.Name))
				{
					sb1.AppendLine($$"""
								{{(isFirst ? "" : "else ")}}if (value.SequenceEqual("{{GetStringName(field)}}"u8))
								{
									result = {{field.ToDisplayString()}};
									return true;
								}
					""");
					isFirst = false;
				}
				sb1.AppendLine("""
								else
								{
									result = default;
									return false;
								}
							}

					""");

				// ToEnumString
				sb2.AppendLine($$"""
						/// <summary>
						/// Converts the <see cref="{{fullName}}"/> enum value to its string representation.
						/// </summary>
						/// <param name="value">The enum value to convert.</param>
						/// <returns>The string representation of the enum value.</returns>
						[MethodImpl(MethodImplOptions.AggressiveInlining)]
						public static string ToEnumString(this {{fullName}} value) => value switch
						{
					""");
				foreach (var field in e.members.OrderBy(i => i.Name))
				{
					sb2.AppendLine($"""
								{field.ToDisplayString()} => "{GetStringName(field)}",
					""");
				}
				sb2.AppendLine("""
								_ => value.ToString(),
						};
					""");
			}
			sb1.AppendLine($$"""
						}
					{{sb2.ToString()}}
					}
					""");
			spc.AddSource($"EnumConverters.{registryInfo}.g.cs", sb1.ToString());
		});
	}
	private static void GetMetadata(IncrementalGeneratorInitializationContext context)
	{
		var metadata = context.CompilationProvider.Combine(context.SyntaxProvider.ForAttributeWithMetadataName(
				fullyQualifiedMetadataName: JsonConverterSourceTypeAttrName,
				predicate: (s, e) =>
				{
					return
							(s is TypeDeclarationSyntax typeDeclaration &&
							(typeDeclaration is InterfaceDeclarationSyntax) &&
							typeDeclaration.AttributeLists.Count > 0);
				},
				transform: (ctx, e) =>
				{
					if (ctx.TargetNode is not TypeDeclarationSyntax typeDeclaration)
						return default;
					INamedTypeSymbol? symbol = ctx.SemanticModel.GetDeclaredSymbol(typeDeclaration);
					if (symbol is null)
						return default;
					AttributeData? attrData = symbol.GetAttributes().FirstOrDefault(a => IsAttribute(a, JsonConverterSourceTypeAttrName));
					return symbol;
				}
				).Collect());
		context.RegisterSourceOutput(metadata, (ctx, pair) =>
		{
			(Compilation compilation, ImmutableArray<INamedTypeSymbol?> symbols) = pair;
			foreach (var symbol in symbols)
			{
				INamedTypeSymbol? baseType = symbols.FirstOrDefault(i => i is not null);
				if (baseType is null)
					return;
				IEnumerable<INamedTypeSymbol> derivedTypes = GetDerivedTypes(baseType, compilation);
				StringBuilder sb = new();
				sb.AppendLine($"// <auto-generated/>\n#nullable enable\n\nnamespace {baseType.ContainingNamespace};\n");
				foreach (var type in derivedTypes.OrderBy(i => i.ToDisplayString()))
				{
					sb.AppendLine($"/* {type.ToDisplayString()} */");
				}
				ctx.AddSource("Metadata.g.cs", sb.ToString());
				break;
			}
		});
	}
	private static IEnumerable<INamedTypeSymbol> GetDerivedTypes(INamedTypeSymbol baseType, Compilation compilation, bool includeReferences = false)
	{
		foreach (var t in GetAllTypes(compilation.GlobalNamespace))
			if (IsDerivedFrom(t, baseType))
				yield return t;
		if (!includeReferences) yield break;
		foreach (var asm in compilation.SourceModule.ReferencedAssemblySymbols)
			foreach (var t in GetAllTypes(asm.GlobalNamespace))
				if (IsDerivedFrom(t, baseType))
					yield return t;
	}
	private static bool IsDerivedFrom(INamedTypeSymbol? candidate, INamedTypeSymbol? baseType)
	{
		if (SymbolEqualityComparer.Default.Equals(candidate, baseType))
			return true;
		if (baseType?.TypeKind == TypeKind.Interface)
			return candidate?.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, baseType)) ?? false;
		for (var current = candidate?.BaseType;
				current != null && current.SpecialType != SpecialType.System_Object;
				current = current.BaseType)
		{
			if (SymbolEqualityComparer.Default.Equals(current, baseType))
				return true;
		}
		return false;
	}
	private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol namespaceSymbol)
	{
		foreach (var member in namespaceSymbol.GetMembers())
		{
			if (member is INamespaceSymbol nestedNS)
				foreach (var type in GetAllTypes(nestedNS))
					yield return type;
			else if (member is INamedTypeSymbol namedType)
			{
				yield return namedType;
				foreach (var nested in namedType.GetTypeMembers())
					yield return nested;
			}
		}
	}

	private void GenerateConverterHub(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<(string? Left, (ITypeSymbol, INamedTypeSymbol)[] Right)> incrementalValueProvider)
	{
		context.RegisterSourceOutput(incrementalValueProvider, (ctx, data) =>
		{
			var (left, right) = data;
			if (string.IsNullOrEmpty(left))
				return;
			StringBuilder sb = new();
			sb.AppendLine($$"""
						// <auto-generated/>

						using System.Text.Json;
						using System.Text.Json.Serialization;
						using RhythmBase.Global.Converters;

						namespace RhythmBase.{{left}}.Converters;

						internal static partial class ConverterHub
						{
							private static class ConverterCache<T>
							{
								public static JsonConverter? Converter;
							}
							static ConverterHub()
							{
						""");
			foreach (var pair in right)
			{
				sb.AppendLine($$"""
								ConverterCache<{{pair.Item1.ToDisplayString()}}>.Converter = new {{pair.Item2.ToDisplayString()}}();
						""");
			}
			sb.AppendLine("""
							}
							public static T Read<T>(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
							{
								if (ConverterCache<T>.Converter is MetadataJsonConverter<T> c1)
									return c1.Read(ref reader, typeof(T), options) ?? default!;
								if (ConverterCache<T>.Converter is JsonConverter<T> c2)
									return c2.Read(ref reader, typeof(T), options.JsonSerializerOptions) ?? default!;
								else
						#if DEBUG
									throw new NotImplementedException($"No converter found for type {typeof(T)}.");
						#else
									return default!;
						#endif
							}
							public static void Write<T>(Utf8JsonWriter writer, T value, MetadataJsonSerializerOptions options)
							{
								if (ConverterCache<T>.Converter is MetadataJsonConverter<T> c1)
									c1.Write(writer, value, options);
								else if (ConverterCache<T>.Converter is JsonConverter<T> c2)
									c2.Write(writer, value, options.JsonSerializerOptions);
								else
						#if DEBUG
									throw new NotImplementedException($"No converter found for type {typeof(T)}.");
						#else
									writer.WriteNullValue();
						#endif
							}
						}
						""");
			ctx.AddSource($"ConverterHub.{left}.g.cs", sb.ToString());
		});
	}
	private static ITypeSymbol UnwarpNullable(ITypeSymbol type)
	{
		if (type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
			&& type is INamedTypeSymbol namedType
			&& namedType.TypeArguments.Length == 1)
			return namedType.TypeArguments[0];
		return type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
	}
	private void GenerateReadBody(Compilation compilation, StringBuilder sb, string id, ClassGenCvtrInfo info, (ITypeSymbol, INamedTypeSymbol)[] classGenMap)
	{
		//sb.AppendLine("/*\n");


		sb.AppendLine("""
								if (base.Read(ref reader, propertyName, ref value, options))
									return true;
						""");
		int index = 0;
		foreach (var prop in info.Properties.OrderBy(i => i.Property.Name))
		{
			var attrs = prop.Property.GetAttributes();
			var aliasAttrSmp = compilation.GetTypeByMetadataName(JsonAliasAttrName);
			var condAttrSmp = compilation.GetTypeByMetadataName(JsonConditionAttrName);
			var ignoreAttrSmp = compilation.GetTypeByMetadataName(JsonIgnoreAttrName);
			var notIgnoreAttrSmp = compilation.GetTypeByMetadataName(JsonNotIgnoreAttrName);
			var jsonCvtrAttrSmp = compilation.GetTypeByMetadataName(JsonConverterAttrName);
			var timeCvtrAttrSmp = compilation.GetTypeByMetadataName(JsonTimeAttrName);
			var dftCvtrAttrSmp = compilation.GetTypeByMetadataName(JsonDefaultSerializerAttrName);

			bool shouldGenerate = prop.Property.SetMethod is not null &&
				prop.Property.ExplicitInterfaceImplementations.Length == 0 &&
				(
				(
				prop.Property.DeclaredAccessibility.HasFlag(Accessibility.Public) ||
				attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, notIgnoreAttrSmp)) is not null ||
				attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, aliasAttrSmp)) is not null
				) &&
				attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, ignoreAttrSmp)) is null
				);
			if (!shouldGenerate) continue;

			bool isNullable = prop.Property.Type.NullableAnnotation == NullableAnnotation.Annotated;

			string alias = attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, aliasAttrSmp) && i.ConstructorArguments.Length == 1)?.ConstructorArguments[0].Value as string
					?? ToLowerCamelCase(prop.Property.ToDisplayParts().Last().ToString());
			string? condition = attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, condAttrSmp) && i.ConstructorArguments.Length == 1)?.ConstructorArguments[0].Value as string;
			INamedTypeSymbol? jsonCvtr = attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, jsonCvtrAttrSmp) && i.ConstructorArguments.Length == 1)?.ConstructorArguments[0].Value as INamedTypeSymbol;
			int? timeEnum = attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, timeCvtrAttrSmp) && i.ConstructorArguments.Length == 1)?.ConstructorArguments[0].Value as int?;

			string valueAccess = $"value.{prop.Property.ToDisplayParts().Last()}";
			sb.AppendLine($$"""{{(index == 0 ? "" : "else ")}}if (propertyName.SequenceEqual("{{alias}}"u8))""");

			var type = UnwarpNullable(prop.Property.Type);

			// 已注册转换器
			if(jsonCvtr is null && classGenMap.FirstOrDefault(i=>i.Item1.Equals(type, SymbolEqualityComparer.Default)) is var (classGen, cvtr) && classGen is not null)
			{
				sb.Append($$"""{{valueAccess}} = ConverterHub.Read<{{type.ToDisplayString()}}>(ref reader, options);""");
			}
			// 自定义转换器
			else if (jsonCvtr is not null)
			{
				if (SymbolEqualityComparer.Default.Equals(jsonCvtr.BaseType.OriginalDefinition, compilation.GetTypeByMetadataName(MetadataJsonConverterTypeName)))
					sb.Append($$"""{{valueAccess}} = new {{jsonCvtr.ToDisplayString()}}().Read(ref reader, typeof({{type.ToDisplayString()}}), options);""");
				else
					sb.Append($$"""{{valueAccess}} = new {{jsonCvtr.ToDisplayString()}}().Read(ref reader, typeof({{type.ToDisplayString()}}), options.JsonSerializerOptions);""");
			}
			// 时间
			else if (timeEnum is int timeType)
			{
				switch (timeType)
				{
					case 0:
						sb.Append($"{valueAccess} = TimeSpan.FromMilliseconds(reader.GetDouble());");
						break;
					case 1:
						sb.Append($"{valueAccess} = TimeSpan.FromSeconds(reader.GetDouble());");
						break;
					default:
						sb.Append($$"""{ throw new NotImplementedException(); /* Unsupported time converter type: {{timeType}} */ }""");
						break;
				}
			}
			// 枚举
			else if (type.TypeKind == TypeKind.Enum)
			{
				if (attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, dftCvtrAttrSmp)) is not INamedTypeSymbol dft)
					sb.Append($$"""{ if (global::RhythmBase.Global.Converters.EnumConverter.TryParse(reader.GetString(), out {{type.ToDisplayString()}} enumValue{{index}})) {{valueAccess}} = enumValue{{index}}; }""");
				else
					// - 非枚举
					sb.Append($$"""{{valueAccess}} = ({{type.ToDisplayString()}})reader.GetInt64();""");
			}
			// 默认类型
			else if (type.SpecialType is not SpecialType.None)
				sb.Append($$"""{{valueAccess}} = reader.Get{{GetJsonReadMethod(type)}}();""");
			// 其他
			else if (IsConcreteEnumerable(type) is ITypeSymbol elementType)
			{
				string listType = $"System.Collections.Generic.List<{elementType.ToDisplayString()}>";
				sb.AppendLine($$"""
						{
							var list = new {{listType}}();
							if (reader.TokenType == JsonTokenType.StartArray)
							{
								while (reader.Read())
								{
									if (reader.TokenType == JsonTokenType.EndArray)
										break;
				""");
				if (elementType.SpecialType is not SpecialType.None)
					sb.Append($$"""  var elementValue = reader.Get{{GetJsonReadMethod(elementType)}}();""");
				else if (elementType.TypeKind == TypeKind.Enum)
					sb.Append($$"""  global::RhythmBase.Global.Converters.EnumConverter.TryParse(reader.GetString(), out {{elementType.ToDisplayString()}} elementValue);""");
				else
					sb.Append($$"""  var elementValue = ConverterHub.Read<{{elementType.ToDisplayString()}}>(ref reader, options);""");
				sb.AppendLine($$"""
									list.Add(elementValue);
								}
							}
							{{valueAccess}} = [..list];
						}
				""");
			}
			else
				sb.Append($$"""{{valueAccess}} = ConverterHub.Read<{{type.ToDisplayString()}}>(ref reader, options);""");
			sb.AppendLine();
			index++;
		}
		sb.AppendLine("""
								else return false;
								return true;
						""");

		//sb.AppendLine("*/throw new NotImplementedException();");
	}
	private void GenerateWriteBody(Compilation compilation, StringBuilder sb, string id, ClassGenCvtrInfo info, (ITypeSymbol, INamedTypeSymbol)[] classGenMap)
	{
		//sb.AppendLine("/*\n");

		sb.AppendLine("base.Write(writer, ref value, options);");

		int index = 0;
		var metadataJsonConverterType = compilation.GetTypeByMetadataName(MetadataJsonConverterTypeName);
		foreach (var prop in info.Properties.OrderBy(i => i.Property.Name))
		{
			var attrs = prop.Property.GetAttributes();
			var aliasAttrSmp = compilation.GetTypeByMetadataName(JsonAliasAttrName);
			var condAttrSmp = compilation.GetTypeByMetadataName(JsonConditionAttrName);
			var ignoreAttrSmp = compilation.GetTypeByMetadataName(JsonIgnoreAttrName);
			var notIgnoreAttrSmp = compilation.GetTypeByMetadataName(JsonNotIgnoreAttrName);
			var jsonCvtrAttrSmp = compilation.GetTypeByMetadataName(JsonConverterAttrName);
			var timeCvtrAttrSmp = compilation.GetTypeByMetadataName(JsonTimeAttrName);
			var dftCvtrAttrSmp = compilation.GetTypeByMetadataName(JsonDefaultSerializerAttrName);

			bool shouldGenerate = prop.Property.GetMethod is not null &&
				prop.Property.ExplicitInterfaceImplementations.Length == 0 &&
				(
				(
				prop.Property.DeclaredAccessibility.HasFlag(Accessibility.Public) ||
				attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, notIgnoreAttrSmp)) is not null ||
				attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, aliasAttrSmp)) is not null
				) &&
				attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, ignoreAttrSmp)) is null
				);
			if (!shouldGenerate) continue;

			bool isNullable = prop.Property.Type.NullableAnnotation == NullableAnnotation.Annotated;

			string alias = attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, aliasAttrSmp) && i.ConstructorArguments.Length == 1)?.ConstructorArguments[0].Value as string
					?? ToLowerCamelCase(prop.Property.ToDisplayParts().Last().ToString());
			string? condition = attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, condAttrSmp) && i.ConstructorArguments.Length == 1)?.ConstructorArguments[0].Value as string;
			INamedTypeSymbol? jsonCvtr = attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, jsonCvtrAttrSmp) && i.ConstructorArguments.Length == 1)?.ConstructorArguments[0].Value as INamedTypeSymbol;
			int? timeEnum = attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, timeCvtrAttrSmp) && i.ConstructorArguments.Length == 1)?.ConstructorArguments[0].Value as int?;

			var type = UnwarpNullable(prop.Property.Type);

			string valueAccess = $"value.{prop.Property.ToDisplayParts().Last().ToString()}";
			if (isNullable)
			{
				valueAccess = $"localValue{index}";
				sb.Append($$"""
						if (value.{{prop.Property.ToDisplayParts().Last().ToString()}} is {{type.ToDisplayString()}} localValue{{index}})
						{ 
				""");
				;
				index++;
			}

			bool hasCondition = false;
			if (!string.IsNullOrEmpty(condition))
			{
				var conditionLines = ReplaceVariableWithName(condition).Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
				sb.AppendLine($"		if ({string.Join("\n\t\t\t", conditionLines)})");
				hasCondition = true;
			}

			// 已注册转换器
			if(jsonCvtr is null && classGenMap.FirstOrDefault(i=>i.Item1.Equals(type, SymbolEqualityComparer.Default)) is var (classGen, cvtr) && classGen is not null )
			{
				sb.Append($$"""{{(hasCondition ? "{" : "")}} writer.WritePropertyName("{{alias}}"u8); ConverterHub.Write(writer, {{valueAccess}}, options); {{(hasCondition ? "}" : "")}}""");
			}
			// 自定义转换器
			else if (jsonCvtr is not null)
			{
				if (SymbolEqualityComparer.Default.Equals(jsonCvtr.BaseType.OriginalDefinition, metadataJsonConverterType))
					sb.Append($$"""{ writer.WritePropertyName("{{alias}}"u8); new {{jsonCvtr.ToDisplayString()}}().Write(writer, {{valueAccess}}, options); }""");
				else
					sb.Append($$"""{ writer.WritePropertyName("{{alias}}"u8); new {{jsonCvtr.ToDisplayString()}}().Write(writer, {{valueAccess}}, options.JsonSerializerOptions); }""");
			}
			// 时间
			else if (timeEnum is int timeType)
			{
				switch (timeType)
				{
					case 0:
						sb.Append($"writer.WriteNumber(\"{alias}\"u8, {valueAccess}.TotalMilliseconds);");
						break;
					case 1:
						sb.Append($"writer.WriteNumber(\"{alias}\"u8, {valueAccess}.TotalSeconds);");
						break;
					default:
						sb.Append($$"""{ throw new NotImplementedException(); /* Unsupported time converter type: {{timeType}} */ }""");
						break;
				}
			}
			// 枚举
			else if (type.TypeKind == TypeKind.Enum)
			{
				if (attrs.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, dftCvtrAttrSmp)) is not INamedTypeSymbol dft)
					sb.Append($"writer.WriteString(\"{alias}\"u8, {valueAccess}.ToEnumString());");
				else
					// - 非枚举
					sb.Append($"writer.WriteNumber(\"{alias}\"u8, System.Convert.ToInt64({valueAccess}));");
			}
			// 默认类型
			else if (type.SpecialType is not SpecialType.None)
				sb.Append($$"""{ writer.Write{{GetJsonWriterMethod(type)}}("{{alias}}"u8, {{valueAccess}}); }""");
			// 其他
			else if (IsConcreteEnumerable(type) is ITypeSymbol elementType)
			{
				string itemVar = $"item{index}";
				sb.AppendLine($$"""
				{ writer.WritePropertyName("{{alias}}"u8);
					writer.WriteStartArray();
					foreach (var {{itemVar}} in {{valueAccess}})
					{
				""");
				// 根据 elementType 写入每个元素
				if (elementType.SpecialType is not SpecialType.None)
					sb.Append($$"""  writer.Write{{GetJsonWriterMethod(elementType)}}Value({{itemVar}});""");
				else if (elementType.TypeKind == TypeKind.Enum)
					sb.Append($$"""  writer.WriteStringValue({{itemVar}}.ToEnumString());""");
				else
					sb.Append($$"""  ConverterHub.Write(writer, {{itemVar}}, options);""");
				sb.AppendLine($$"""
					}
					writer.WriteEndArray(); }
				""");
			}
			else
				sb.Append($$"""{ writer.WritePropertyName("{{alias}}"u8); ConverterHub.Write(writer, {{valueAccess}}, options); }""");
			if (isNullable)
				sb.AppendLine(" }");
			else
				sb.AppendLine();
		}
		//sb.AppendLine("*/throw new NotImplementedException();");
	}

	private string GetJsonReadMethod(ITypeSymbol type)
	{
		return type.SpecialType switch
		{
			SpecialType.System_Boolean => "Boolean",
			SpecialType.System_Byte => "Byte",
			SpecialType.System_Char => "String",
			SpecialType.System_Decimal => "Decimal",
			SpecialType.System_Double => "Double",
			SpecialType.System_Single => "Single",
			SpecialType.System_Int16 => "Int16",
			SpecialType.System_Int32 => "Int32",
			SpecialType.System_Int64 => "Int64",
			SpecialType.System_SByte => "SByte",
			SpecialType.System_UInt16 => "UInt16",
			SpecialType.System_UInt32 => "UInt32",
			SpecialType.System_UInt64 => "UInt64",
			SpecialType.System_String => "String",
			_ => throw new NotSupportedException($"Unsupported property type: {type.ToDisplayString()}")
		};
	}
	private string GetJsonWriterMethod(ITypeSymbol type)
	{
		return type.SpecialType switch
		{
			SpecialType.System_Boolean => "Boolean",
			SpecialType.System_Byte => "Number",
			SpecialType.System_Char => "String",
			SpecialType.System_Decimal or
			SpecialType.System_Double or
			SpecialType.System_Single or
			SpecialType.System_Byte or
			SpecialType.System_Int16 or
			SpecialType.System_Int32 or
			SpecialType.System_Int64 or
			SpecialType.System_SByte or
			SpecialType.System_UInt16 or
			SpecialType.System_UInt32 or
			SpecialType.System_UInt64 => "Number",
			SpecialType.System_String => "String",
			_ => throw new NotSupportedException($"Unsupported property type: {type.ToDisplayString()}")
		};
	}

	private void GenerateConverter(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<((string, Compilation), ((IClassGen[], ClassGenAttr)[], (ITypeSymbol, INamedTypeSymbol)[]))> jsonClassCvtrMapGenInfo)
	{
		context.RegisterSourceOutput(jsonClassCvtrMapGenInfo, (ctx, classGenInfoArray) =>
		{
			var (idAndCompilation, classGensAndClassGenMap) = classGenInfoArray;
			var (id, compilation) = idAndCompilation;
			var (classGens, classGenMap) = classGensAndClassGenMap;
			if (string.IsNullOrEmpty(id))
				return;
			string ns = $"RhythmBase.{id}.Converters";
			StringBuilder classCvtrSb = new();
			StringBuilder classMapSb = new();
			classCvtrSb.AppendLine($"""
						// <auto-generated/>
						#nullable enable
						
						using System;
						using RhythmBase.Global.Extensions;
						using System.Text.Json;
						using static RhythmBase.Global.Converters.EnumConverter;
						
						namespace {ns};
						""");

			classMapSb.AppendLine($$"""
						internal static class ConverterMap {
						""");

			foreach (var classGen in classGens)
			{
				var classes = classGen.Item1;
				classMapSb.AppendLine($$"""
							internal static RhythmBase.Global.Converters.InstanceConverter<{{classGen.Item2.RootType.ToDisplayString()}}> GetConverter({{classGen.Item2.EnumType.ToDisplayString()}} type) => type switch
							{
						""");
				foreach (var c in classes.OrderBy(i => i.ICGClassType.Name))
				{
					var t = c.ICGClassType;
					var baseType = t.BaseType;
					var baseRecord = classes.FirstOrDefault(d => baseType.Equals(d.ICGClassType, SymbolEqualityComparer.Default));
					if (baseRecord is null && (baseRecord is not null || !baseType.Equals(classGen.Item2.RootType, SymbolEqualityComparer.Default))) continue;
					if (c is ClassGenCvtrInfo c1)
					{
						string baseConverterName;
						ISymbol enumValueSymbol = c1.ClassTypeEnum;
						INamedTypeSymbol ocvtr = classGen.Item2.RootConverterType.OriginalDefinition.Construct(t);
						if (baseRecord is ClassGenCvtrInfo baseRecord1)
						{
							baseConverterName = $"{ns}.{baseRecord1.ClassType.Name}Converter";
						}
						else if (baseRecord is ClassRefConverterInfo baseRecord2)
						{
							var baseConverter = baseRecord2.ConverterType;
							if (baseConverter.IsGenericType)
								baseConverterName = baseConverter.OriginalDefinition.Construct(t).ToDisplayString();
							else
							{
								ctx.ReportDiagnostic(Diagnostic.Create(NonGenericBaseConverterRule, c1.ClassType.Locations.FirstOrDefault(), baseConverter.ToDisplayString(), "base converter must be generic if the base type is not directly the root type"));
								continue;
							}
						}
						else
						{
							baseConverterName = ocvtr.ToDisplayString();
						}
						classCvtrSb.AppendLine($$"""
						internal class {{t.Name}}Converter : {{baseConverterName}}
						{
							protected override bool Read(ref Utf8JsonReader reader, ReadOnlySpan<byte> propertyName, ref {{t.ToDisplayString()}} value, global::RhythmBase.Global.Converters.MetadataJsonSerializerOptions options)
							{
						""");
						GenerateReadBody(compilation, classCvtrSb, id, c1, classGenMap);
						classCvtrSb.AppendLine($$"""
							}
							protected override void Write(Utf8JsonWriter writer, ref {{t.ToDisplayString()}} value, global::RhythmBase.Global.Converters.MetadataJsonSerializerOptions options)
							{
						""");
						GenerateWriteBody(compilation, classCvtrSb, id, c1, classGenMap);
						classCvtrSb.AppendLine("""
							}
						}
						""");
					}
					switch (c)
					{
						case ClassGenCvtrInfo c2:
							classMapSb.AppendLine($$"""
								{{c2.ClassTypeEnum.ToDisplayString()}} => new {{t.Name}}Converter(),
						""");
							break;
						case ClassRefConverterInfo c3:
							if (!c3.ClassType.IsAbstract && c3.ClassTypeEnum is not null)
								classMapSb.AppendLine($$"""
								{{c3.ClassTypeEnum.ToDisplayString()}} => new {{c3.ConverterType.ToDisplayString()}}(),
						""");
							break;
					}
				}
				classMapSb.AppendLine($$"""
								_ => throw new NotImplementedException(),
							};
						""");
			}
			classMapSb.AppendLine("""
						}
						""");
			classCvtrSb.Append(classMapSb);

			ctx.AddSource($"Converters.{id}.g.cs", classCvtrSb.ToString());
		});
	}
	private static void GenerateConverter(IncrementalGeneratorInitializationContext context, GenerationConfig config)
	{
		var classes = context.SyntaxProvider.CreateSyntaxProvider(
				predicate: (s, e) =>
				{
					return
							(s is TypeDeclarationSyntax typeDeclaration &&
							(typeDeclaration is ClassDeclarationSyntax or RecordDeclarationSyntax or InterfaceDeclarationSyntax or StructDeclarationSyntax) &&
							typeDeclaration.BaseList is not null);
				},
				transform: (ctx, e) =>
				{
					if (ctx.Node is not TypeDeclarationSyntax typeDeclaration)
						return (ClassInfo?)null;
					//InterfaceDeclarationSyntax? interfaceDeclaration = ctx.Node as InterfaceDeclarationSyntax;
					INamedTypeSymbol classDeclarationSymbol = ctx.SemanticModel.GetDeclaredSymbol(typeDeclaration) ?? throw new NotImplementedException();
					var interfaceToImplement = ctx.SemanticModel.Compilation.GetTypeByMetadataName(config.BaseInterfaceFullName);
					bool isTargetEvent =
									SymbolEqualityComparer.Default.Equals(classDeclarationSymbol, interfaceToImplement) ||
									classDeclarationSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interfaceToImplement));
					PropertyDeclarationSyntax[] propertyDeclarations = [.. typeDeclaration.ChildNodes().OfType<PropertyDeclarationSyntax>()];
					if (!isTargetEvent)
						return null;
					PropertyInfo[] props = [.. propertyDeclarations
										.Where(i =>
										{
												return IsSerializableEventProperty(i, ctx.SemanticModel);
										})
										.Select(i => BuildPropertyInfo(i, ctx.SemanticModel))];
					//bool serializerIgnore = HasAttribute(classDeclarationSymbol, JsonObjectNotSerializableAttrName);
					bool needSerializer = HasAttribute(classDeclarationSymbol, JsonObjectSerializableAttrName);
					bool hasSerializer = HasAttribute(classDeclarationSymbol, JsonObjectHasSerializerAttrName);
					var baseTypeSymbol = classDeclarationSymbol.BaseType;
					ClassInfo classInfo = new()
					{
						Type = classDeclarationSymbol,
						Name = classDeclarationSymbol.ToDisplayString(),
						BaseTypeName = typeDeclaration is StructDeclarationSyntax ? "Base" : classDeclarationSymbol.BaseType?.ToDisplayString() ?? "object",
						//SerializerIgnore = serializerIgnore,
						NeedSerializer = needSerializer,
						HasSerializer = hasSerializer,
						Properties = props,
					};
					return classInfo;
				}
				).Collect();

		context.RegisterSourceOutput(classes, (ctx, classSymbols) =>
		{
			try
			{
				StringBuilder sb = new();
				StringBuilder sb2 = new();
				sb.AppendLine($"""
// <auto-generated/>
#nullable enable

using System;
using RhythmBase.Global.Extensions;
using System.Text.Json;
using static RhythmBase.Global.Converters.EnumConverter;

namespace {config.TargetConverterNamespace};
""");
				classSymbols = classSymbols.Where(i => i is not null).ToImmutableArray();
				foreach (ClassInfo? classInfo in classSymbols.OrderBy(i => i?.Name))
				{
					if (classInfo is not ClassInfo ci) continue;
					if (!ci.NeedSerializer) continue;
					if (ci.Type.IsAbstract) continue;
					sb.AppendLine($$"""
internal class {{config.BaseConverterClassName}}{{ToShorter(ci.Name)}} : {{config.BaseConverterClassName}}{{ToShorter(ci.BaseTypeName)}}<{{ci.Name}}>
{
	protected override bool Read(ref Utf8JsonReader reader, ReadOnlySpan<byte> propertyName, ref {{ci.Name}} value, MetadataJsonSerializerOptions options)
	{
""");
					ci.Properties = ci.Properties.OrderBy(i => i.Symbol.Name).ToArray();
					if (ci.Properties.Length > 0)
					{
						if (ci.Properties.Count(i => i.Symbol.SetMethod is not null) == 0)
						{
							sb.AppendLine("        return base.Read(ref reader, propertyName, ref value, options);");
						}
						else
						{
							sb.AppendLine($$"""
		if(base.Read(ref reader, propertyName, ref value, options))
			return true;
""");
							bool isFirst = true;
							int enumIndex = 0;
							foreach (var pi in ci.Properties)
							{
								if (pi.Symbol.SetMethod is null) continue;
								// ReadOnly properties are skipped
								string propName = pi.Alias ?? ToLowerCamelCase(pi.Symbol.Name);
								bool newlineNeeded = false;
								// Custom converter
								if (!string.IsNullOrEmpty(pi.Converter))
								{
									if (pi.Symbol.Type.NullableAnnotation == NullableAnnotation.Annotated)
									{
										sb.AppendLine($$"""
		{{(isFirst ? "" : "else ")}}if (propertyName.SequenceEqual("{{propName}}"u8)){ if (reader.TokenType is not JsonTokenType.Null)
""");
										newlineNeeded = true;
									}
									else
										sb.AppendLine($"		{(isFirst ? "" : "else ")}if (propertyName.SequenceEqual(\"{propName}\"u8))");
									sb.AppendLine($"			value.{pi.Symbol.Name} = global::RhythmBase.Global.Converters.ConverterHub.Read<{WithoutNullable(pi.Symbol.Type).ToDisplayString()}>(ref reader, options);");
									if (newlineNeeded)
										sb.AppendLine("		}");
								}
								else
								{
									// Nullable
									var typeNotNull = pi.Symbol.Type;
									if (pi.Symbol.NullableAnnotation == NullableAnnotation.Annotated)
									{
										typeNotNull = WithoutNullable(pi.Symbol.Type);
										sb.AppendLine($$"""		{{(isFirst ? "" : "else ")}}if (propertyName.SequenceEqual("{{propName}}"u8)){ if(reader.TokenType is not JsonTokenType.Null)""");
										newlineNeeded = true;
									}
									else
										sb.AppendLine($"		{(isFirst ? "" : "else ")}if (propertyName.SequenceEqual(\"{propName}\"u8))");

									// Enum
									if (typeNotNull.TypeKind == TypeKind.Enum)
									{
										sb.AppendLine($$"""
			if(reader.TokenType is JsonTokenType.String && TryParse(reader.ValueSpan, out {{typeNotNull.ToDisplayString()}} enumValue{{enumIndex}}))
				value.{{pi.Symbol.Name}} = enumValue{{enumIndex}};
			else if(reader.TokenType is JsonTokenType.Number && reader.TryGetInt32(out int intValue{{enumIndex}}))
				value.{{pi.Symbol.Name}} = ({{typeNotNull.ToDisplayString()}})intValue{{enumIndex}};
			else
				value.{{pi.Symbol.Name}} = default;
""");
										enumIndex++;
									}
									else if (pi.TimeType is int t)
									{
										sb.AppendLine($"			value.{pi.Symbol.Name} = {t switch
										{
											0 => "TimeSpan.FromSeconds(reader.GetDouble())",
											1 => "TimeSpan.FromMilliseconds(reader.GetDouble())",
											_ => throw new NotImplementedException(),
										}};");
									}
									else
									{
										// Other types
										switch (typeNotNull.SpecialType)
										{
											case
																SpecialType.System_Byte or
																SpecialType.System_SByte or
																SpecialType.System_Char or
																SpecialType.System_Decimal or
																SpecialType.System_Double or
																SpecialType.System_Single or
																SpecialType.System_Int16 or
																SpecialType.System_Int32 or
																SpecialType.System_Int64 or
																SpecialType.System_UInt16 or
																SpecialType.System_UInt32 or
																SpecialType.System_UInt64:
												string typePostfix = typeNotNull.SpecialType.ToString().Replace("System_", "");
												sb.AppendLine($"			value.{pi.Symbol.Name} = reader.Get{typePostfix}();");
												break;
											case SpecialType.System_Boolean:
												sb.AppendLine($"""
			if (reader.TokenType is JsonTokenType.True or JsonTokenType.False)
				value.{pi.Symbol.Name} = reader.GetBoolean();
			else if (reader.TokenType is JsonTokenType.String)
				value.{pi.Symbol.Name} = "Enabled" == reader.GetString();
			else
				value.{pi.Symbol.Name} = false;
""");
												break;
											case SpecialType.System_String:
												sb.AppendLine($"			value.{pi.Symbol.Name} = reader.GetString() ?? \"\";");
												break;
											default:
												if (typeNotNull.TypeKind == TypeKind.Struct)
													sb.AppendLine($"			value.{pi.Symbol.Name} = global::RhythmBase.Global.Converters.ConverterHub.Read<{typeNotNull.ToDisplayString()}>(ref reader, options);");
												else if (IsConcreteEnumerable(pi.Symbol.Type) is ITypeSymbol typeSymbol)
													sb.AppendLine($"/*Tag:GenericList*/			value.{pi.Symbol.Name} = global::RhythmBase.Global.Converters.ConverterHub.Read<{typeNotNull.ToDisplayString()}>(ref reader, options) ?? [];");
												else
													sb.AppendLine($"			value.{pi.Symbol.Name} = global::RhythmBase.Global.Converters.ConverterHub.Read<{typeNotNull.ToDisplayString()}>(ref reader, options) ?? new();");
												break;
										}
									}
									if (newlineNeeded)
										sb.AppendLine("		}");
								}
								isFirst = false;
							}
							sb.AppendLine($$"""
		else return false;
		return true;
""");
						}
					}
					else
					{
						sb.AppendLine("		return base.Read(ref reader, propertyName, ref value, options);");
					}
					sb.AppendLine($$"""
	}
	protected override void Write(Utf8JsonWriter writer, ref {{ci.Name}} value, MetadataJsonSerializerOptions options)
	{
		base.Write(writer, ref value, options);
""");
					if (ci.Properties.Length > 0)
					{
						int tempNotNullVarCount = 0;
						foreach (var pi in ci.Properties)
						{
							if (pi.Symbol.GetMethod is null || (pi.Symbol.SetMethod is null && pi.Alias is null)) continue;
							string propName = pi.Alias ?? ToLowerCamelCase(pi.Symbol.Name);
							bool multiline = false;
							// Custom converter
							if (!string.IsNullOrEmpty(pi.Converter))
							{
								sb2.AppendLine($"		writer.WritePropertyName(\"{propName}\"u8);");
								if (pi.Symbol.NullableAnnotation == NullableAnnotation.Annotated)
								{
									sb2.AppendLine($"""
		if (value.{pi.Symbol.Name} is {WithoutNullable(pi.Symbol.Type).ToDisplayString()} valueNotNull{tempNotNullVarCount})
			global::RhythmBase.Global.Converters.ConverterHub.Write<{WithoutNullable(pi.Symbol.Type).ToDisplayString()}>(writer, valueNotNull{tempNotNullVarCount}, options);
		else
			writer.WriteNullValue();
""");
									tempNotNullVarCount++;
								}
								else
								{
									sb2.AppendLine($"		global::RhythmBase.Global.Converters.ConverterHub.Write<{WithoutNullable(pi.Symbol.Type).ToDisplayString()}>(writer, value.{pi.Symbol.Name}, options);");
								}
								multiline = true;
							}
							else
							{
								var typeNotNull = pi.Symbol.Type;
								bool isNullable = false;
								if (pi.Symbol.NullableAnnotation == NullableAnnotation.Annotated)
								{
									typeNotNull = WithoutNullable(pi.Symbol.Type);
									isNullable = true;
									sb2.Append($"""
		if (value.{pi.Symbol.Name} is {WithoutNullable(pi.Symbol.Type).ToDisplayString()} valueNotNull{tempNotNullVarCount})
""");
								}

								if (typeNotNull.TypeKind == TypeKind.Enum)
									sb2.AppendLine($"		writer.WriteString(\"{propName}\"u8, {(isNullable ? $"valueNotNull{tempNotNullVarCount}" : $"value.{pi.Symbol.Name}")}.ToEnumString());");
								else if (pi.TimeType is int t)
								{
									sb2.AppendLine($"		writer.WriteNumber(\"{propName}\"u8, {(t switch
									{
										0 => "value." + pi.Symbol.Name + ".TotalMilliseconds",
										1 => "value." + pi.Symbol.Name + ".TotalSeconds",
										_ => throw new NotImplementedException(),
									})});");
								}
								else
								{
									switch (typeNotNull.SpecialType)
									{
										case SpecialType.System_Boolean:
											sb2.AppendLine($"		writer.WriteBoolean(\"{propName}\"u8, value.{LastPartOf(pi.Symbol.Name)}{(isNullable ? ".Value" : "")});");
											break;
										case SpecialType.System_String:
											sb2.AppendLine($"		writer.WriteString(\"{propName}\"u8, value.{LastPartOf(pi.Symbol.Name)});");
											break;
										case SpecialType.System_Byte or
															SpecialType.System_Int16 or
															SpecialType.System_Int32 or
															SpecialType.System_Int64 or
															SpecialType.System_UInt16 or
															SpecialType.System_UInt32 or
															SpecialType.System_UInt64 or
															SpecialType.System_Single or
															SpecialType.System_Double or
															SpecialType.System_Decimal:
											sb2.AppendLine($"		writer.WriteNumber(\"{propName}\"u8, value.{LastPartOf(pi.Symbol.Name)}{(isNullable ? ".Value" : "")});");
											break;
										default:
											sb2.AppendLine($$"""		{ writer.WritePropertyName("{{propName}}"u8);	global::RhythmBase.Global.Converters.ConverterHub.Write(writer, {{(isNullable ? $"valueNotNull{tempNotNullVarCount}" : $"value.{pi.Symbol.Name}")}}, options); }""");
											break;
									}
								}
								if (isNullable)
								{
									sb2.AppendLine($"""
				else
						writer.WriteNull("{propName}"u8);
""");
									tempNotNullVarCount++;
								}
							}
							AppendWriteLinesWithCondition(sb, sb2, pi.Condition, multiline);
							sb2.Clear();
						}
					}
					else
					{

					}
					sb.AppendLine($$"""
	}
}
""");
				}
				ctx.AddSource($"EventInstanceConverters{config.Id}.g.cs", sb.ToString());
				//GenerateEventTypeUtils(ctx, classSymbols, config);
			}
			catch (Exception ex)
			{
				ctx.AddSource($"EventInstanceConverters_Error{config.Id}.g.cs", $$"""
				/*
				An error occurred during generation: {{ex}}
				{{string.Join("\n", marks)}}
				*/
				""");
			}
		});
	}
	private class ClassEnumMapGenerationInfo
	{
		public INamedTypeSymbol ClassType { get; set; }
		public INamedTypeSymbol ClassTypeEnum { get; set; }
		public IEnumerable<INamedTypeSymbol> Classes { get; set; }
		public Dictionary<INamedTypeSymbol, HashSet<ISymbol>> ClassEnumMap { get; set; }
		public Dictionary<INamedTypeSymbol, ISymbol> ClassEnumDoubleMap { get; internal set; }
		public INamedTypeSymbol FallbackClassType { get; internal set; }
		public ISymbol? FallbackClassTypeEnum { get; internal set; }
	}
	private static void GenerateEventTypeUtils(SourceProductionContext spc, (string, ClassEnumMapGenerationInfo[]) classSymbols)
	{
		(string configId, ClassEnumMapGenerationInfo[] infos) = classSymbols;
		for (int i = 0; i < infos.Length; i++)
		{
			ClassEnumMapGenerationInfo? info = infos[i];
			int maxLength = info.ClassEnumMap.Keys
					.Max(i => i.ToDisplayString().Length);

			StringBuilder sb = new();
			sb.AppendLine($$"""
// <auto-generated/>
#nullable enable
using System;
namespace RhythmBase.{{configId}}.Extensions;

/// <summary>  
/// Utility class for converting between event types and enumerations.  
/// </summary>
public static partial class ClassEnumMap
{
	private static System.Collections.ObjectModel.ReadOnlyCollection<Type>? _t;
	private static System.Collections.ObjectModel.ReadOnlyDictionary<System.Type, RhythmBase.Global.Components.ReadOnlyEnumCollection<{{info.ClassTypeEnum.ToDisplayString()}}>>? _t2e;
	private static System.Collections.ObjectModel.ReadOnlyDictionary<{{info.ClassTypeEnum.ToDisplayString()}}, System.Type>? _e2t;
	private static System.Collections.ObjectModel.ReadOnlyCollection<Type> _types => _t ??= new(new Type[]
	{
""");
			foreach (var classSymbol in info.Classes)
			{
				sb.AppendLine($"\t\ttypeof({classSymbol.ToDisplayString()}),");
			}
			sb.AppendLine($$"""
	});
	private static System.Collections.ObjectModel.ReadOnlyDictionary<System.Type, RhythmBase.Global.Components.ReadOnlyEnumCollection<{{info.ClassTypeEnum.ToDisplayString()}}>> _type2enums => _t2e ??= new(new Dictionary<System.Type, RhythmBase.Global.Components.ReadOnlyEnumCollection<{{info.ClassTypeEnum.ToDisplayString()}}>>()
	{
""");
			string indent = new(' ', maxLength + 13);
			foreach (var pair in info.ClassEnumMap)
			{
				if (pair.Value.Count == 0)
					continue;
				sb.AppendLine($$"""
			[typeof({{pair.Key.ToDisplayString()}})] ={{new string(' ', maxLength - pair.Key.ToDisplayString().Length)}} new RhythmBase.Global.Components.ReadOnlyEnumCollection<{{info.ClassTypeEnum.ToDisplayString()}}>(2, // {{pair.Value.Count}}
			{{indent}}{{string.Join($",\n\t\t{indent}", pair.Value.OrderBy(i => i.Name).Select(i => $"{i.ToDisplayString()}"))}}),
""");
			}
			sb.AppendLine($$"""
	});
	private static System.Collections.ObjectModel.ReadOnlyDictionary<{{info.ClassTypeEnum.ToDisplayString()}}, System.Type> _enum2type => _e2t ??= new(new Dictionary<{{info.ClassTypeEnum.ToDisplayString()}}, System.Type>()
	{
""");
			foreach (var pair in info.ClassEnumDoubleMap)
			{
				sb.AppendLine($"\t\t[{pair.Value.ToDisplayString()}] ={new string(' ', maxLength - pair.Key.ToDisplayString().Length)} typeof({pair.Key.ToDisplayString()}),");
			}
			sb.AppendLine($$"""
	});  
	/// <summary>  
	/// Converts a type to its corresponding <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumeration.  
	/// </summary>  
	/// <param name="type">The type to convert.</param>  
	/// <returns>The corresponding <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumeration.</returns>  
	/// <exception cref="IllegalEventTypeException">Thrown when no matching <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> is found or multiple matching <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> are found.</exception>  
	public static {{info.ClassTypeEnum.ToDisplayString()}} ToEnum(Type type)
	{
		{{info.ClassTypeEnum.ToDisplayString()}} v;
		if (_type2enums == null)
		{
			string name = type.Name;
			if (!Enum.TryParse(name, out {{info.ClassTypeEnum.ToDisplayString()}} result))
			{
				throw new IllegalEventTypeException(type, "Unable to find a matching EventType.");
			}
			v = result;
		}
		else
		{
			try
			{
				v = _type2enums[type].Single();
			}
			catch (Exception)
			{
				throw new IllegalEventTypeException(type, "Multiple matching EventTypes were found. Please check if the type is an abstract class type.", new ArgumentException("Multiple matching EventTypes were found. Please check if the type is an abstract class type.", nameof(type)));
			}
		}
		return v;
	}
	/// <summary>  
	/// Converts a generic event type to its corresponding <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumeration.  
	/// </summary>  
	/// <typeparam name="TEvent">The generic event type to convert.</typeparam>  
	/// <returns>The corresponding <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumeration.</returns>  
	public static {{info.ClassTypeEnum.ToDisplayString()}} ToEnum<TEvent>() where TEvent : {{info.ClassType.ToDisplayString()}}, new() => ToEnum(typeof(TEvent));
	/// <summary>  
	/// Converts a type to an array of corresponding <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumerations.  
	/// </summary>  
	/// <param name="type">The type to convert.</param>  
	/// <returns>An array of corresponding <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumerations.</returns>  
	/// <exception cref="IllegalEventTypeException">Thrown when an unexpected exception occurs.</exception>  
	public static ReadOnlyEnumCollection<{{info.ClassTypeEnum.ToDisplayString()}}> ToEnums(Type type)
	{
		return _type2enums.TryGetValue(type, out var value) ? value : throw new IllegalEventTypeException(type);
	}
	/// <summary>  
	/// Converts a generic event type to an array of corresponding <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumerations.  
	/// </summary>  
	/// <typeparam name="TEvent">The generic event type to convert.</typeparam>  
	/// <returns>An array of corresponding <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumerations.</returns>
	public static ReadOnlyEnumCollection<{{info.ClassTypeEnum.ToDisplayString()}}> ToEnums<TEvent>() where TEvent : {{info.ClassType.ToDisplayString()}} => ToEnums(typeof(TEvent));
	/// <summary>  
	/// Converts a string representation of an event type to its corresponding Type.  
	/// </summary>  
	/// <param name="type">The string representation of the event type.</param>  
	/// <returns>The corresponding Type.</returns>
	public static Type ToType(string type)
	{
		Type t;
		if (global::RhythmBase.Global.Converters.EnumConverter.TryParse(type, out {{info.ClassTypeEnum.ToDisplayString()}} result))
		{
			t = result.ToType();
		}
		else
		{
			t = typeof({{info.FallbackClassType.ToDisplayString()}});
		}
		return t;
	}
	/// <summary>  
	/// Converts an <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumeration to its corresponding Type.  
	/// </summary>  
	/// <param name="type">The <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumeration to convert.</param>  
	/// <returns>The corresponding Type.</returns>  
	/// <exception cref="IllegalEventTypeException">Thrown when the value does not exist in the <see cref="{{info.ClassTypeEnum.ToDisplayString()}}" /> enumeration.</exception>
	public static Type ToType(this {{info.ClassTypeEnum.ToDisplayString()}} type)
	{
		Type t;
		if (_enum2type == null)
		{
			return Type.GetType($"{{info.ClassType.ContainingNamespace.ToDisplayString()}}.{type}") ?? throw new RhythmBaseException(
					$"Illegal Type: {type}.");
		}
		else
		{
			try
			{
				t = _enum2type[type];
			}
			catch
			{
				throw new IllegalEventTypeException(type.ToString(), "This value does not exist in the {{info.ClassTypeEnum.ToDisplayString()}} enumeration.");
			}
		}
		return t;
	}
}
""");
			string filename =
					infos.Length == 1
					? $"ClassEnumMap.{configId}.g.cs"
					: $"ClassEnumMap.{configId}_{i}.g.cs";
			spc.AddSource(filename, sb.ToString());
		}

	}
	private record struct ADFilterInfo(INamedTypeSymbol Type, AttributeData? specialNameAttribute);
	private static void GenerateFilterTypeUtilsForEnum(IncrementalGeneratorInitializationContext context)
	{
		List<string> marks = [];
		IncrementalValueProvider<ImmutableArray<ADFilterInfo>> types = context.SyntaxProvider.CreateSyntaxProvider(
				predicate: static (s, ct) => s is StructDeclarationSyntax,
				transform: (ctx, ct) =>
				{
					if (ctx.Node is not StructDeclarationSyntax structDecl)
						return (ADFilterInfo?)null;
					INamedTypeSymbol? interfaceToImplement = ctx.SemanticModel.Compilation.GetTypeByMetadataName("RhythmBase.Adofai.Components.Filters.IFilter");
					INamedTypeSymbol? specialIdAttribute = ctx.SemanticModel.Compilation.GetTypeByMetadataName("RhythmBase.Global.Converters.RDJsonSpecialIDAttribute");
					INamedTypeSymbol? symbol = ctx.SemanticModel.GetDeclaredSymbol(structDecl);
					if (symbol == null || !symbol.Interfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interfaceToImplement)))
						return null;
					AttributeData? attribute =
									symbol.GetAttributes().FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, specialIdAttribute));
					return new ADFilterInfo(symbol, attribute);
				})
				.Where(i => i != null)
				.Select((i, _) => i!.Value)
				.Collect();
		context.RegisterImplementationSourceOutput(types, (spc, type) =>
		{
			StringBuilder sb = new();
			sb.AppendLine("""
			// <auto-generated/>
			namespace RhythmBase.Adofai.Utils;
			partial class FilterTypeUtils{
				private static System.Collections.ObjectModel.ReadOnlyDictionary<string, RhythmBase.Adofai.FilterType> _str2e;
				private static System.Collections.ObjectModel.ReadOnlyDictionary<RhythmBase.Adofai.FilterType, string> _e2str;
				internal static System.Collections.ObjectModel.ReadOnlyDictionary<string, RhythmBase.Adofai.FilterType> _string2enum = _str2e ??= new(new Dictionary<string, RhythmBase.Adofai.FilterType>()
				{
			""");
			foreach (var t in type.OrderBy(i => i.Type.Name))
			{
				string specialName = t.specialNameAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? t.Type.Name;
				sb.AppendLine($"""
							["{specialName}"] = RhythmBase.Adofai.FilterType.{t.Type.Name},
					""");
			}
			sb.AppendLine("""
				});
				internal static System.Collections.ObjectModel.ReadOnlyDictionary<RhythmBase.Adofai.FilterType, string> _enum2string = _e2str ??= new(new Dictionary<RhythmBase.Adofai.FilterType, string>()
				{
			""");
			foreach (var t in type.OrderBy(i => i.Type.Name))
			{
				string specialName = t.specialNameAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? t.Type.Name;
				sb.AppendLine($"""
							[RhythmBase.Adofai.FilterType.{t.Type.Name}] = "{specialName}",
					""");
			}
			sb.AppendLine("""
				});
			}
			""");
			spc.AddSource($"FilterTypeUtilsForEnum.g.cs", sb.ToString());
		});
	}

}