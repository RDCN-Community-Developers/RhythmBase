using RhythmBase.RhythmDoctor.Components.Conditions;
using RhythmBase.RhythmDoctor.Components.Lang;
using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Decides whether an event runs, mirroring the game's condition gating:
/// the event must be <see cref="IBaseEvent.Active"/>, every referenced conditional must match,
/// and the global tags <c>p</c> (2P), <c>f</c> (flashy), <c>n</c> (narration) and <c>o</c> (once)
/// must match.
/// </summary>
internal sealed class ConditionEvaluator
{
	private readonly Chart _chart;
	private readonly PlayheadConfig _config;
	private readonly ConditionalExpressionEvaluator _custom;
	private readonly Func<sbyte, HitResult?>? _lastHit;

	internal ConditionEvaluator(Chart chart, PlayheadConfig config, ExpressionContext context, Func<sbyte, HitResult?>? lastHit)
	{
		_chart = chart;
		_config = config;
		_custom = new ConditionalExpressionEvaluator(context);
		_lastHit = lastHit;
	}

	/// <summary>
	/// Gets whether the event runs, given how many times it has run before.
	/// </summary>
	/// <param name="e">The event.</param>
	/// <param name="timesRun">How many times the event has already run.</param>
	/// <param name="viaTag">Whether the event was triggered by a <c>TagAction</c> rather than its own tick.</param>
	internal bool ShouldRun(IBaseEvent e, int timesRun, bool viaTag = false)
	{
		if (!e.Active)
			return false;
		// A tagged event only runs on its own tick when it is also allowed to run normally.
		if (!viaTag && !string.IsNullOrEmpty(e.Tag) && !e.RunTag)
			return false;
		if (e.Condition.IsEmpty)
			return true;

		foreach (BaseConditional conditional in _chart.Conditionals)
		{
			bool? expected = e.Condition[conditional];
			if (expected is null)
				continue;
			if (Evaluate(conditional, timesRun) != expected.Value)
				return false;
		}

		bool? twoPlayer = e.Condition['p'];
		if (twoPlayer is not null && twoPlayer.Value != _config.TwoPlayerMode)
			return false;
		bool? flashy = e.Condition['f'];
		if (flashy is not null && flashy.Value != _config.Flashy)
			return false;
		bool? narration = e.Condition['n'];
		if (narration is not null && narration.Value != _config.NarrationEnabled)
			return false;
		bool? once = e.Condition['o'];
		if (once is not null && once.Value != (timesRun == 0))
			return false;
		return true;
	}

	private bool Evaluate(BaseConditional conditional, int timesRun)
	{
		switch (conditional)
		{
			case TimesExecutedCondition times:
				return timesRun < times.MaxTimes;
			case CustomCondition custom:
				return EvaluateCustom(custom, timesRun);
			case PlayerModeCondition playerMode:
				return playerMode.TwoPlayerMode == _config.TwoPlayerMode;
			case LanguageCondition language:
				return _config.Language == language.TargetLanguage;
			case NarrationCondition narration:
				return narration.NarrationEnabled == _config.NarrationEnabled;
			case AccessibilityCondition accessibility:
				return _config.AccessibilityProvider?.Invoke(accessibility.TargetEffectType)
					?? (accessibility.TargetEffectType == EffectType.Narration ? _config.NarrationEnabled : _config.Flashy);
			case LastHitCondition lastHit:
				return _lastHit is not null && _lastHit(lastHit.Row) == lastHit.Result;
			default:
				return false;
		}
	}

	private bool EvaluateCustom(CustomCondition condition, int timesRun)
	{
		string expression = condition.Expression.Trim();
		if (expression == "neverRun")
			return timesRun == 0;
		try
		{
			return _custom.Check(expression);
		}
		catch (Exception)
		{
			return false;
		}
	}
}

