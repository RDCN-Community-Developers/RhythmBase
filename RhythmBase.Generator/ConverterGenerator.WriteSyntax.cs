using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RhythmBase.Generator;

public partial class ConverterGenerator
{
	private static ExpressionSyntax SynCondition(string condition) => SyntaxFactory.ParseExpression(ReplaceVariableWithName(condition));

	private static StatementSyntax WritePropertyName(string alias)
		=> SyntaxFactory.ParseStatement($"writer.WritePropertyName(\"{SynEscapeString(alias)}\"u8);");

	private static StatementSyntax WriteNumberValue(string expression)
		=> SyntaxFactory.ParseStatement($"writer.WriteNumberValue(System.Convert.ToInt64({expression}));");

	/// <summary>
	/// Builds the write statements for one property (without the condition or nullable wrapper).
	/// Returns whether the statements must be wrapped in a block when a condition applies.
	/// </summary>
	private (List<StatementSyntax> Statements, bool NeedsConditionBlock) BuildWriteBranch(GenerateWritePropertyInfo info, INamedTypeSymbol containingType)
	{
		var compilation = info.Compilation;
		var classGenMap = info.ClassGenMap;
		var jsonCvtr = info.JsonCvtr;
		var type = info.Type;
		var alias = info.Alias;
		var valueAccess = info.ValueAccess;
		var attrs = info.Attrs;
		var dftCvtrAttrSmp = info.DftCvtrAttrSmp;
		var jsonEnumAttrSmp = info.JsonEnumAttrSmp;
		int? timeEnum = info.TimeEnum;
		var metadataJsonConverterType = compilation.GetTypeByMetadataName(MetadataJsonConverterTypeName);

		var jsonUnitAttrSmp = compilation.GetTypeByMetadataName(JsonUnitAttrName);
		AttributeData[] unitAttrs = attrs.Where(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, jsonUnitAttrSmp)).ToArray();
		if (unitAttrs.Length > 0)
			return (EmitUnitWriteSyntax(info, unitAttrs, metadataJsonConverterType, containingType), true);

		// 已注册转换器
		if (jsonCvtr is null && classGenMap.Any(i => i.Item1.Equals(type, SymbolEqualityComparer.Default)))
		{
			return ([
				WritePropertyName(alias),
				SyntaxFactory.ParseStatement($"TypeConverterRegistry.Write(writer, {valueAccess}, options);"),
			], true);
		}

		// 自定义转换器
		if (jsonCvtr is not null)
		{
			string arg = SymbolEqualityComparer.Default.Equals(jsonCvtr.BaseType.OriginalDefinition, metadataJsonConverterType)
				? "options"
				: "options.JsonSerializerOptions";
			return ([
				SyntaxFactory.Block(
					WritePropertyName(alias),
					SyntaxFactory.ParseStatement($"new {jsonCvtr.ToDisplayString()}().Write(writer, {valueAccess}, {arg});")),
			], false);
		}

		// 时间
		if (timeEnum is int timeType)
		{
			StatementSyntax timeStatement = timeType switch
			{
				0 => SynWriteScalar("Number", alias, $"{valueAccess}.TotalMilliseconds"),
				1 => SynWriteScalar("Number", alias, $"{valueAccess}.TotalSeconds"),
				_ => SyntaxFactory.Block(SyntaxFactory.ParseStatement("throw new NotImplementedException();")),
			};
			return ([timeStatement], false);
		}

		// 枚举
		if (type.TypeKind == TypeKind.Enum)
		{
			bool useUtf8 = !attrs.Any(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, dftCvtrAttrSmp)) &&
				type.GetAttributes().Any(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, jsonEnumAttrSmp));
			return ([useUtf8
				? SynWriteScalar("String", alias, $"{valueAccess}.ToEnumUtf8String()")
				: SynWriteScalar("Number", alias, $"System.Convert.ToInt64({valueAccess})")], false);
		}

		// 默认类型
		if (type.SpecialType is not SpecialType.None)
			return ([SyntaxFactory.Block(SynWriteScalar(GetJsonWriterMethod(type), alias, valueAccess))], false);

		// 集合
		if (IsConcreteEnumerable(type) is ITypeSymbol elementType)
		{
			string itemVar = $"item{info.Index}";
			List<StatementSyntax> elementBody = [];
			if (elementType.SpecialType is not SpecialType.None)
				elementBody.Add(SynWriteScalarValue(GetJsonWriterMethod(elementType), itemVar));
			else if (elementType.TypeKind == TypeKind.Enum)
			{
				if (elementType.GetAttributes().Any(i => SymbolEqualityComparer.Default.Equals(i.AttributeClass, jsonEnumAttrSmp)))
					elementBody.Add(SyntaxFactory.ParseStatement($"writer.WriteStringValue({itemVar}.ToEnumUtf8String());"));
				else
					elementBody.Add(WriteNumberValue(itemVar));
			}
			else
				elementBody.Add(SyntaxFactory.ParseStatement($"TypeConverterRegistry.Write(writer, {itemVar}, options);"));

			return ([SyntaxFactory.Block(
				WritePropertyName(alias),
				SyntaxFactory.ParseStatement("writer.WriteStartArray();"),
				SyntaxFactory.ForEachStatement(
					SyntaxFactory.ParseTypeName("var"),
					SyntaxFactory.Identifier(itemVar),
					SyntaxFactory.ParseExpression(valueAccess),
					SyntaxFactory.Block(elementBody.ToArray())),
				SyntaxFactory.ParseStatement("writer.WriteEndArray();"))], false);
		}

		// 其他
		return ([SyntaxFactory.Block(
			WritePropertyName(alias),
			SyntaxFactory.ParseStatement($"TypeConverterRegistry.Write(writer, {valueAccess}, options);"))], false);
	}

	private static StatementSyntax SynWriteScalarValue(string writerMethod, string expression)
		=> SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
			SyntaxFactory.MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,
				SyntaxFactory.IdentifierName("writer"),
				SyntaxFactory.IdentifierName($"Write{writerMethod}Value")),
			SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
				SyntaxFactory.Argument(SyntaxFactory.ParseExpression(expression))))));

	/// <summary>Builds the full property statements, including the JsonCondition wrapper when present.</summary>
	private List<StatementSyntax> BuildWriteStatements(GenerateWritePropertyInfo info, INamedTypeSymbol containingType, string? conditionText)
	{
		(List<StatementSyntax> statements, bool needsConditionBlock) = BuildWriteBranch(info, containingType);
		bool hasCondition = !string.IsNullOrEmpty(conditionText);
		if (!hasCondition)
			return statements;
		if (needsConditionBlock)
			statements = [SyntaxFactory.Block(statements.ToArray())];
		StatementSyntax body = statements.Count == 1 ? statements[0] : SyntaxFactory.Block(statements.ToArray());
		return [SyntaxFactory.IfStatement(SynCondition(conditionText!), body)];
	}

	/// <summary>Builds the JsonUnit write conversion statements with syntax nodes.</summary>
	private List<StatementSyntax> EmitUnitWriteSyntax(GenerateWritePropertyInfo info, AttributeData[] unitAttrs, INamedTypeSymbol? metadataJsonConverterType, INamedTypeSymbol containingType)
	{
		var compilation = info.Compilation;
		var classGenMap = info.ClassGenMap;
		var jsonCvtr = info.JsonCvtr;
		var type = info.Type;
		var valueAccess = info.ValueAccess;
		var alias = info.Alias;
		string tmp = $"__unit{info.Index}";

		AttributeData? fn = unitAttrs.FirstOrDefault(a => a.ConstructorArguments.Length == 2 && a.ConstructorArguments[0].Value is string && a.ConstructorArguments[1].Value is string);
		if (fn is AttributeData f)
		{
			string reverse = (string)f.ConstructorArguments[1].Value!;
			List<StatementSyntax> list =
			[
				SyntaxFactory.ParseStatement($"var {tmp} = {containingType.ToDisplayString()}.{reverse}({valueAccess});"),
			];
			list.AddRange(EmitNormalWriteSyntax(info, tmp, metadataJsonConverterType));
			return list;
		}

		AttributeData[] members = unitAttrs.Where(a => a.ConstructorArguments.Length >= 2 && a.ConstructorArguments[0].Value is string).ToArray();
		if (members.Length > 0)
		{
			List<StatementSyntax> list = [SyntaxFactory.ParseStatement($"var {tmp} = {valueAccess};")];
			foreach (AttributeData a in members)
			{
				string member = (string)a.ConstructorArguments[0].Value!;
				string m = FloatLiteral(a.ConstructorArguments[1].Value);
				string b = a.ConstructorArguments.Length >= 3 ? FloatLiteral(a.ConstructorArguments[2].Value) : "0f";
				list.Add(SyntaxFactory.ParseStatement($"{tmp}.{member} = ({tmp}.{member} - {b}) / {m};"));
			}
			list.AddRange(EmitNormalWriteSyntax(info, tmp, metadataJsonConverterType));
			return list;
		}

		AttributeData s = unitAttrs[0];
		string sm = FloatLiteral(s.ConstructorArguments[0].Value);
		string sbv = s.ConstructorArguments.Length >= 2 ? FloatLiteral(s.ConstructorArguments[1].Value) : "0f";
		string expr = $"({valueAccess} - {sbv}) / {sm}";
		if (jsonCvtr is not null)
		{
			List<StatementSyntax> list = [SyntaxFactory.ParseStatement($"var {tmp} = ({type.ToDisplayString()})({expr});")];
			list.AddRange(EmitNormalWriteSyntax(info, tmp, metadataJsonConverterType));
			return list;
		}
		return [SynWriteScalar(GetJsonWriterMethod(type), alias, expr)];
	}

	private List<StatementSyntax> EmitNormalWriteSyntax(GenerateWritePropertyInfo info, string expr, INamedTypeSymbol? metadataJsonConverterType)
	{
		var compilation = info.Compilation;
		var classGenMap = info.ClassGenMap;
		var jsonCvtr = info.JsonCvtr;
		var type = info.Type;
		var alias = info.Alias;
		if (jsonCvtr is not null)
		{
			string arg = SymbolEqualityComparer.Default.Equals(jsonCvtr.BaseType.OriginalDefinition, metadataJsonConverterType)
				? "options"
				: "options.JsonSerializerOptions";
			return [
				WritePropertyName(alias),
				SyntaxFactory.ParseStatement($"new {jsonCvtr.ToDisplayString()}().Write(writer, {expr}, {arg});"),
			];
		}
		if (classGenMap.Any(i => i.Item1.Equals(type, SymbolEqualityComparer.Default)))
			return [
				WritePropertyName(alias),
				SyntaxFactory.ParseStatement($"TypeConverterRegistry.Write(writer, {expr}, options);"),
			];
		return [SynWriteScalar(GetJsonWriterMethod(type), alias, expr)];
	}
}
