using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// Walks a parsed expression tree and produces values. It is shared by
/// <see cref="ExpressionEvaluator"/> and <see cref="ConditionalExpressionEvaluator"/>.
/// </summary>
internal sealed class ExpressionRuntime
{
	private static readonly Random _random = new Random();
	private readonly ExpressionContext _context;
	private readonly Parser _parser;
	private readonly bool _conditional;

	/// <summary>
	/// Initializes a new runtime over the given context.
	/// </summary>
	/// <param name="context">The shared expression state.</param>
	/// <param name="parser">The parser that produced the tree; used for rule/token metadata.</param>
	/// <param name="conditional">Whether unknown identifiers resolve to their own name (conditional mode).</param>
	public ExpressionRuntime(ExpressionContext context, Parser parser, bool conditional)
	{
		_context = context;
		_parser = parser;
		_conditional = conditional;
	}

	/// <summary>
	/// Evaluates a <c>program</c> tree and returns the value of its last statement.
	/// </summary>
	public object? EvalProgram(ParserRuleContext program)
	{
		object? last = null;
		for (int i = 0; i < program.ChildCount; i++)
		{
			if (program.GetChild(i) is ParserRuleContext child && RuleName(child) == "statement")
				last = EvalStatement(child);
		}
		return last;
	}

	/// <summary>
	/// Evaluates a <c>conditional</c> tree.
	/// </summary>
	public object? EvalConditional(ParserRuleContext conditional)
	{
		for (int i = 0; i < conditional.ChildCount; i++)
		{
			if (conditional.GetChild(i) is ParserRuleContext child)
			{
				string rule = RuleName(child);
				if (rule == "mode_keyword")
					return EvalModeKeyword(child);
				if (rule == "expression")
					return EvalExpression(child);
			}
		}
		throw new FormatException("Empty conditional expression.");
	}

	private object? EvalStatement(ParserRuleContext ctx)
	{
		if (ctx.ChildCount == 1 && ctx.GetChild(0) is ParserRuleContext child)
		{
			switch (RuleName(child))
			{
				case "assignment":
					return EvalAssignment(child);
				case "increment":
					return EvalIncrement(child);
				case "decrement":
					return EvalDecrement(child);
				case "expression":
					return EvalExpression(child);
			}
		}
		throw Unexpected(ctx);
	}

	private object? EvalAssignment(ParserRuleContext ctx)
	{
		string name = RequiredChild(ctx, "identifier").GetText();
		object? value = EvalExpression(RequiredChild(ctx, "expression"));
		Assign(name, value);
		return value;
	}

	private object? EvalIncrement(ParserRuleContext ctx)
	{
		string name = RequiredChild(ctx, "identifier").GetText();
		object? current = Resolve(name);
		object result;
		if (current is int integer)
			result = integer + 1;
		else
			result = ToFloat(current) + 1f;
		Assign(name, result);
		return result;
	}

	private object? EvalDecrement(ParserRuleContext ctx)
	{
		string name = RequiredChild(ctx, "identifier").GetText();
		object? current = Resolve(name);
		object result;
		if (current is int integer)
			result = integer - 1;
		else
			result = ToFloat(current) - 1f;
		Assign(name, result);
		return result;
	}

	private object? EvalExpression(ParserRuleContext ctx)
	{
		int count = ctx.ChildCount;

		if (count == 1)
			return EvalPrimary(ctx.GetChild(0));

		if (count == 2)
		{
			string op = ctx.GetChild(0).GetText();
			object? operand = EvalExpression((ParserRuleContext)ctx.GetChild(1));
			switch (op)
			{
				case "NOT":
				case "Not":
					return !ToBool(operand);
				case "+":
					return operand is int ? operand : ToFloat(operand);
				case "-":
					return operand is int i ? -i : -ToFloat(operand);
				default:
					throw new FormatException($"Unknown unary operator '{op}'.");
			}
		}

		if (count == 3)
		{
			if (ctx.GetChild(0) is ITerminalNode first && first.GetText() == "(")
				return EvalExpression((ParserRuleContext)ctx.GetChild(1));

			object? left = EvalExpression((ParserRuleContext)ctx.GetChild(0));
			string op = ctx.GetChild(1).GetText();
			object? right = EvalExpression((ParserRuleContext)ctx.GetChild(2));
			return ApplyBinary(op, left, right);
		}

		throw new FormatException("Unsupported expression shape.");
	}

	private object? EvalPrimary(IParseTree node)
	{
		if (node is ParserRuleContext ctx)
		{
			switch (RuleName(ctx))
			{
				case "function_call":
					return EvalFunctionCall(ctx);
				case "identifier":
					return Resolve(ctx.GetText());
				default:
					throw Unexpected(ctx);
			}
		}

		if (node is ITerminalNode terminal)
		{
			string? symbol = _parser.Vocabulary.GetSymbolicName(terminal.Symbol.Type);
			string text = terminal.GetText();
			switch (symbol)
			{
				case "INT":
					return int.Parse(text, CultureInfo.InvariantCulture);
				case "FLOAT":
					return float.Parse(text, CultureInfo.InvariantCulture);
				case "BOOLEAN":
					return text == "true" || text == "True";
				case "RANK_ENUM":
					return text;
				case "QUOTED_STRING":
					return Unescape(text);
				case "PREFIX_STRING":
					return text.Substring(4);
				default:
					throw new FormatException($"Unsupported literal '{text}'.");
			}
		}

		throw new FormatException("Unsupported expression node.");
	}

	private object? EvalFunctionCall(ParserRuleContext ctx)
	{
		string name = RequiredChild(ctx, "identifier").GetText();
		List<object?> args = new List<object?>();
		for (int i = 0; i < ctx.ChildCount; i++)
		{
			if (ctx.GetChild(i) is ParserRuleContext child && RuleName(child) == "expression")
				args.Add(EvalExpression(child));
		}
		return Invoke(name, args);
	}

	private object? Invoke(string name, List<object?> args)
	{
		switch (name)
		{
			case "Rand":
				if (args.Count != 1)
					throw new FormatException("Rand() expects 1 argument.");
				return RandomInt(ToInt(args[0]));
			case "IIf":
				if (args.Count != 3)
					throw new FormatException("IIf() expects 3 arguments.");
				return ToBool(args[0]) ? args[1] : args[2];
			case "atLeastRank":
				if (args.Count != 1)
					throw new FormatException("atLeastRank() expects 1 argument.");
				return global::RhythmBase.RhythmDoctor.Components.Variables.atLeastRank(ToStr(args[0]));
			case "atLeastNPerfects":
				if (args.Count != 2)
					throw new FormatException("atLeastNPerfects() expects 2 arguments.");
				return global::RhythmBase.RhythmDoctor.Components.Variables.atLeastNPerfects(ToInt(args[0]), ToInt(args[1]));
			default:
				return InvokeEnvironmentFunction(name, args);
		}
	}

	private object? InvokeEnvironmentFunction(string name, List<object?> args)
	{
		MethodInfo? method = _context.Variables.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.Instance);
		if (method == null)
			throw new FormatException($"Function '{name}' is not supported.");

		ParameterInfo[] parameters = method.GetParameters();
		if (parameters.Length != args.Count)
			throw new FormatException($"Function '{name}' expects {parameters.Length} argument(s).");

		object?[] converted = new object?[args.Count];
		for (int i = 0; i < args.Count; i++)
		{
			Type type = parameters[i].ParameterType;
			if (type == typeof(float))
				converted[i] = ToFloat(args[i]);
			else if (type == typeof(int))
				converted[i] = ToInt(args[i]);
			else if (type == typeof(bool))
				converted[i] = ToBool(args[i]);
			else if (type == typeof(string))
				converted[i] = ToStr(args[i]);
			else
				converted[i] = args[i];
		}
		return method.Invoke(_context.Variables, converted);
	}

	private object? Resolve(string name)
	{
		if (ExpressionContext.TryParseFixed(name, out char kind, out int index))
		{
			switch (kind)
			{
				case 'i':
					return _context.I[index];
				case 'f':
					return _context.F[index];
				default:
					return _context.B[index];
			}
		}

		if (TryGetVariable(name, out object? value))
			return value;

		// In conditional strings an unknown identifier denotes an enum-like literal
		// (English, Chinese, Windows, Mac, ...).
		if (_conditional)
			return name;

		throw new FormatException($"Unknown variable '{name}'.");
	}

	private void Assign(string name, object? value)
	{
		if (ExpressionContext.TryParseFixed(name, out char kind, out int index))
		{
			switch (kind)
			{
				case 'i':
					_context.I[index] = ToInt(value);
					return;
				case 'f':
					_context.F[index] = ToFloat(value);
					return;
				default:
					_context.B[index] = ToBool(value);
					return;
			}
		}

		if (TrySetVariable(name, value))
			return;

		throw new FormatException($"Variable '{name}' is read-only or unknown.");
	}

	private object? EvalModeKeyword(ParserRuleContext ctx)
	{
		string text = ctx.GetText();
		if (TryGetVariable(text, out object? direct))
			return ToBool(direct);
		if (TryGetVariable("twoPlayerMode", out object? twoPlayer))
			return text == "2pMode" ? ToBool(twoPlayer) : !ToBool(twoPlayer);
		return text == "1pMode";
	}

	// Variables outside the 30 fixed slots are read/written through the dynamic member
	// protocol, so any VariablesBase (DynamicObject) environment works transparently.
	private bool TryGetVariable(string name, out object? value)
	{
		if (_context.Variables is not DynamicObject dynamicVariables)
			throw new InvalidOperationException("The IVariables environment must derive from System.Dynamic.DynamicObject, for example VariablesBase.");
		return dynamicVariables.TryGetMember(new VariableGetBinder(name), out value);
	}

	private bool TrySetVariable(string name, object? value)
	{
		if (_context.Variables is not DynamicObject dynamicVariables)
			throw new InvalidOperationException("The IVariables environment must derive from System.Dynamic.DynamicObject, for example VariablesBase.");
		return dynamicVariables.TrySetMember(new VariableSetBinder(name), value);
	}

	private static object ApplyBinary(string op, object? left, object? right)
	{
		switch (op)
		{
			case "+":
				if (left is string || right is string)
					return ToStr(left) + ToStr(right);
				if (IsIntegral(left) && IsIntegral(right))
					return ToInt(left) + ToInt(right);
				return ToFloat(left) + ToFloat(right);
			case "-":
				if (IsIntegral(left) && IsIntegral(right))
					return ToInt(left) - ToInt(right);
				return ToFloat(left) - ToFloat(right);
			case "*":
				if (IsIntegral(left) && IsIntegral(right))
					return ToInt(left) * ToInt(right);
				return ToFloat(left) * ToFloat(right);
			case "/":
				return ToFloat(left) / ToFloat(right);
			case "%":
				if (IsIntegral(left) && IsIntegral(right))
					return ToInt(left) % ToInt(right);
				return ToFloat(left) % ToFloat(right);
			case "==":
				return AreEqual(left, right);
			case "!=":
				return !AreEqual(left, right);
			case ">":
				return Compare(left, right) > 0;
			case "<":
				return Compare(left, right) < 0;
			case ">=":
				return Compare(left, right) >= 0;
			case "<=":
				return Compare(left, right) <= 0;
			case "AND":
			case "And":
				return ToBool(left) && ToBool(right);
			case "OR":
			case "Or":
				return ToBool(left) || ToBool(right);
			default:
				throw new FormatException($"Unknown operator '{op}'.");
		}
	}

	private static bool AreEqual(object? left, object? right)
	{
		if (left is string || right is string)
			return string.Equals(ToStr(left), ToStr(right), StringComparison.Ordinal);
		return ToFloat(left) == ToFloat(right);
	}

	private static int Compare(object? left, object? right)
	{
		if (left is string || right is string)
			return string.CompareOrdinal(ToStr(left), ToStr(right));
		return ToFloat(left).CompareTo(ToFloat(right));
	}

	private static bool IsIntegral(object? value) => value is int || value is bool;

	private static int RandomInt(int value)
	{
		if (value > 0)
			return _random.Next(1, value + 1);
		if (value < 0)
			return -_random.Next(1, -value + 1);
		return 0;
	}

	private string RuleName(ParserRuleContext ctx) => _parser.RuleNames[ctx.RuleIndex];

	private ParserRuleContext RequiredChild(ParserRuleContext ctx, string rule)
	{
		for (int i = 0; i < ctx.ChildCount; i++)
		{
			if (ctx.GetChild(i) is ParserRuleContext child && RuleName(child) == rule)
				return child;
		}
		throw new FormatException($"Missing '{rule}' child.");
	}

	private FormatException Unexpected(ParserRuleContext ctx) => new FormatException($"Unexpected rule '{RuleName(ctx)}'.");

	private static string Unescape(string quoted)
	{
		string inner = quoted.Substring(1, quoted.Length - 2);
		StringBuilder builder = new StringBuilder(inner.Length);
		for (int i = 0; i < inner.Length; i++)
		{
			char c = inner[i];
			if (c == '\\' && i + 1 < inner.Length)
			{
				i++;
				char next = inner[i];
				switch (next)
				{
					case 'n':
						builder.Append('\n');
						break;
					case 't':
						builder.Append('\t');
						break;
					case 'r':
						builder.Append('\r');
						break;
					default:
						builder.Append(next);
						break;
				}
			}
			else
			{
				builder.Append(c);
			}
		}
		return builder.ToString();
	}

	/// <summary>Converts a runtime value to a float.</summary>
	internal static float ToFloat(object? value)
	{
		switch (value)
		{
			case float f:
				return f;
			case int i:
				return i;
			case bool b:
				return b ? 1f : 0f;
			case string s:
				return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed) ? parsed : 0f;
			default:
				return 0f;
		}
	}

	/// <summary>Converts a runtime value to an integer.</summary>
	internal static int ToInt(object? value)
	{
		switch (value)
		{
			case int i:
				return i;
			case float f:
				return (int)Math.Round(f);
			case bool b:
				return b ? 1 : 0;
			case string s:
				return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed) ? parsed : 0;
			default:
				return 0;
		}
	}

	/// <summary>Converts a runtime value to a boolean.</summary>
	internal static bool ToBool(object? value)
	{
		switch (value)
		{
			case bool b:
				return b;
			case int i:
				return i != 0;
			case float f:
				return f != 0f;
			case string s:
				return string.Equals(s, "true", StringComparison.OrdinalIgnoreCase);
			default:
				return false;
		}
	}

	/// <summary>Converts a runtime value to a string.</summary>
	internal static string ToStr(object? value)
	{
		switch (value)
		{
			case null:
				return "";
			case string s:
				return s;
			case float f:
				return f.ToString(CultureInfo.InvariantCulture);
			case int i:
				return i.ToString(CultureInfo.InvariantCulture);
			case bool b:
				return b ? "true" : "false";
			default:
				return value.ToString() ?? "";
		}
	}

	// Minimal binders carrying only the member name; they are consumed by
	// VariablesBase.TryGetMember / TrySetMember, so the fallbacks are only reached when the
	// environment reports the member as missing (or read-only).
	private sealed class VariableGetBinder : GetMemberBinder
	{
		public VariableGetBinder(string name)
			: base(name, ignoreCase: false)
		{
		}

		public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject? errorSuggestion)
			=> errorSuggestion ?? throw new InvalidOperationException($"Variable '{Name}' does not exist.");
	}

	private sealed class VariableSetBinder : SetMemberBinder
	{
		public VariableSetBinder(string name)
			: base(name, ignoreCase: false)
		{
		}

		public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject? errorSuggestion)
			=> errorSuggestion ?? throw new InvalidOperationException($"Variable '{Name}' is read-only or unknown.");
	}
}
