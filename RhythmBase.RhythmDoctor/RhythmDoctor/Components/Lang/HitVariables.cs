using System;
using System.Collections.Generic;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// A ready-made sandbox environment backed by a hit timeline. It implements the simulation
/// functions a level needs from hits and forwards variable access to an optional inner environment.
/// </summary>
/// <remarks>
/// Add <see cref="HitRecord"/>s before creating the playhead; the playhead wires
/// <see cref="TickProvider"/> so <c>atLeastNPerfects</c> / <c>atLeastRank</c> are evaluated up to
/// the current time. It also shares its timeline with <c>LastHit</c> conditions when no separate
/// <see cref="PlayheadConfig.HitTimeline"/> is given.
/// </remarks>
public sealed class HitVariables : ExpressionContext, IRankSimulation, IPerfectsSimulation
{
	private readonly ExpressionContext? _inner;
	private readonly IRandomSimulator _random;
	private readonly List<HitRecord> _hits = [];

	/// <summary>Initializes a new environment.</summary>
	/// <param name="inner">An optional environment to forward variable access to.</param>
	/// <param name="random">An optional random source.</param>
	public HitVariables(ExpressionContext? inner = null, IRandomSimulator? random = null)
	{
		_inner = inner;
		_random = random ?? new RandomSimulator();
		Rand = NextRandom;
		RankSimulation = this;
		PerfectsSimulation = this;
	}

	/// <summary>Gets or sets an optional rank provider given the current time.</summary>
	public Func<float, ExpressionContext.RDRank>? RankProvider { get; set; }

	/// <summary>Gets the hit timeline.</summary>
	public IReadOnlyList<HitRecord> Hits => _hits;

	internal Func<float>? TickProvider { get; set; }

	/// <inheritdoc/>
	public ExpressionContext.RDRank RankCap { get; set; } = ExpressionContext.RDRank.S;

	/// <inheritdoc/>
	public float SuccessRate { get; set; } = 0.9f;

	/// <summary>Adds a hit to the timeline.</summary>
	/// <param name="hit">The hit.</param>
	public void AddHit(HitRecord hit)
	{
		_hits.Add(hit);
		_hits.Sort(static (a, b) => a.Tick.CompareTo(b.Tick));
	}

	/// <inheritdoc/>
	protected override bool TryGetMemberValue(string name, out object? value)
	{
		if (_inner is not null)
			return _inner.TryGetVariable(name, out value);
		value = null;
		return false;
	}

	/// <inheritdoc/>
	protected override bool TrySetMemberValue(string name, object? value) => _inner is not null && _inner.TrySetVariable(name, value);

	/// <inheritdoc/>
	public bool AtLeastNPerfects(int hitsToCheck, int numberOfPerfects)
	{
		float tick = TickProvider?.Invoke() ?? float.PositiveInfinity;
		int perfect = 0;
		int considered = 0;
		for (int i = _hits.Count - 1; i >= 0 && considered < hitsToCheck; i--)
		{
			if (_hits[i].Tick > tick)
				continue;
			considered++;
			if (_hits[i].Result == HitResult.Perfect)
				perfect++;
		}
		return perfect >= numberOfPerfects;
	}

	/// <inheritdoc/>
	public bool AtLeast(ExpressionContext.RDRank rank)
	{
		float tick = TickProvider?.Invoke() ?? float.PositiveInfinity;
		ExpressionContext.RDRank current = RankProvider?.Invoke(tick) ?? RankFromMistakes(CountMistakes(tick));
		// RDRank numbers each grade as minus(3k), base(3k+1), plus(3k+2); compare on the base grade.
		int basis = (int)rank - ((int)rank % 3) + 1;
		return (int)current <= basis;
	}

	private int NextRandom(int value) => _inner is not null
		? _inner.Rand(value)
		: value > 0 ? _random.Next(1, value + 1) : value < 0 ? -_random.Next(1, -value + 1) : 0;

	private int CountMistakes(float tick)
	{
		int mistakes = 0;
		foreach (HitRecord hit in _hits)
		{
			if (hit.Tick > tick)
				break;
			if (hit.Result != HitResult.Perfect)
				mistakes++;
		}
		return mistakes;
	}

	private static ExpressionContext.RDRank RankFromMistakes(int mistakes) => mistakes switch
	{
		<= 0 => ExpressionContext.RDRank.S,
		<= 2 => ExpressionContext.RDRank.A,
		<= 5 => ExpressionContext.RDRank.B,
		<= 10 => ExpressionContext.RDRank.C,
		<= 20 => ExpressionContext.RDRank.D,
		_ => ExpressionContext.RDRank.F,
	};
}
