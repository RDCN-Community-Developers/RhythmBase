using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

// 这坨写得太史了

namespace RhythmBase.Generator;

[Generator(LanguageNames.CSharp)]
public partial class ConverterGenerator : IIncrementalGenerator
{
	private static readonly DiagnosticDescriptor InvalidConverterRegistrationRule = new(
		"RDBG001",
		"Invalid converter registration",
		"Converter '{0}' has invalid RDJsonConverterFor registration: {1}",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	private static readonly DiagnosticDescriptor DuplicateConverterRegistrationRule = new(
		"RDBG002",
		"Duplicate converter registration",
		"Target type '{0}' is registered by multiple converters: {1}",
		"RhythmBase.Generator",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		GenerateConverterRegistry(context);
		GenerateEnumConverter(context);
		GenerationConfig[] configs = [
			new()
			{
				Id = "RD",
				SourceNamespace = "RhythmBase.RhythmDoctor.Events",
				TargetConverterNamespace = "RhythmBase.RhythmDoctor.Converters",
				TargetUtilsNamespace = "RhythmBase.RhythmDoctor.Utils",
				TargetUtilsClassName = "EventTypeUtils",
				BaseConverterClassName = "EventInstanceConverter",
				BaseInterfaceFullName = "RhythmBase.RhythmDoctor.Events.IBaseEvent",
				ClassTypeEnumFullname = "RhythmBase.RhythmDoctor.EventType",
				ClassTypeEnumUnknownMemberName = "ForwardEvent",
			},
			new()
			{
				Id = "AD",
				SourceNamespace = "RhythmBase.Adofai.Events",
				TargetConverterNamespace = "RhythmBase.Adofai.Converters",
				TargetUtilsNamespace = "RhythmBase.Adofai.Utils",
				TargetUtilsClassName = "EventTypeUtils",
				BaseConverterClassName = "EventInstanceConverter",
				BaseInterfaceFullName = "RhythmBase.Adofai.Events.IBaseEvent",
				ClassTypeEnumFullname = "RhythmBase.Adofai.EventType",
				ClassTypeEnumUnknownMemberName = "ForwardEvent",
			},
			new()
			{
				Id = "Filter",
				SourceNamespace = "RhythmBase.Adofai.Components.Filters",
				TargetConverterNamespace = "RhythmBase.Adofai.Converters",
				TargetUtilsNamespace = "RhythmBase.Adofai.Utils",
				TargetUtilsClassName = "FilterTypeUtils",
				BaseConverterClassName = "FilterInstanceConverter",
				BaseInterfaceFullName = "RhythmBase.Adofai.Components.Filters.IFilter",
				ClassTypeEnumFullname = "RhythmBase.Adofai.FilterType",
				ClassTypeEnumUnknownMemberName = "Unknown",
			},
			new()
			{
				Id = "BeatBlock",
				SourceNamespace = "RhythmBase.BeatBlock.Events",
				TargetConverterNamespace = "RhythmBase.BeatBlock.Converters",
				TargetUtilsNamespace = "RhythmBase.BeatBlock.Utils",
				TargetUtilsClassName = "EventTypeUtils",
				BaseConverterClassName = "EventInstanceConverter",
				BaseInterfaceFullName = "RhythmBase.BeatBlock.Events.IBaseEvent",
				ClassTypeEnumFullname = "RhythmBase.BeatBlock.EventType",
				ClassTypeEnumUnknownMemberName = "ForwardEvent",
            }
			];
		foreach( var config in configs )
		{
            GenerateConverter(context, config);
        }
		GenerateFilterTypeUtilsForEnum(context);
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
	static bool IsConcreteEnumerable(ITypeSymbol type)
	{
		if (type is IArrayTypeSymbol)
			return true;
		if (type.SpecialType == SpecialType.System_String)
			return false;
		return type.AllInterfaces.Any(i =>
			i.IsGenericType && i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
			|| (!i.IsGenericType && i.SpecialType == SpecialType.System_Collections_IEnumerable));
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

	static	List<string> marks = [];
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