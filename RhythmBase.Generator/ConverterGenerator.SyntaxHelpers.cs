using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RhythmBase.Generator;

/// <summary>
/// Small SyntaxFactory helpers used by the code-generation stages. They keep the emitted code
/// structurally valid and let <c>NormalizeWhitespace</c> format the final source instead of
/// hand-maintained indentation.
/// </summary>
public partial class ConverterGenerator
{
	private static readonly SymbolDisplayFormat SynFullyQualified = SymbolDisplayFormat.FullyQualifiedFormat;

	private static readonly SymbolDisplayFormat SynQualifiedMember = SymbolDisplayFormat.FullyQualifiedFormat
		.AddMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType);

	private static TypeSyntax SynType(ITypeSymbol symbol) => SyntaxFactory.ParseTypeName(symbol.ToDisplayString(SynFullyQualified));

	private static TypeSyntax SynType(string displayName) => SyntaxFactory.ParseTypeName(displayName);

	private static ExpressionSyntax SynString(string value)
		=> SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));

	private static ExpressionSyntax SynName(string identifier) => SyntaxFactory.IdentifierName(identifier);

	private static ExpressionSyntax SynMember(ExpressionSyntax target, string name)
		=> SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, target, SyntaxFactory.IdentifierName(name));

	private static ExpressionSyntax SynInvoke(ExpressionSyntax target, string method, params ExpressionSyntax[] arguments)
		=> SyntaxFactory.InvocationExpression(
			SynMember(target, method),
			SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments.Select(SyntaxFactory.Argument))));

	private static StatementSyntax SynCall(string receiverExpression, string method, params ExpressionSyntax[] arguments)
		=> SyntaxFactory.ExpressionStatement(SynInvoke(SyntaxFactory.ParseExpression(receiverExpression), method, arguments));

	private static StatementSyntax SynCompare(string alias, string modelPath, string leftProperty, string rightProperty)
		=> SynCall("c", "Compare", SynString(alias), SynString(modelPath), SynMember(SynName("left"), leftProperty), SynMember(SynName("right"), rightProperty));

	private static StatementSyntax SynCompareSequence()
		=> SynCall(
			"c",
			"CompareSequence",
			SynString(""),
			SyntaxFactory.CastExpression(SynType("global::System.Collections.IEnumerable"), SynName("left")),
			SyntaxFactory.CastExpression(SynType("global::System.Collections.IEnumerable"), SynName("right")));

	private static StatementSyntax SynCompareExtraData(string extraDataAccess)
		=> SynCall(
			"global::RhythmBase.Global.Diff.DiffCollector",
			"CompareExtraData",
			SynName("c"),
			SynMember(SynName("left"), extraDataAccess),
			SynMember(SynName("right"), extraDataAccess),
			SynString(extraDataAccess));

	private static FieldDeclarationSyntax SynStaticReadOnlyField(string typeName, string fieldName, ExpressionSyntax initializer)
		=> SyntaxFactory.FieldDeclaration(
				SyntaxFactory.VariableDeclaration(SynType(typeName))
					.AddVariables(SyntaxFactory.VariableDeclarator(fieldName).WithInitializer(SyntaxFactory.EqualsValueClause(initializer))))
			.AddModifiers(
				SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
				SyntaxFactory.Token(SyntaxKind.StaticKeyword),
				SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));

	private static ExpressionSyntax SynCollectionEntry(ExpressionSyntax key, ExpressionSyntax value)
		=> SyntaxFactory.AssignmentExpression(
			SyntaxKind.SimpleAssignmentExpression,
			SyntaxFactory.ImplicitElementAccess(
				SyntaxFactory.BracketedArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(key)))),
			value);

	private static ObjectCreationExpressionSyntax SynNewWithInitializer(string typeName, IEnumerable<ExpressionSyntax> entries)
		=> SyntaxFactory.ObjectCreationExpression(SynType(typeName))
			.WithArgumentList(SyntaxFactory.ArgumentList())
			.WithInitializer(SyntaxFactory.InitializerExpression(
				SyntaxKind.CollectionInitializerExpression,
				SyntaxFactory.SeparatedList(entries)));


	private static string SynEscapeString(string value)
		=> value.Replace("\\", "\\\\").Replace("\"", "\\\"");

	/// <summary>Builds <c>writer.WriteXxx("alias"u8, valueAccess);</c>.</summary>
	private static StatementSyntax SynWriteScalar(string writerMethod, string alias, string valueAccess)
		=> SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
			SyntaxFactory.MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,
				SyntaxFactory.IdentifierName("writer"),
				SyntaxFactory.IdentifierName($"Write{writerMethod}")),
			SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
			{
				SyntaxFactory.Argument(SyntaxFactory.ParseExpression($"\"{SynEscapeString(alias)}\"u8")),
				SyntaxFactory.Argument(SyntaxFactory.ParseExpression(valueAccess)),
			}))));

	/// <summary>Builds the read-branch condition <c>reader.ValueTextEquals("alias"u8) &amp;&amp; reader.Read()</c>.</summary>
	private static ExpressionSyntax SynReaderMatch(string alias)
		=> SyntaxFactory.ParseExpression($"reader.ValueTextEquals(\"{SynEscapeString(alias)}\"u8) && reader.Read()");

	private static string FloatLiteral(object? value)
	{
		float f = Convert.ToSingle(value, System.Globalization.CultureInfo.InvariantCulture);
		return f.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "f";
	}
}
