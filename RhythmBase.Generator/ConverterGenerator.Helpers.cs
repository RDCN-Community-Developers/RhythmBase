using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
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
                    return default(ConverterRegistryScanResult);

                if (!HasAttribute(symbol, JsonConverterForAttrName))
                    return default(ConverterRegistryScanResult);

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

            foreach (var group in registrations.GroupBy(i => i.TargetType, SymbolEqualityComparer.Default))
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
    private static void GenerateEnumConverter(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<EnumInfo>> enums = context.SyntaxProvider.CreateSyntaxProvider(
        predicate: (syntaxNode, e) =>
        {
            return syntaxNode is EnumDeclarationSyntax enumDeclaration &&
             HasAttribute(enumDeclaration.AttributeLists, JsonEnumAttrName);
        },
        transform: (ctx, e) =>
        {
            EnumDeclarationSyntax enumDeclaration = (EnumDeclarationSyntax)ctx.Node;
            EnumMemberDeclarationSyntax[] enumMemberDeclaration = [.. enumDeclaration.ChildNodes().OfType<EnumMemberDeclarationSyntax>()];
            EnumInfo info = new();
            INamedTypeSymbol? symbol = ctx.SemanticModel.GetDeclaredSymbol(enumDeclaration);
            if (symbol is null)
                return default;
            info.Symbol = new(symbol.Name, symbol.ToDisplayString());
            info.PascalCase = symbol.GetAttributes().Any(a => IsAttribute(a, JsonEnumAttrName) && a.ConstructorArguments.Length == 1 && a.ConstructorArguments[0].Value is bool b && b);
            info.Fields = [.. enumMemberDeclaration.Select(i =>
            {
                IFieldSymbol? symbol = ctx.SemanticModel.GetDeclaredSymbol(i);
                string? alias = symbol?.GetAttributes().FirstOrDefault(a => IsAttribute(a, JsonAliasAttrName) && a.ConstructorArguments.Length == 1)?.ConstructorArguments[0].Value as string;
                return new FieldName(i.Identifier.Text, symbol?.ToDisplayString() ?? "", alias);
            })];
            return info;
        }
        ).Collect();

        context.RegisterSourceOutput(enums, (spc, enumSymbols) =>
        {
            StringBuilder sb = new();
            sb.AppendLine("""
				
					// <auto-generated/>
					#nullable enable
					using System;
					using System.Runtime.CompilerServices;
				
					namespace RhythmBase.Global.Extensions
					{
						/// <summary>
						/// Provides extension methods for converting enums to and from string representations.
						/// </summary>
						public static class EnumConverter
						{
					""");

            foreach (EnumInfo e in enumSymbols.OrderBy(i => i.Symbol.FullName))
            {
                static string GetStringName(FieldName fieldName, bool isPascalCase)
                {
                    if (!string.IsNullOrEmpty(fieldName.Alias))
                        return fieldName.Alias;
                    if (isPascalCase)
                        return fieldName.Name;
                    return ToLowerCamelCase(fieldName.Name);
                }
                string fullName = e.Symbol.FullName;
                sb.AppendLine($"/* {fullName} */");

                // TryParse(string)
                sb.AppendLine($$"""
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
                foreach (FieldName field in e.Fields)
                {
                    sb.AppendLine($$"""
									case "{{GetStringName(field, e.PascalCase)}}":
										result = {{field.FullName}};
										return true;
					""");
                }
                sb.AppendLine("""
					
									default:
										result = default;
										return false;
								}
							}
					
					""");
                sb.AppendLine();

                // TryParse(ReadOnlySpan<byte>)
                sb.AppendLine($$"""
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
                foreach (FieldName field in e.Fields.OrderBy(i => i.FullName))
                {
                    sb.AppendLine($$"""
								{{(isFirst ? "" : "else ")}}if (value.SequenceEqual("{{GetStringName(field, e.PascalCase)}}"u8))
								{
									result = {{field.FullName}};
									return true;
								}
					""");
                    isFirst = false;
                }
                sb.AppendLine("""
								else
								{
									result = default;
									return false;
								}
							}

					""");

                // ToEnumString
                sb.AppendLine($$"""
							/// <summary>
							/// Converts the <see cref="{{fullName}}"/> enum value to its string representation.
							/// </summary>
							/// <param name="value">The enum value to convert.</param>
							/// <returns>The string representation of the enum value.</returns>
							[MethodImpl(MethodImplOptions.AggressiveInlining)]
							public static string ToEnumString(this {{fullName}} value) => value switch
							{
					""");
                foreach (FieldName field in e.Fields)
                {
                    sb.AppendLine($"""
								{field.FullName} => "{GetStringName(field, e.PascalCase)}",
					""");
                }
                sb.AppendLine("""
								_ => value.ToString(),
							};
					""");
            }
            sb.AppendLine("""
						}
					}
					""");

            spc.AddSource("EnumConverters.g.cs", sb.ToString());
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
using static RhythmBase.Global.Extensions.EnumConverter;

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
	protected override bool Read(ref Utf8JsonReader reader, ReadOnlySpan<byte> propertyName, ref {{ci.Name}} value, JsonSerializerOptions options)
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
                                                else if (IsConcreteEnumerable(pi.Symbol.Type))
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
	protected override void Write(Utf8JsonWriter writer, ref {{ci.Name}} value, JsonSerializerOptions options)
	{
		base.Write(writer, ref value, options);
""");
                    if (ci.Properties.Length > 0)
                    {
                        foreach (var pi in ci.Properties)
                        {
                            sb.AppendLine($"/* Property: {pi.Symbol.Name}|{pi.Alias} */");
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
		if (value.{pi.Symbol.Name} is {WithoutNullable(pi.Symbol.Type).ToDisplayString()} t)
			global::RhythmBase.Global.Converters.ConverterHub.Write<{WithoutNullable(pi.Symbol.Type).ToDisplayString()}>(writer, t, options);
		else
			writer.WriteNullValue();
""");
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
		if (value.{pi.Symbol.Name} is null)
			writer.WriteNull("{propName}"u8);
		else
	
""");
                                }

                                if (typeNotNull.TypeKind == TypeKind.Enum)
                                    sb2.AppendLine($"		writer.WriteString(\"{propName}\"u8, value.{pi.Symbol.Name}{(isNullable ? "?" : "")}.ToEnumString());");
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
                                            sb2.AppendLine($$"""		{ writer.WritePropertyName("{{propName}}"u8);	global::RhythmBase.Global.Converters.ConverterHub.Write(writer, value.{{pi.Symbol.Name}}, options); }""");
                                            break;
                                    }
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
                GenerateEventTypeUtils(ctx, classSymbols, config);
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
    private static void GenerateEventTypeUtils(SourceProductionContext spc, ImmutableArray<ClassInfo?> classSymbols, GenerationConfig config)
    {
        var validClassSymbols = classSymbols
            .Where(i =>
            {
                if (i is not ClassInfo ci) return false;
                if (ci.Type.IsAbstract) return false;
                if (ci.Type.IsGenericType) return false;
                if (!(ci.NeedSerializer || ci.HasSerializer)) return false;
                return true;
            })
            .Cast<ClassInfo>()
            .OrderBy(i => i.Name)
            .ToArray();
        int maxLength = validClassSymbols
            .Max(i => i.Name.Length);

        IOrderedEnumerable<ClassInfo> validClassSymbolsWithAbstract = classSymbols
            .Where(i =>
            {
                if (i is not ClassInfo ci) return false;
                if (ci.Type.IsGenericType) return false;
                return true;
            })
            .Cast<ClassInfo>()
            .OrderBy(i => i.Name);

        Dictionary<ISymbol, HashSet<ClassInfo>> typeLink = validClassSymbolsWithAbstract
             .Select(i => i.Type)
             .ToDictionary(
                 i => i as ISymbol,
                 i =>
                 {
                     HashSet<ClassInfo> result = [.. validClassSymbolsWithAbstract.Where(j => InheritsOrImplements(j.Type, i))];
                     return result;
                 },
                 SymbolEqualityComparer.Default);

        foreach (var pair in typeLink)
        {
            List<ClassInfo> abstracts = [];
            int abstractCount;
            do
            {
                abstractCount = abstracts.Count;
                foreach (ClassInfo abs in abstracts)
                {
                    if (typeLink.TryGetValue(abs.Type, out var result))
                        pair.Value.UnionWith(result);
                }
                abstracts = [.. pair.Value.Where(i => i.Type.IsAbstract)];
            }
            while (abstractCount < abstracts.Count);
            pair.Value.RemoveWhere(abstracts.Contains);
        }

        StringBuilder sb = new();
        sb.AppendLine($$"""
// <auto-generated/>
#nullable enable
using System;
namespace {{config.TargetUtilsNamespace}};

/// <summary>  
/// Utility class for converting between event types and enumerations.  
/// </summary>
public static partial class {{config.TargetUtilsClassName}}
{
	private static System.Collections.ObjectModel.ReadOnlyDictionary<{{config.ClassTypeEnumFullname}}, {{config.TargetConverterNamespace}}.{{config.BaseConverterClassName}}Base>? _c;
	private static System.Collections.ObjectModel.ReadOnlyCollection<Type>? _t;
	private static System.Collections.ObjectModel.ReadOnlyDictionary<System.Type, RhythmBase.Global.Components.ReadOnlyEnumCollection<{{config.ClassTypeEnumFullname}}>>? _t2e;
	private static System.Collections.ObjectModel.ReadOnlyDictionary<{{config.ClassTypeEnumFullname}}, System.Type>? _e2t;
	internal static System.Collections.ObjectModel.ReadOnlyDictionary<{{config.ClassTypeEnumFullname}}, {{config.TargetConverterNamespace}}.{{config.BaseConverterClassName}}Base> converters => _c ??= new(new Dictionary<{{config.ClassTypeEnumFullname}}, {{config.TargetConverterNamespace}}.{{config.BaseConverterClassName}}Base>()
	{
""");
        foreach (ClassInfo info in validClassSymbols)
        {
            sb.AppendLine($"\t\t[{config.ClassTypeEnumFullname}.{ToShorter(info.Name)}] ={new string(' ', maxLength - info.Name.Length)} new {config.TargetConverterNamespace}.{config.BaseConverterClassName}{ToShorter(info.Name)}(),");
        }
        sb.AppendLine($$"""
	});
	private static System.Collections.ObjectModel.ReadOnlyCollection<Type> _types => _t ??= new(new Type[]
	{
""");
        foreach (ClassInfo classInfo in validClassSymbolsWithAbstract)
        {
            sb.AppendLine($"\t\ttypeof({classInfo.Name}),");
        }
        sb.AppendLine($$"""
	});
	private static System.Collections.ObjectModel.ReadOnlyDictionary<System.Type, RhythmBase.Global.Components.ReadOnlyEnumCollection<{{config.ClassTypeEnumFullname}}>> _type2enums => _t2e ??= new(new Dictionary<System.Type, RhythmBase.Global.Components.ReadOnlyEnumCollection<{{config.ClassTypeEnumFullname}}>>()
	{
""");
        string indent = new(' ', maxLength + 13);
        foreach (ClassInfo classInfo in validClassSymbolsWithAbstract)
        {
            if (typeLink[classInfo.Type].Count == 0)
                continue;
            sb.AppendLine($$"""
			[typeof({{classInfo.Name}})] ={{new string(' ', maxLength - classInfo.Name.Length)}} new RhythmBase.Global.Components.ReadOnlyEnumCollection<{{config.ClassTypeEnumFullname}}>(2, // {{typeLink[classInfo.Type].Count}}
			{{indent}}{{string.Join($",\n\t\t{indent}", typeLink[classInfo.Type].OrderBy(i => i.Name).Select(i => $"{config.ClassTypeEnumFullname}.{ToShorter(i.Name)}"))}}),
""");
        }
        sb.AppendLine($$"""
	});
	private static System.Collections.ObjectModel.ReadOnlyDictionary<{{config.ClassTypeEnumFullname}}, System.Type> _enum2type => _e2t ??= new(new Dictionary<{{config.ClassTypeEnumFullname}}, System.Type>()
	{
""");
        foreach (ClassInfo info in validClassSymbols)
        {
            sb.AppendLine($"\t\t[{config.ClassTypeEnumFullname}.{ToShorter(info.Name)}] ={new string(' ', maxLength - info.Name.Length)} typeof({info.Name}),");
        }
        sb.AppendLine($$"""
	});  
	/// <summary>  
	/// Converts a type to its corresponding <see cref="{{config.ClassTypeEnumFullname}}" /> enumeration.  
	/// </summary>  
	/// <param name="type">The type to convert.</param>  
	/// <returns>The corresponding <see cref="{{config.ClassTypeEnumFullname}}" /> enumeration.</returns>  
	/// <exception cref="IllegalEventTypeException">Thrown when no matching <see cref="{{config.ClassTypeEnumFullname}}" /> is found or multiple matching <see cref="{{config.ClassTypeEnumFullname}}" /> are found.</exception>  
	public static {{config.ClassTypeEnumFullname}} ToEnum(Type type)
	{
		{{config.ClassTypeEnumFullname}} v;
		if (_type2enums == null)
		{
			string name = type.Name;
			if (!Enum.TryParse(name, out {{config.ClassTypeEnumFullname}} result))
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
	/// Converts a generic event type to its corresponding <see cref="{{config.ClassTypeEnumFullname}}" /> enumeration.  
	/// </summary>  
	/// <typeparam name="TEvent">The generic event type to convert.</typeparam>  
	/// <returns>The corresponding <see cref="{{config.ClassTypeEnumFullname}}" /> enumeration.</returns>  
	public static {{config.ClassTypeEnumFullname}} ToEnum<TEvent>() where TEvent : {{config.BaseInterfaceFullName}}, new() => ToEnum(typeof(TEvent));
	/// <summary>  
	/// Converts a type to an array of corresponding <see cref="{{config.ClassTypeEnumFullname}}" /> enumerations.  
	/// </summary>  
	/// <param name="type">The type to convert.</param>  
	/// <returns>An array of corresponding <see cref="{{config.ClassTypeEnumFullname}}" /> enumerations.</returns>  
	/// <exception cref="IllegalEventTypeException">Thrown when an unexpected exception occurs.</exception>  
	public static ReadOnlyEnumCollection<{{config.ClassTypeEnumFullname}}> ToEnums(Type type)
	{
		return _type2enums.TryGetValue(type, out var value) ? value : throw new IllegalEventTypeException(type);
	}
	/// <summary>  
	/// Converts a generic event type to an array of corresponding <see cref="{{config.ClassTypeEnumFullname}}" /> enumerations.  
	/// </summary>  
	/// <typeparam name="TEvent">The generic event type to convert.</typeparam>  
	/// <returns>An array of corresponding <see cref="{{config.ClassTypeEnumFullname}}" /> enumerations.</returns>
	public static ReadOnlyEnumCollection<{{config.ClassTypeEnumFullname}}> ToEnums<TEvent>() where TEvent : {{config.BaseInterfaceFullName}} => ToEnums(typeof(TEvent));
	/// <summary>  
	/// Converts a string representation of an event type to its corresponding Type.  
	/// </summary>  
	/// <param name="type">The string representation of the event type.</param>  
	/// <returns>The corresponding Type.</returns>
	public static Type ToType(string type)
	{
		Type ConvertToType;
		if (Enum.TryParse(type, out EventType result))
		{
			ConvertToType = result.ToType();
		}
		else
		{
			ConvertToType = {{config.ClassTypeEnumFullname}}.{{config.ClassTypeEnumUnknownMemberName}}.ToType();
		}
		return ConvertToType;
	}
	/// <summary>  
	/// Converts an <see cref="{{config.ClassTypeEnumFullname}}" /> enumeration to its corresponding Type.  
	/// </summary>  
	/// <param name="type">The <see cref="{{config.ClassTypeEnumFullname}}" /> enumeration to convert.</param>  
	/// <returns>The corresponding Type.</returns>  
	/// <exception cref="IllegalEventTypeException">Thrown when the value does not exist in the <see cref="{{config.ClassTypeEnumFullname}}" /> enumeration.</exception>
	public static Type ToType(this {{config.ClassTypeEnumFullname}} type)
	{
		Type ConvertToType;
		if (_enum2type == null)
		{
			return Type.GetType($"{{config.SourceNamespace}}.{type}") ?? throw new RhythmBaseException(
					$"Illegal Type: {type}.");
		}
		else
		{
			try
			{
				ConvertToType = _enum2type[type];
			}
			catch
			{
				throw new IllegalEventTypeException(type.ToString(), "This value does not exist in the {{config.ClassTypeEnumFullname}} enumeration.");
			}
		}
		return ConvertToType;
	}
}
""");

        spc.AddSource($"{config.TargetUtilsClassName}.{config.Id}.g.cs", sb.ToString());
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