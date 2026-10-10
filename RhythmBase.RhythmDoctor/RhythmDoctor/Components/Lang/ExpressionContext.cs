using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Expr = System.Linq.Expressions.Expression;
using ParamExpr = System.Linq.Expressions.ParameterExpression;
using NewArrayExpr = System.Linq.Expressions.NewArrayExpression;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// Controls how the context reacts when a member that is neither a fixed slot nor a declared
/// member is read or written through the dynamic member protocol.
/// </summary>
public enum UnknownMemberPolicy
{
	/// <summary>Report the member as missing so the caller raises an error.</summary>
	Throw,

	/// <summary>Accept the access as a no-op without storing anything.</summary>
	Ignore,

	/// <summary>Allocate and remember the member on first access.</summary>
	Create,
}

/// <summary>
/// The single variable carrier shared by expression evaluations: the 30 fixed slots plus every
/// other member, resolved through the dynamic member protocol.
/// </summary>
/// <remarks>
/// <para>
/// Members are organized in partial files: the fixed slots live in
/// <c>ExpressionContext.Slots.cs</c>, the pure built-in functions in
/// <c>ExpressionContext.Functions.cs</c>, the rank enum in <c>ExpressionContext.Rank.cs</c>, the
/// game-mirroring fields in <c>ExpressionContext.Variables.cs</c>, and the replaceable sandbox
/// rules (<see cref="IRankSimulation"/> / <see cref="IPerfectsSimulation"/> and
/// <see cref="DefaultSimulation"/>) in <c>Simulation.cs</c>.
/// </para>
/// <para>
/// The 30 fixed slots <c>i0</c>–<c>i9</c>, <c>f0</c>–<c>f9</c> and <c>b0</c>–<c>b9</c> are exposed
/// both statically (<see cref="I"/>, <see cref="F"/>, <see cref="B"/>) and dynamically as
/// <c>i0</c>, <c>f9</c>, <c>b0</c> and so on. The arrays and the other members declared on this
/// type are deliberately reachable only statically: the custom dynamic meta object routes every
/// dynamic access through <see cref="TryGetVariable"/>, <see cref="TrySetVariable"/> and
/// <see cref="TryInvokeFunction"/>, so a dynamic <c>I</c> or <c>UnknownMembers</c> is treated as
/// an unknown member rather than the declared member.
/// </para>
/// <para>
/// Declared fields (for example <c>bpm</c> or <c>p1Press</c>) are read and written by name through
/// <see cref="TryGetMemberValue"/> / <see cref="TrySetMemberValue"/>, which reflect over the public
/// instance fields like the game's <c>LevelBase.GetVariable</c>. Derive from this type to add more
/// fields; override those members to customize routing. Names that are not handled fall through to
/// <see cref="UnknownMembers"/>.
/// </para>
/// </remarks>
public partial class ExpressionContext : IDynamicMetaObjectProvider
{
	private static readonly HashSet<string> _reserved =
	[
with(StringComparer.Ordinal),
		nameof(I), nameof(F), nameof(B), nameof(UnknownMembers),
		nameof(Rand), nameof(IIf), nameof(RankSimulation), nameof(PerfectsSimulation),
	];

	private readonly Dictionary<string, object?> _extra = [with(StringComparer.Ordinal)];
	private Dictionary<string, FieldInfo>? _memberFields;

	/// <summary>
	/// Initializes a new context with a private <see cref="DefaultSimulation"/> backing both
	/// sandbox rules.
	/// </summary>
	public ExpressionContext()
	{
		DefaultSimulation simulation = new DefaultSimulation();
		RankSimulation = simulation;
		PerfectsSimulation = simulation;
	}

	/// <summary>
	/// Gets or sets the sandbox rule backing <c>atLeastRank</c>. This property itself is not
	/// dynamically accessible.
	/// </summary>
	public IRankSimulation RankSimulation { get; set; }

	/// <summary>
	/// Gets or sets the sandbox rule backing <c>atLeastNPerfects</c>. This property itself is not
	/// dynamically accessible.
	/// </summary>
	public IPerfectsSimulation PerfectsSimulation { get; set; }

	/// <summary>
	/// Gets or sets how undeclared members behave under the dynamic member protocol. This
	/// property itself is not dynamically accessible.
	/// </summary>
	public UnknownMemberPolicy UnknownMembers { get; set; } = UnknownMemberPolicy.Throw;

	/// <summary>
	/// Gets or sets a variable by name through the same routing used by the dynamic member
	/// protocol. Reading an unresolved name yields <see langword="null"/>; writing one throws.
	/// </summary>
	/// <param name="name">The variable name.</param>
	/// <returns>The resolved value, or <see langword="null"/> when the name is unresolved.</returns>
	/// <exception cref="InvalidOperationException">When a value cannot be assigned.</exception>
	public object? this[string name]
	{
		get => TryGetVariable(name, out object? value) ? value : null;
		set
		{
			if (!TrySetVariable(name, value))
				throw new InvalidOperationException($"Variable '{name}' is read-only or unknown.");
		}
	}

	/// <summary>
	/// Resolves a variable by name: the 30 fixed slots, then <see cref="TryGetMemberValue"/>, then
	/// <see cref="UnknownMembers"/>.
	/// </summary>
	/// <param name="name">The variable name.</param>
	/// <param name="value">The resolved value when found.</param>
	/// <returns><see langword="true"/> when a value was produced.</returns>
	public bool TryGetVariable(string name, out object? value)
	{
		if (TryParseFixed(name, out char kind, out int index))
		{
			value = kind switch
			{
				'i' => I[index],
				'f' => F[index],
				_ => B[index],
			};
			return true;
		}

		if (_reserved.Contains(name))
		{
			value = null;
			return false;
		}

		if (TryGetMemberValue(name, out value))
			return true;

		return PolicyGet(name, out value);
	}

	/// <summary>
	/// Assigns a variable by name: the 30 fixed slots, then <see cref="TrySetMemberValue"/>, then
	/// <see cref="UnknownMembers"/>.
	/// </summary>
	/// <param name="name">The variable name.</param>
	/// <param name="value">The value to assign.</param>
	/// <returns><see langword="true"/> when the value was accepted.</returns>
	public bool TrySetVariable(string name, object? value)
	{
		if (TryParseFixed(name, out char kind, out int index))
		{
			switch (kind)
			{
				case 'i':
					I[index] = ExpressionRuntime.ToInt(value);
					return true;
				case 'f':
					F[index] = ExpressionRuntime.ToFloat(value);
					return true;
				default:
					B[index] = ExpressionRuntime.ToBool(value);
					return true;
			}
		}

		if (_reserved.Contains(name))
			return false;

		if (TrySetMemberValue(name, value))
			return true;

		return PolicySet(name, value);
	}

	/// <summary>
	/// Invokes a function exposed by the context. The base implementation handles the pure
	/// <c>Rand</c> / <c>IIf</c> delegates plus <c>atLeastRank</c> / <c>atLeastNPerfects</c> through
	/// <see cref="RankSimulation"/> and <see cref="PerfectsSimulation"/>; override to add more.
	/// </summary>
	/// <param name="name">The function name.</param>
	/// <param name="args">The evaluated arguments.</param>
	/// <param name="result">The invocation result when handled.</param>
	/// <returns><see langword="true"/> when the function was handled.</returns>
	public virtual bool TryInvokeFunction(string name, IReadOnlyList<object?> args, out object? result)
	{
		switch (name)
		{
			case "Rand":
				RequireArgs(name, args, 1);
				result = Rand(ExpressionRuntime.ToInt(args[0]));
				return true;
			case "IIf":
				RequireArgs(name, args, 3);
				result = IIf(ExpressionRuntime.ToBool(args[0]), args[1], args[2]);
				return true;
			case "atLeastRank":
				RequireArgs(name, args, 1);
				result = RankSimulation.AtLeast(ExpressionRuntime.ToStr(args[0]));
				return true;
			case "atLeastNPerfects":
				RequireArgs(name, args, 2);
				result = PerfectsSimulation.AtLeastNPerfects(ExpressionRuntime.ToInt(args[0]), ExpressionRuntime.ToInt(args[1]));
				return true;
			default:
				result = null;
				return false;
		}
	}

	/// <summary>
	/// Enumerates the names currently reachable through the dynamic member protocol.
	/// </summary>
	/// <returns>The fixed slot names followed by the dynamically created member names.</returns>
	public IEnumerable<string> GetDynamicMemberNames()
	{
		for (int i = 0; i < SlotCount; i++)
		{
			yield return "i" + i;
			yield return "f" + i;
			yield return "b" + i;
		}
		foreach (string name in _extra.Keys)
			yield return name;
	}

	/// <summary>
	/// Tries to read a declared field by name. Override to route members that are not fields.
	/// </summary>
	/// <param name="name">The member name.</param>
	/// <param name="value">The value when the member exists.</param>
	/// <returns><see langword="true"/> when the member exists and is readable.</returns>
	protected virtual bool TryGetMemberValue(string name, out object? value)
	{
		FieldInfo? field = FindField(name);
		if (field != null)
		{
			value = field.GetValue(this);
			return true;
		}
		value = null;
		return false;
	}

	/// <summary>
	/// Tries to write a declared field by name. Override to route members that are not fields.
	/// </summary>
	/// <param name="name">The member name.</param>
	/// <param name="value">The value to assign.</param>
	/// <returns><see langword="true"/> when the member was assigned.</returns>
	protected virtual bool TrySetMemberValue(string name, object? value)
	{
		FieldInfo? field = FindField(name);
		if (field == null)
			return false;
		field.SetValue(this, ConvertForField(field.FieldType, value));
		return true;
	}

	/// <inheritdoc/>
	DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expr parameter) => new ContextMetaObject(parameter, this);

	private bool PolicyGet(string name, out object? value)
	{
		switch (UnknownMembers)
		{
			case UnknownMemberPolicy.Create:
				if (!_extra.TryGetValue(name, out value))
					_extra[name] = value = null;
				return true;
			case UnknownMemberPolicy.Ignore:
				value = null;
				return true;
			default:
				value = null;
				return false;
		}
	}

	private bool PolicySet(string name, object? value)
	{
		switch (UnknownMembers)
		{
			case UnknownMemberPolicy.Create:
				_extra[name] = value;
				return true;
			case UnknownMemberPolicy.Ignore:
				return true;
			default:
				return false;
		}
	}

	private static void RequireArgs(string name, IReadOnlyList<object?> args, int count)
	{
		if (args.Count != count)
			throw new FormatException($"{name}() expects {count} argument(s).");
	}

	private FieldInfo? FindField(string name)
	{
		_memberFields ??= BuildFieldMap();
		return _memberFields.TryGetValue(name, out FieldInfo? field) ? field : null;
	}

	private Dictionary<string, FieldInfo> BuildFieldMap()
	{
		Dictionary<string, FieldInfo> map = new(StringComparer.Ordinal);
		foreach (FieldInfo field in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
			map[field.Name] = field;
		return map;
	}

	private static object? ConvertForField(Type type, object? value)
	{
		if (type == typeof(int))
			return ExpressionRuntime.ToInt(value);
		if (type == typeof(float))
			return ExpressionRuntime.ToFloat(value);
		if (type == typeof(bool))
			return ExpressionRuntime.ToBool(value);
		if (type == typeof(string))
			return ExpressionRuntime.ToStr(value);
		return value;
	}

	// Routes every dynamic member access through the context's own variable protocol instead of
	// the CLR members, so the arrays, the policy and the delegates stay statically-only.
	private sealed class ContextMetaObject : DynamicMetaObject
	{
		private static readonly MethodInfo _tryGetVariable = typeof(ExpressionContext).GetMethod(nameof(TryGetVariable))!;
		private static readonly MethodInfo _trySetVariable = typeof(ExpressionContext).GetMethod(nameof(TrySetVariable))!;
		private static readonly MethodInfo _tryInvokeFunction = typeof(ExpressionContext).GetMethod(nameof(TryInvokeFunction))!;
		private static readonly ConstructorInfo _missing = typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) })!;

		public ContextMetaObject(Expr parameter, ExpressionContext value)
			: base(parameter, BindingRestrictions.GetInstanceRestriction(parameter, value), value)
		{
		}

		public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
		{
			Expr self = Expr.Convert(Expression, typeof(ExpressionContext));
			ParamExpr value = Expr.Variable(typeof(object), "value");
			Expr body = Expr.Block(
				typeof(object),
				new[] { value },
				Expr.Condition(
					Expr.Call(self, _tryGetVariable, Expr.Constant(binder.Name), value),
					value,
					Throw($"Variable '{binder.Name}' does not exist.")));
			return new DynamicMetaObject(body, Restrictions);
		}

		public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
		{
			Expr self = Expr.Convert(Expression, typeof(ExpressionContext));
			Expr assigned = Expr.Convert(value.Expression, typeof(object));
			Expr body = Expr.Block(
				typeof(object),
				Expr.Condition(
					Expr.Call(self, _trySetVariable, Expr.Constant(binder.Name), assigned),
					assigned,
					Throw($"Variable '{binder.Name}' is read-only or unknown.")));
			return new DynamicMetaObject(body, Restrictions);
		}

		public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
		{
			Expr self = Expr.Convert(Expression, typeof(ExpressionContext));
			NewArrayExpr array = Expr.NewArrayInit(typeof(object), args.Select(arg => Expr.Convert(arg.Expression, typeof(object))));
			ParamExpr result = Expr.Variable(typeof(object), "result");
			Expr body = Expr.Block(
				typeof(object),
				new[] { result },
				Expr.Condition(
					Expr.Call(self, _tryInvokeFunction, Expr.Constant(binder.Name), Expr.Convert(array, typeof(IReadOnlyList<object>)), result),
					result,
					Throw($"Function '{binder.Name}' is not supported.")));
			return new DynamicMetaObject(body, Restrictions);
		}

		private static Expr Throw(string message)
			=> Expr.Throw(Expr.New(_missing, Expr.Constant(message)), typeof(object));
	}
}
