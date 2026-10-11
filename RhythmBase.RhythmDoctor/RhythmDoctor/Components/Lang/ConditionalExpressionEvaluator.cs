using System;
using Antlr4.Runtime;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// Parses and evaluates the <c>ConditionalExpression</c> grammar, the string form used by
/// Rhythm Doctor's <c>Conditional_Custom</c> conditionals.
/// </summary>
/// <remarks>
/// Every accepted form is an expression interpreted as a boolean, except the two player-mode
/// keywords <c>2pMode</c> / <c>1pMode</c>. Every identifier and function is resolved from the
/// shared <see cref="ExpressionContext"/>; unknown identifiers are treated as enum-like literals
/// (for example <c>English</c>, <c>Chinese</c>, <c>Windows</c>, <c>Mac</c>) when the context's
/// <see cref="ExpressionContext.UnknownMembers"/> policy leaves them unresolved.
/// </remarks>
public sealed class ConditionalExpressionEvaluator
{
	private readonly ExpressionContext _context;

	/// <summary>
	/// Initializes a new evaluator over a new, private context.
	/// </summary>
	public ConditionalExpressionEvaluator()
		: this(new ExpressionContext())
	{
	}

	/// <summary>
	/// Initializes a new evaluator over an existing shared context.
	/// </summary>
	/// <param name="context">The shared context; must not be <see langword="null"/>.</param>
	/// <exception cref="ArgumentNullException">When <paramref name="context"/> is <see langword="null"/>.</exception>
	public ConditionalExpressionEvaluator(ExpressionContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}

	/// <summary>
	/// Gets the shared expression state.
	/// </summary>
	public ExpressionContext Context => _context;

	/// <summary>
	/// Evaluates a conditional string.
	/// </summary>
	/// <param name="code">The conditional string to evaluate.</param>
	/// <returns>The boolean result; <see langword="false"/> for an empty input.</returns>
	/// <exception cref="FormatException">When the string cannot be parsed or evaluated.</exception>
	public bool Check(string code)
	{
		if (string.IsNullOrWhiteSpace(code))
			return false;

		ConditionalExpressionParser parser = CreateParser(code);
		ConditionalExpressionParser.ConditionalContext tree = parser.conditional();
		if (parser.NumberOfSyntaxErrors > 0)
			throw new FormatException("Invalid conditional expression.");
		return ExpressionRuntime.ToBool(new ExpressionRuntime(_context, parser, conditional: true).EvalConditional(tree));
	}

	/// <summary>
	/// Tries to evaluate a conditional string.
	/// </summary>
	/// <param name="code">The conditional string to evaluate.</param>
	/// <param name="result">The result when successful; otherwise <see langword="false"/>.</param>
	/// <returns><see langword="true"/> when evaluation succeeds.</returns>
	public bool TryCheck(string code, out bool result)
	{
		try
		{
			result = Check(code);
			return true;
		}
		catch
		{
			result = false;
			return false;
		}
	}

	private static ConditionalExpressionParser CreateParser(string code)
	{
		ConditionalExpressionLexer lexer = new ConditionalExpressionLexer(CharStreams.fromString(code));
		lexer.RemoveErrorListeners();
		lexer.AddErrorListener(ThrowingErrorListener.Instance);
		CommonTokenStream tokens = new CommonTokenStream(lexer);
		ConditionalExpressionParser parser = new ConditionalExpressionParser(tokens);
		parser.RemoveErrorListeners();
		parser.AddErrorListener(ThrowingErrorListener.Instance);
		return parser;
	}
}
