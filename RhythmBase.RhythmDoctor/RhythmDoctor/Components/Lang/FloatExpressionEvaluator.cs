using System;
using Antlr4.Runtime;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// Parses and evaluates the pure <c>Expression</c> grammar, the form used by numeric
/// fields and <c>{...}</c> text interpolation (the game's <c>FloatExpression</c>).
/// </summary>
/// <remarks>
/// The evaluator has no side effects: it resolves every identifier and function from the shared
/// <see cref="ExpressionContext"/>. Unlike <see cref="CallCustomMethodEvaluator"/>, assignments
/// and increments are rejected by the grammar; use <see cref="CallCustomMethodEvaluator"/> to
/// execute them.
/// </remarks>
public sealed class FloatExpressionEvaluator
{
	private readonly ExpressionContext _context;

	/// <summary>
	/// Initializes a new evaluator over a new, private context.
	/// </summary>
	public FloatExpressionEvaluator()
		: this(new ExpressionContext())
	{
	}

	/// <summary>
	/// Initializes a new evaluator over an existing shared context.
	/// </summary>
	/// <param name="context">The shared context; must not be <see langword="null"/>.</param>
	/// <exception cref="ArgumentNullException">When <paramref name="context"/> is <see langword="null"/>.</exception>
	public FloatExpressionEvaluator(ExpressionContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}

	/// <summary>
	/// Gets the shared expression state.
	/// </summary>
	public ExpressionContext Context => _context;

	/// <summary>
	/// Evaluates a pure expression and returns its value.
	/// </summary>
	/// <param name="code">The expression to evaluate.</param>
	/// <returns>The value, or <see langword="null"/> for an empty input.</returns>
	/// <exception cref="FormatException">When the code cannot be parsed or evaluated.</exception>
	public object? Evaluate(string code)
	{
		if (string.IsNullOrWhiteSpace(code))
			return null;

		ExpressionParser parser = CreateParser(code);
		ExpressionParser.EntryContext tree = parser.entry();
		if (parser.NumberOfSyntaxErrors > 0)
			throw new FormatException("Invalid expression.");
		return new ExpressionRuntime(_context, parser, conditional: false).EvalEntry(tree);
	}

	/// <summary>
	/// Tries to evaluate a pure expression.
	/// </summary>
	/// <param name="code">The code to evaluate.</param>
	/// <param name="value">The value when evaluation succeeds; otherwise <see langword="null"/>.</param>
	/// <returns><see langword="true"/> when evaluation succeeds.</returns>
	public bool TryEvaluate(string code, out object? value)
	{
		try
		{
			value = Evaluate(code);
			return true;
		}
		catch
		{
			value = null;
			return false;
		}
	}

	/// <summary>
	/// Evaluates a pure expression and coerces the result to a float.
	/// </summary>
	/// <param name="code">The expression to evaluate.</param>
	/// <returns>The numeric value.</returns>
	public float EvaluateFloat(string code) => ExpressionRuntime.ToFloat(Evaluate(code));

	/// <summary>
	/// Tries to evaluate a pure expression as a float.
	/// </summary>
	/// <param name="code">The expression to evaluate.</param>
	/// <param name="result">The numeric result when successful; otherwise <c>0</c>.</param>
	/// <returns><see langword="true"/> when the code evaluated successfully.</returns>
	public bool TryEvaluateFloat(string code, out float result)
	{
		try
		{
			result = EvaluateFloat(code);
			return true;
		}
		catch
		{
			result = 0f;
			return false;
		}
	}

	/// <summary>
	/// Evaluates a pure expression and coerces the result to a boolean.
	/// </summary>
	/// <param name="code">The expression to evaluate.</param>
	/// <returns>The boolean value.</returns>
	public bool EvaluateBoolean(string code) => ExpressionRuntime.ToBool(Evaluate(code));

	/// <summary>
	/// Evaluates a pure expression and coerces the result to a string.
	/// </summary>
	/// <param name="code">The expression to evaluate.</param>
	/// <returns>The string value.</returns>
	public string EvaluateString(string code) => ExpressionRuntime.ToStr(Evaluate(code));

	private static ExpressionParser CreateParser(string code)
	{
		ExpressionLexer lexer = new ExpressionLexer(CharStreams.fromString(code));
		lexer.RemoveErrorListeners();
		lexer.AddErrorListener(ThrowingErrorListener.Instance);
		CommonTokenStream tokens = new CommonTokenStream(lexer);
		ExpressionParser parser = new ExpressionParser(tokens);
		parser.RemoveErrorListeners();
		parser.AddErrorListener(ThrowingErrorListener.Instance);
		return parser;
	}
}
