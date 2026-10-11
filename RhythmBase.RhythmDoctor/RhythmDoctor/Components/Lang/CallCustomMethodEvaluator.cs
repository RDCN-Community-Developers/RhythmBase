using System;
using Antlr4.Runtime;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// Parses and executes the <c>CallCustomMethod</c> grammar: exactly one statement that either
/// assigns, increments, decrements a target, or invokes a function.
/// </summary>
/// <remarks>
/// <para>
/// The target of an assignment/increment/decrement is resolved as follows: a fixed slot
/// (<c>i0</c>–<c>b9</c>) is written to the shared <see cref="ExpressionContext"/>; any other
/// name (including dotted/indexed names such as <c>level.foo</c>, <c>room[0].foo</c> or
/// <c>vfx.foo</c>) is routed through <see cref="ExpressionContext.TrySetVariable"/>, whose
/// <see cref="ExpressionContext.TrySetMemberValue"/> override decides whether the write is
/// accepted (falling back to <see cref="ExpressionContext.UnknownMembers"/>). Functions are first
/// offered to <see cref="ExpressionContext.TryInvokeFunction"/>, so the context can dispatch
/// prefixed/dotted names such as <c>vfx.foo(...)</c>, <c>room[0].foo(...)</c> and
/// <c>level.foo(...)</c> before the evaluator falls back to reflecting a plain method name.
/// </para>
/// <para>
/// Because the slots and every declared member live in the shared
/// <see cref="ExpressionContext"/>, pass the same context to every evaluator that must observe a
/// previous assignment; otherwise the write is confined to a throwaway context and appears
/// ineffective.
/// </para>
/// </remarks>
public sealed class CallCustomMethodEvaluator
{
	private readonly ExpressionContext _context;

	/// <summary>
	/// Initializes a new evaluator over a new, private context.
	/// </summary>
	public CallCustomMethodEvaluator()
		: this(new ExpressionContext())
	{
	}

	/// <summary>
	/// Initializes a new evaluator over an existing shared context.
	/// </summary>
	/// <param name="context">The shared context; must not be <see langword="null"/>.</param>
	/// <exception cref="ArgumentNullException">When <paramref name="context"/> is <see langword="null"/>.</exception>
	public CallCustomMethodEvaluator(ExpressionContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
	}

	/// <summary>
	/// Gets the shared expression state.
	/// </summary>
	public ExpressionContext Context => _context;

	/// <summary>
	/// Executes one CallCustomMethod statement and returns its value.
	/// </summary>
	/// <param name="code">The statement, for example <c>foo = 1</c>, <c>bar++</c> or <c>FadeOut(1)</c>.</param>
	/// <returns>The statement value, or <see langword="null"/> for an empty input.</returns>
	/// <exception cref="FormatException">When the code cannot be parsed or executed.</exception>
	public object? Execute(string code)
	{
		if (string.IsNullOrWhiteSpace(code))
			return null;

		CallCustomMethodParser parser = CreateParser(code);
		CallCustomMethodParser.ProgramContext tree = parser.program();
		if (parser.NumberOfSyntaxErrors > 0)
			throw new FormatException("Invalid CallCustomMethod code.");
		return new ExpressionRuntime(_context, parser, conditional: false).EvalProgram(tree);
	}

	/// <summary>
	/// Tries to execute one CallCustomMethod statement.
	/// </summary>
	/// <param name="code">The code to execute.</param>
	/// <param name="value">The statement value when execution succeeds; otherwise <see langword="null"/>.</param>
	/// <returns><see langword="true"/> when execution succeeds.</returns>
	public bool TryExecute(string code, out object? value)
	{
		try
		{
			value = Execute(code);
			return true;
		}
		catch
		{
			value = null;
			return false;
		}
	}

	private static CallCustomMethodParser CreateParser(string code)
	{
		CallCustomMethodLexer lexer = new CallCustomMethodLexer(CharStreams.fromString(code));
		lexer.RemoveErrorListeners();
		lexer.AddErrorListener(ThrowingErrorListener.Instance);
		CommonTokenStream tokens = new CommonTokenStream(lexer);
		CallCustomMethodParser parser = new CallCustomMethodParser(tokens);
		parser.RemoveErrorListeners();
		parser.AddErrorListener(ThrowingErrorListener.Instance);
		return parser;
	}
}
