using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace RhythmBase.Generator;

public partial class ConverterGenerator
{
	/// <summary>Input for the node-based read-property emitter.</summary>
	private sealed class GenerateReadPropertyInfo
	{
		public Compilation Compilation { get; }
		public (ITypeSymbol, INamedTypeSymbol)[] ClassGenMap { get; }
		public int Index { get; }
		public ImmutableArray<AttributeData> Attrs { get; }
		public INamedTypeSymbol? DftCvtrAttrSmp { get; }
		public INamedTypeSymbol? JsonEnumAttrSmp { get; }
		public bool IsNullable { get; }
		public INamedTypeSymbol? JsonCvtr { get; }
		public int? TimeEnum { get; }
		public string ValueAccess { get; }
		public ITypeSymbol Type { get; }
		public INamedTypeSymbol ContainingType { get; }

		public GenerateReadPropertyInfo(Compilation compilation, (ITypeSymbol, INamedTypeSymbol)[] classGenMap, int index,
			ImmutableArray<AttributeData> attrs, INamedTypeSymbol? dftCvtrAttrSmp, INamedTypeSymbol? jsonEnumAttrSmp,
			bool isNullable, INamedTypeSymbol? jsonCvtr, int? timeEnum, string valueAccess, ITypeSymbol type, INamedTypeSymbol containingType)
		{
			Compilation = compilation;
			ClassGenMap = classGenMap;
			Index = index;
			Attrs = attrs;
			DftCvtrAttrSmp = dftCvtrAttrSmp;
			JsonEnumAttrSmp = jsonEnumAttrSmp;
			IsNullable = isNullable;
			JsonCvtr = jsonCvtr;
			TimeEnum = timeEnum;
			ValueAccess = valueAccess;
			Type = type;
			ContainingType = containingType;
		}
	}

	private static StatementSyntax ReadAssign(string target, string expression)
		=> SyntaxFactory.ParseStatement($"{target} = {expression};");

	/// <summary>Builds the read statement for a single property with syntax nodes.</summary>
	private StatementSyntax GenerateReadPropertySyntax(GenerateReadPropertyInfo info)
	{
		var jsonCvtr = info.JsonCvtr;
		var classGenMap = info.ClassGenMap;
		string valueAccess = info.ValueAccess;
		var type = info.Type;
		bool isNullable = info.IsNullable;
		int? timeEnum = info.TimeEnum;
		var compilation = info.Compilation;
		var dftCvtrAttrSmp = info.DftCvtrAttrSmp;
		var jsonEnumAttrSmp = info.JsonEnumAttrSmp;
		var attrs = info.Attrs;
		int index = info.Index;

		// JsonUnit 单位转换
		var jsonUnitAttrSmp = compilation.GetTypeByMetadataName(JsonUnitAttrName);
		AttributeData[] unitAttrs = attrs.Where(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, jsonUnitAttrSmp)).ToArray();
		if (unitAttrs.Length > 0 && TryEmitUnitReadSyntax(info, unitAttrs) is StatementSyntax unitStatement)
			return unitStatement;

		// 已注册转换器
		if (jsonCvtr is null && classGenMap.Any(i => i.Item1.Equals(type, SymbolEqualityComparer.Default)))
			return ReadAssign(valueAccess, $"TypeConverterRegistry.Read<{type.ToDisplayString()}>(ref reader, options)");

		// 自定义转换器
		if (jsonCvtr is not null)
		{
			bool metadata = SymbolEqualityComparer.Default.Equals(jsonCvtr.BaseType.OriginalDefinition, compilation.GetTypeByMetadataName(MetadataJsonConverterTypeName));
			return ReadAssign(valueAccess,
				$"new {jsonCvtr.ToDisplayString()}().Read(ref reader, typeof({type.ToDisplayString()}), {(metadata ? "options" : "options.JsonSerializerOptions")})");
		}

		// 时间
		if (timeEnum is int timeType)
		{
			return timeType switch
			{
				0 => ReadAssign(valueAccess, "TimeSpan.FromMilliseconds(reader.GetDouble())"),
				1 => ReadAssign(valueAccess, "TimeSpan.FromSeconds(reader.GetDouble())"),
				_ => SyntaxFactory.Block(SyntaxFactory.ParseStatement("throw new NotImplementedException();")),
			};
		}

		// 枚举
		if (type.TypeKind == TypeKind.Enum)
		{
			if (
				!attrs.Any(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, dftCvtrAttrSmp)) &&
				type.GetAttributes().Any(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, jsonEnumAttrSmp)))
			{
				string enumVar = $"enumValue{index}";
				return SyntaxFactory.Block(SyntaxFactory.IfStatement(
					SyntaxFactory.ParseExpression($"global::RhythmBase.Global.Serialization.EnumConverter.TryParse(ref reader, out {type.ToDisplayString()} {enumVar})"),
					SyntaxFactory.ParseStatement($"{valueAccess} = {enumVar};"),
					SyntaxFactory.ElseClause(SyntaxFactory.ParseStatement("return false;"))));
			}
			return ReadAssign(valueAccess, $"({type.ToDisplayString()})reader.GetInt64()");
		}

		// 默认类型
		if (type.SpecialType is not SpecialType.None)
		{
			string method = GetJsonReadMethod(type);
			string suffix = method == "String" && !isNullable ? " ?? \"\"" : "";
			return ReadAssign(valueAccess, $"reader.Get{method}(){suffix}");
		}

		// 其他：集合
		if (IsConcreteEnumerable(type) is ITypeSymbol elementType)
		{
			string listType = $"System.Collections.Generic.List<{elementType.ToDisplayString()}>";
			StatementSyntax whileLoop = SyntaxFactory.WhileStatement(
				SyntaxFactory.ParseExpression("reader.Read() && reader.TokenType != JsonTokenType.EndArray"),
				SyntaxFactory.Block(
					SyntaxFactory.ParseStatement(ReadElementStatement(elementType, isNullable, jsonEnumAttrSmp)),
					SyntaxFactory.ParseStatement("list.Add(elementValue);")));
			StatementSyntax arrayIf = SyntaxFactory.IfStatement(
				SyntaxFactory.ParseExpression("reader.TokenType == JsonTokenType.StartArray"),
				SyntaxFactory.Block(whileLoop));
			return SyntaxFactory.Block(
				SyntaxFactory.ParseStatement($"var list = new {listType}();"),
				arrayIf,
				SyntaxFactory.ParseStatement($"{valueAccess} = [..list];"));
		}

		return ReadAssign(valueAccess, $"TypeConverterRegistry.Read<{type.ToDisplayString()}>(ref reader, options)");
	}

	private string ReadElementStatement(ITypeSymbol elementType, bool isNullable, INamedTypeSymbol? jsonEnumAttrSmp)
	{
		if (elementType.SpecialType is not SpecialType.None)
		{
			string method = GetJsonReadMethod(elementType);
			string suffix = method == "String" && !isNullable ? " ?? \"\"" : "";
			return $"var elementValue = reader.Get{method}(){suffix};";
		}
		if (elementType.TypeKind == TypeKind.Enum)
		{
			if (elementType.GetAttributes().Any(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, jsonEnumAttrSmp)))
				return $"global::RhythmBase.Global.Serialization.EnumConverter.TryParse(reader.GetString(), out {elementType.ToDisplayString()} elementValue);";
			return $"var elementValue = ({elementType.ToDisplayString()})reader.GetInt64();";
		}
		return $"var elementValue = TypeConverterRegistry.Read<{elementType.ToDisplayString()}>(ref reader, options);";
	}

	/// <summary>Builds the JsonUnit read conversion block with syntax nodes.</summary>
	private StatementSyntax? TryEmitUnitReadSyntax(GenerateReadPropertyInfo info, AttributeData[] unitAttrs)
	{
		var type = info.Type;
		var jsonCvtr = info.JsonCvtr;
		var classGenMap = info.ClassGenMap;
		var compilation = info.Compilation;
		string? raw;
		if (jsonCvtr is not null)
		{
			raw = SymbolEqualityComparer.Default.Equals(jsonCvtr.BaseType.OriginalDefinition, compilation.GetTypeByMetadataName(MetadataJsonConverterTypeName))
				? $"new {jsonCvtr.ToDisplayString()}().Read(ref reader, typeof({type.ToDisplayString()}), options)"
				: $"new {jsonCvtr.ToDisplayString()}().Read(ref reader, typeof({type.ToDisplayString()}), options.JsonSerializerOptions)";
		}
		else if (classGenMap.Any(i => i.Item1.Equals(type, SymbolEqualityComparer.Default)))
			raw = $"TypeConverterRegistry.Read<{type.ToDisplayString()}>(ref reader, options)";
		else if (type.SpecialType == SpecialType.System_Single)
			raw = "reader.GetSingle()";
		else if (type.SpecialType == SpecialType.System_Double)
			raw = "reader.GetDouble()";
		else if (type.SpecialType == SpecialType.System_Int32)
			raw = "reader.GetInt32()";
		else if (type.SpecialType == SpecialType.System_Int64)
			raw = "reader.GetInt64()";
		else
			return null;

		List<StatementSyntax> body = [];
		AttributeData? fn = unitAttrs.FirstOrDefault(a => a.ConstructorArguments.Length == 2 && a.ConstructorArguments[0].Value is string && a.ConstructorArguments[1].Value is string);
		if (fn is AttributeData f)
		{
			string scaleFn = (string)f.ConstructorArguments[0].Value!;
			body.Add(ReadAssign(info.ValueAccess, $"{info.ContainingType.ToDisplayString()}.{scaleFn}({raw})"));
		}
		else
		{
			AttributeData[] members = unitAttrs.Where(a => a.ConstructorArguments.Length >= 2 && a.ConstructorArguments[0].Value is string).ToArray();
			if (members.Length > 0)
			{
				string tmp = $"__unit{info.Index}";
				body.Add(SyntaxFactory.ParseStatement($"var {tmp} = {raw};"));
				foreach (AttributeData a in members)
				{
					string member = (string)a.ConstructorArguments[0].Value!;
					string m = FloatLiteral(a.ConstructorArguments[1].Value);
					string b = a.ConstructorArguments.Length >= 3 ? FloatLiteral(a.ConstructorArguments[2].Value) : "0f";
					body.Add(SyntaxFactory.ParseStatement($"{tmp}.{member} = {tmp}.{member} * {m} + {b};"));
				}
				body.Add(ReadAssign(info.ValueAccess, tmp));
			}
			else
			{
				AttributeData s = unitAttrs[0];
				string sm = FloatLiteral(s.ConstructorArguments[0].Value);
				string sbv = s.ConstructorArguments.Length >= 2 ? FloatLiteral(s.ConstructorArguments[1].Value) : "0f";
				body.Add(SyntaxFactory.ParseStatement($"{info.ValueAccess} = ({type.ToDisplayString()})({raw} * {sm} + {sbv});"));
			}
		}
		return SyntaxFactory.Block(body.ToArray());
	}
}
