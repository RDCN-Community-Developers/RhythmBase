using RhythmBase.RhythmDoctor;
using RhythmBase.RhythmDoctor.Components;
using RhythmBase.RhythmDoctor.Components.Lang;
using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.Test;

[TestClass]
public sealed class PlayheadTests
{
	[TestMethod]
	public void AdvanceReachesTargetAndSnapshots()
	{
		Level level = Level.Default;
		level.Add(new Comment { TickTime = new TickTime(1), Text = "hi" });
		Playhead playhead = new(level);
		PlayheadAdvanceResult result = playhead.ForwardTo(new TickTime(4));
		Assert.IsTrue(result.Reached);
		PlayheadSnapshot snapshot = playhead.Snapshot();
		Assert.IsTrue(snapshot.TryGet<VariablesSnapshot>(out _));
	}

	[TestMethod]
	public void CallCustomMethodAssignsFixedSlot()
	{
		Level level = Level.Default;
		level.Add(new CallCustomMethod { TickTime = new TickTime(1), MethodName = "i0 = 3" });
		Playhead playhead = new(level);
		playhead.ForwardTo(new TickTime(3));
		Assert.IsTrue(playhead.Snapshot().TryGet<VariablesSnapshot>(out VariablesSnapshot vars));
		Assert.AreEqual(3, vars.I[0]);
	}

	[TestMethod]
	public void CloneContinuesSlotState()
	{
		Level level = Level.Default;
		level.Add(new CallCustomMethod { TickTime = new TickTime(1), MethodName = "i0 = 5" });
		Playhead playhead = new(level);
		playhead.ForwardTo(new TickTime(3));
		Playhead clone = playhead.Clone();
		Assert.IsTrue(clone.Snapshot().TryGet<VariablesSnapshot>(out VariablesSnapshot vars));
		Assert.AreEqual(5, vars.I[0]);
	}

	[TestMethod]
	public void HitVariablesRankAndPerfects()
	{
		HitVariables variables = new();
		variables.AddHit(new HitRecord { Tick = 1f, Result = HitResult.Perfect });
		variables.AddHit(new HitRecord { Tick = 2f, Result = HitResult.Perfect });
		Assert.IsTrue(variables.AtLeastNPerfects(2, 2));
		Assert.IsTrue(variables.AtLeast(ExpressionContext.RDRank.S));
		variables.RankProvider = _ => ExpressionContext.RDRank.F;
		Assert.IsFalse(variables.AtLeast(ExpressionContext.RDRank.S));
	}
}
