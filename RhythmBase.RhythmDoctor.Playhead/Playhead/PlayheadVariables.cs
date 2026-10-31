using System.Globalization;
using RhythmBase.Global.Components;
using RhythmBase.RhythmDoctor.Components.Lang;
using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// The variable state at a point in time: the 30 fixed slots plus the recorded custom-method calls.
/// </summary>
public readonly struct VariablesSnapshot
{
	/// <summary>Gets the integer slots <c>i0</c>鈥?c>i9</c>.</summary>
	public int[] I { get; init; }
	/// <summary>Gets the float slots <c>f0</c>鈥?c>f9</c>.</summary>
	public float[] F { get; init; }
	/// <summary>Gets the boolean slots <c>b0</c>鈥?c>b9</c>.</summary>
	public bool[] B { get; init; }
	/// <summary>Gets the custom-method expressions applied so far.</summary>
	public IReadOnlyList<string> CallCustomMethods { get; init; }
	/// <summary>Gets the custom (vfx./room) assignments recorded so far, keyed by <c>target.field</c>.</summary>
	public IReadOnlyDictionary<string, object?> Custom { get; init; }
}

/// <summary>
/// Applies <c>CallCustomMethod</c> events to the shared expression context and records them.
/// </summary>
internal sealed class VariablesModule : IPlayheadObject
{
	private readonly List<string> _calls = [];
	private readonly Dictionary<string, object?> _custom = [];
	private readonly ExpressionContext _context;

	internal VariablesModule(Playhead playhead, ExpressionContext context)
	{
		Playhead = playhead;
		_context = context;
	}

	private VariablesModule(VariablesModule source, Playhead playhead, ExpressionContext context)
	{
		Playhead = playhead;
		_context = context;
		_calls = [.. source._calls];
		_custom = new Dictionary<string, object?>(source._custom);
	}

	/// <summary>
	/// Records an assignment or increment against a custom target (<c>vfx.</c>/<c>roomN.</c>).
	/// </summary>
	internal void ApplyCustom(string target, string expression)
	{
		expression = expression.Trim();
		if (expression.EndsWith("++", StringComparison.Ordinal) || expression.EndsWith("--", StringComparison.Ordinal))
		{
			bool increment = expression.EndsWith("++", StringComparison.Ordinal);
			string name = expression.Substring(0, expression.Length - 2).Trim();
			string key = target + "." + name;
			double current = _custom.TryGetValue(key, out object? existing) && existing is IConvertible convertible
				? convertible.ToDouble(CultureInfo.InvariantCulture)
				: 0d;
			_custom[key] = current + (increment ? 1d : -1d);
			return;
		}
		int equals = expression.IndexOf('=');
		if (equals > 0 && expression[equals - 1] != '=' && (equals + 1 >= expression.Length || expression[equals + 1] != '='))
		{
			string name = expression.Substring(0, equals).Trim();
			string right = expression.Substring(equals + 1).Trim();
			object? value;
			try
			{
				value = Playhead.EvaluateCode(right);
			}
			catch (Exception)
			{
				value = null;
			}
			_custom[target + "." + name] = value ?? right;
			return;
		}
		_custom[target + "." + expression] = true;
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Variables;

	public string Id => "variables";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Variables;

	public IReadOnlyList<PlayheadObjectSnapshot> Describe() => [new PlayheadObjectSnapshot { Id = Id, Kind = Kind, State = Snapshot() }];

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } = [EventType.CallCustomMethod];

	public void MoveNext(IBaseEvent e)
	{
		if (e is CallCustomMethod call && !string.IsNullOrEmpty(call.MethodName))
			_calls.Add(call.MethodName);
	}

	public object Snapshot() => new VariablesSnapshot
	{
		I = (int[])_context.I.Clone(),
		F = (float[])_context.F.Clone(),
		B = (bool[])_context.B.Clone(),
		CallCustomMethods = _calls,
		Custom = new Dictionary<string, object?>(_custom),
	};

	public IPlayheadObject Clone(Playhead playhead) => new VariablesModule(this, playhead, playhead.ExpressionContext);
}


