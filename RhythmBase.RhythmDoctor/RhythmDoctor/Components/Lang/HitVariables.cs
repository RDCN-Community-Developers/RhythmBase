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
public sealed class HitVariables : VariablesBase
{
	private readonly VariablesBase? _inner;
	private readonly IRandomSimulator _random;
	private readonly List<HitRecord> _hits = [];

	/// <summary>Initializes a new environment.</summary>
	/// <param name="inner">An optional environment to forward variable access to.</param>
	/// <param name="random">An optional random source.</param>
	public HitVariables(VariablesBase? inner = null, IRandomSimulator? random = null)
	{
		_inner = inner;
		_random = random ?? new RandomSimulator();
	}

	/// <summary>Gets or sets an optional rank provider given the current time.</summary>
	public Func<float, Rank>? RankProvider { get; set; }

	/// <summary>Gets the hit timeline.</summary>
	public IReadOnlyList<HitRecord> Hits => _hits;

	internal Func<float>? TickProvider { get; set; }

	/// <summary>Adds a hit to the timeline.</summary>
	/// <param name="hit">The hit.</param>
	public void AddHit(HitRecord hit)
	{
		_hits.Add(hit);
		_hits.Sort(static (a, b) => a.Tick.CompareTo(b.Tick));
	}

	/// <inheritdoc/>
	public override bool TryGetVariable(string name, out object? value)
	{
		if (_inner is not null)
			return _inner.TryGetVariable(name, out value);
		value = null;
		return false;
	}

	/// <inheritdoc/>
	public override bool TrySetVariable(string name, object? value) => _inner is not null && _inner.TrySetVariable(name, value);

	/// <inheritdoc/>
	public override int Rand(int value) => _inner?.Rand(value) ?? (value > 0 ? _random.Next(1, value + 1) : value < 0 ? -_random.Next(1, -value + 1) : 0);

	/// <inheritdoc/>
	public override bool atLeastNPerfects(int hitsToCheck, int numberOfPerfects)
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
	public override bool atLeastRank(Rank rank)
	{
		float tick = TickProvider?.Invoke() ?? float.PositiveInfinity;
		Rank current = RankProvider?.Invoke(tick) ?? RankFromMistakes(CountMistakes(tick));
		Rank basis = rank & ~(Rank.Plus | Rank.Minus);
		return (int)current <= (int)basis;
	}

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

	private static Rank RankFromMistakes(int mistakes) => mistakes switch
	{
		<= 0 => Rank.S,
		<= 2 => Rank.A,
		<= 5 => Rank.B,
		<= 10 => Rank.C,
		<= 20 => Rank.D,
		_ => Rank.F,
	};
}
