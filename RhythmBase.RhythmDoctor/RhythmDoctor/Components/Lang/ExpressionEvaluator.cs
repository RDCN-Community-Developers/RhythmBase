using System;
using Antlr4.Runtime;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// Parses and evaluates the <c>Expression</c> grammar used by numeric fields, text
/// interpolation and CallCustomMethod arguments.
/// </summary>
/// <remarks>
/// Assignments, increments and decrements persist in the <see cref="Context"/>, so the same
/// instance can be reused to evaluate a whole level. The 30 fixed slots live in the context;
/// every other identifier comes from the <see cref="IVariables"/> virtual environment.
/// </remarks>
public sealed class ExpressionEvaluator
{
	private readonly ExpressionContext _context;

	/// <summary>
	/// Initializes a new evaluator over a virtual environment, creating a private context.
	/// </summary>
	/// <param name="variables">The virtual environment; must not be <see langword="null"/>.</param>
	public ExpressionEvaluator(IVariables variables)
		: this(new ExpressionContext(variables))
	{
	}

	/// <summary>
	/// Initializes a new evaluator over an existing shared context.
	/// </summary>
	/// <param name="context">The shared context; must not be <see langword="null"/>.</param>
	/// <exception cref="ArgumentNullException">When <paramref name="context"/> is <see langword="null"/>.</exception>
	public ExpressionEvaluator(ExpressionContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}

	/// <summary>
	/// Gets the shared expression state.
	/// </summary>
	public ExpressionContext Context => _context;

	/// <summary>
	/// Gets the virtual environment used for identifiers outside the 30 fixed slots.
	/// </summary>
	public IVariables Variables => _context.Variables;

	/// <summary>
	/// Evaluates a piece of code and returns its value.
	/// </summary>
	/// <param name="code">The expression (or statement sequence) to evaluate.</param>
	/// <returns>The value of the last statement, or <see langword="null"/> for an empty input.</returns>
	/// <exception cref="FormatException">When the code cannot be parsed or evaluated.</exception>
	public object? Evaluate(string code)
	{
		if (string.IsNullOrWhiteSpace(code))
			return null;

		ExpressionParser parser = CreateParser(code);
		ExpressionParser.ProgramContext tree = parser.program();
		if (parser.NumberOfSyntaxErrors > 0)
			throw new FormatException("Invalid expression.");
		return new ExpressionRuntime(_context, parser, conditional: false).EvalProgram(tree);
	}

	/// <summary>
	/// Tries to evaluate a piece of code.
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
	/// Evaluates a piece of code and coerces the result to a float.
	/// </summary>
	/// <param name="code">The code to evaluate.</param>
	/// <returns>The numeric value.</returns>
	public float EvaluateFloat(string code) => ExpressionRuntime.ToFloat(Evaluate(code));

	/// <summary>
	/// Evaluates a piece of code and coerces the result to a boolean.
	/// </summary>
	/// <param name="code">The code to evaluate.</param>
	/// <returns>The boolean value.</returns>
	public bool EvaluateBoolean(string code) => ExpressionRuntime.ToBool(Evaluate(code));

	/// <summary>
	/// Evaluates a piece of code and coerces the result to a string.
	/// </summary>
	/// <param name="code">The code to evaluate.</param>
	/// <returns>The string value.</returns>
	public string EvaluateString(string code) => ExpressionRuntime.ToStr(Evaluate(code));

	/// <summary>
	/// Evaluates a piece of code and returns the result as a float.
	/// </summary>
	/// <param name="code">The code to evaluate.</param>
	/// <returns>The numeric result.</returns>
	public float Run(string code) => EvaluateFloat(code);

	/// <summary>
	/// Tries to run a piece of code and return the result as a float.
	/// </summary>
	/// <param name="code">The code to run.</param>
	/// <param name="result">The numeric result when successful; otherwise <c>0</c>.</param>
	/// <returns><see langword="true"/> when the code ran successfully.</returns>
	public bool TryRun(string code, out float result)
	{
		try
		{
			result = Run(code);
			return true;
		}
		catch
		{
			result = 0f;
			return false;
		}
	}

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
