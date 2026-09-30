using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Events;
namespace RhythmBase.Adofai.Utils;

/// <summary>
/// Beat Calculator.
/// </summary>
/// <remarks>
/// The Adofai timeline is treated as a monotonic map from a cumulative beat coordinate
/// (1 beat = 180°, i.e. π radians) to absolute time. Each floor contributes a fixed beat length
/// and a constant effective BPM (= level BPM × the floor speed multiplier), so the whole timeline
/// can be cached as a piecewise-linear <see cref="BpmCache"/> list, exactly like the RhythmDoctor side.
/// This mirrors the game's <c>scrLevelMaker.CalculateFloorEntryTimes</c>.
/// </remarks>
public partial class BeatCalculator
{
	private const double TwoPi = Math.PI * 2;
	private const double StartEntryAngle = Math.PI * 1.5;
	private const double AngleEpsilon = 1e-6;

	private FloorTime[] _floors = [];
	private bool _dirty = true;
	private bool _refreshing;

	/// <summary>Marks the cached floor table as stale so the next query rebuilds it.</summary>
	internal void MarkDirty() => _dirty = true;

	/// <summary>
	/// Precomputed timing for a single floor, produced by <see cref="Refresh"/>.
	/// </summary>
	internal readonly struct FloorTime
	{
		/// <summary>Absolute tick (1-based) at this floor's entry. 1 beat = 180°.</summary>
		public readonly float StartTick;
		/// <summary>Beat length of this floor, including hold lengths and extra beats.</summary>
		public readonly float BeatLength;
		/// <summary>Effective speed multiplier applied to the level BPM on this floor.</summary>
		public readonly float Speed;
		/// <summary>Absolute time at this floor's entry.</summary>
		public readonly TimeSpan EntryTime;
		/// <summary>Effective BPM on this floor (<c>levelBpm * Speed</c>).</summary>
		public readonly float EffectiveBpm;
		/// <summary>Entry angle in radians.</summary>
		public readonly double EntryAngle;
		/// <summary>Exit angle in radians.</summary>
		public readonly double ExitAngle;
		/// <summary>Whether this floor rotates counter-clockwise.</summary>
		public readonly bool IsCCW;
		/// <summary>Number of planets active on this floor.</summary>
		public readonly int Planets;
		/// <summary>Hold length (beats), or -1 when absent.</summary>
		public readonly int HoldLength;
		/// <summary>Extra beats added by Pause / FreeRoam.</summary>
		public readonly double ExtraBeats;
		/// <summary>Whether this floor is a mid-spin.</summary>
		public readonly bool MidSpin;
		/// <summary>Whether this floor is a full turnaround (360°).</summary>
		public readonly bool Turnaround;

		internal FloorTime(float startTick, float beatLength, float speed, float effectiveBpm, TimeSpan entryTime,
			double entryAngle, double exitAngle, bool isCCW, int planets, int holdLength,
			double extraBeats, bool midSpin, bool turnaround)
		{
			StartTick = startTick;
			BeatLength = beatLength;
			Speed = speed;
			EffectiveBpm = effectiveBpm;
			EntryTime = entryTime;
			EntryAngle = entryAngle;
			ExitAngle = exitAngle;
			IsCCW = isCCW;
			Planets = planets;
			HoldLength = holdLength;
			ExtraBeats = extraBeats;
			MidSpin = midSpin;
			Turnaround = turnaround;
		}
	}

	/// <summary>Gets the number of cached floors (including the end tile).</summary>
	public int FloorCount
	{
		get
		{
			EnsureRefreshed();
			return _floors.Length;
		}
	}

	/// <summary>Gets the cached timing of a floor by index.</summary>
	internal FloorTime FloorAt(int index)
	{
		EnsureRefreshed();
		return _floors[index];
	}

	/// <summary>
	/// Converts a floor + angle offset (degrees into the floor) to an absolute tick.
	/// </summary>
	public float TileOffsetToTick(int floor, float angleOffset)
	{
		EnsureRefreshed();
		if (_floors.Length == 0)
			return 1f;
		floor = Math.Max(0, Math.Min(_floors.Length - 1, floor));
		return _floors[floor].StartTick + angleOffset / 180f;
	}

	/// <summary>
	/// Converts an absolute tick to the floor it belongs to and the angle offset (degrees) into that floor.
	/// </summary>
	public (int Floor, float AngleOffset) TickToTileOffset(float tick)
	{
		EnsureRefreshed();
		return TickToTileOffsetCore(_floors, tick);
	}

	/// <summary>
	/// Creates a tile-authoritative <see cref="TickTime"/> from a tile plus an angle offset (degrees).
	/// </summary>
	/// <param name="tile">The owning tile.</param>
	/// <param name="angleOffset">The angle offset (degrees) into the tile.</param>
	/// <returns>An <see cref="TickTime"/> bound to this calculator.</returns>
	public TickTime TickOf(Tile tile, float angleOffset = 0f) => new(this, tile, angleOffset);

	private static (int Floor, float AngleOffset) TickToTileOffsetCore(FloorTime[] floors, float tick)
	{
		if (floors.Length == 0)
			return (0, 0f);
		if (tick <= floors[0].StartTick)
			return (0, (tick - floors[0].StartTick) * 180f);
		for (int i = 0; i < floors.Length; i++)
		{
			float end = floors[i].StartTick + floors[i].BeatLength;
			if (tick < end - TickEpsilon || i == floors.Length - 1)
				return (i, (tick - floors[i].StartTick) * 180f);
		}
		int last = floors.Length - 1;
		return (last, (tick - floors[last].StartTick) * 180f);
	}

	private void EnsureRefreshed()
	{
		if (_refreshing)
			return;
		if (_dirty || _floors.Length == 0)
			Refresh();
	}

	partial void EnsureCache() => EnsureRefreshed();

	/// <summary>
	/// Rebuilds the per-floor timing table and the tick/time cache from the current level tiles and events.
	/// </summary>
	public partial void Refresh()
	{
		if (_refreshing)
			return;
		_refreshing = true;
		try
		{
			RefreshCore();
			_dirty = false; // allow tile-unit ticks to be resolved below without re-entering Refresh
			bool changed = false;
			foreach (BaseEvent free in Collection.FreeEvents)
			{
				if (free is not SetSpeed setSpeed || setSpeed._floor >= 0)
					continue;
				setSpeed._floor = TickToTileOffsetCore(_floors, setSpeed.TickTime.Tick).Floor;
				changed = true;
			}
			if (changed)
				RefreshCore();
			InvalidateAll();
		}
		finally
		{
			_refreshing = false;
		}
	}

	private void RefreshCore()
	{
		if (Collection.Settings is null)
			return;
		float levelBpm = Collection.Settings.Bpm;
		if (levelBpm <= 0f)
			levelBpm = Constants.DefaultBpm;

		int count = Collection.Count + 1; // real floors plus the end tile, mirroring listFloors
		if (count <= 0)
		{
			_floors = [];
			_bpmCache = [];
			return;
		}

		// Group time-authoritative SetSpeed events by their target floor.
		Dictionary<int, List<SetSpeed>> freeSetSpeeds = [];
		foreach (BaseEvent free in Collection.FreeEvents)
		{
			if (free is not SetSpeed setSpeed)
				continue;
			int floor = Math.Max(0, Math.Min(count - 1, setSpeed._floor));
			if (!freeSetSpeeds.TryGetValue(floor, out List<SetSpeed>? list))
				freeSetSpeeds[floor] = list = [];
			list.Add(setSpeed);
		}

		bool[] midSpin = new bool[count];
		double[] entryAngle = new double[count];
		double[] exitAngle = new double[count];
		bool[] isCCW = new bool[count];
		int[] planets = new int[count];
		int[] holdLength = new int[count];
		double[] extraBeats = new double[count];
		float[] speed = new float[count];
		double[] angleLength = new double[count];
		bool[] turnaround = new bool[count];

		// Resolve geometry (entry/exit angles), mirroring scrLevelMaker.InstantiateFloatFloors.
		entryAngle[0] = StartEntryAngle;
		Array.Fill(holdLength, -1);
		for (int i = 0; i < count; i++)
		{
			Tile tile = Collection[i];
			midSpin[i] = i < Collection.Count && tile.IsMidSpin;
			if (i > 0)
				entryAngle[i] = Mod(exitAngle[i - 1] + Math.PI, TwoPi);
			exitAngle[i] = midSpin[i] ? entryAngle[i] : (90f - tile.Angle) * Math.PI / 180.0;
		}
		exitAngle[count - 1] = entryAngle[count - 1] + Math.PI;

		// Pass 1: per-floor timing attributes (Twirl / Hold / MultiPlanet / SetSpeed), mirroring ApplyCoreEventsToFloors.
		bool dirFlag = false;
		float speedAccum = 1f;
		int planetAccum = 2;
		for (int i = 0; i < count; i++)
		{
			List<SetSpeed> setSpeeds = [];
			foreach (IBaseEvent item in Collection[i])
			{
				switch (item)
				{
					case Twirl:
						dirFlag = !dirFlag;
						break;
					case Hold hold when i < count - 1 && hold.Duration >= 0:
						holdLength[i] = hold.Duration;
						break;
					case MultiPlanet mp:
						planetAccum = Math.Max(2, Math.Min(3, (int)mp.Planets + 2));
						if (i > 0 && midSpin[i - 1])
							planets[i - 1] = planetAccum;
						break;
				}
			}
			if (freeSetSpeeds.TryGetValue(i, out List<SetSpeed>? freeList))
				setSpeeds.AddRange(freeList);
			isCCW[i] = dirFlag;
			speed[i] = speedAccum;
			planets[i] = planetAccum;
			if (setSpeeds.Count > 0)
			{
				ApplySetSpeed(setSpeeds, entryAngle[i], exitAngle[i], isCCW[i], levelBpm, ref speedAccum, out float tileSpeed);
				speed[i] = tileSpeed;
			}
		}

		// Pass 2: angle lengths (depends on geometry, spin, planets and hold).
		for (int i = 0; i < count; i++)
		{
			angleLength[i] = CalculateFloorAngleLength(i, count, entryAngle, exitAngle, isCCW, planets, holdLength, midSpin, out turnaround[i]);
		}

		// Pass 3: extra beats from Pause / FreeRoam (FreeRoam depends on the resolved angle length).
		for (int i = 0; i < count; i++)
		{
			foreach (IBaseEvent item in Collection[i])
			{
				switch (item)
				{
					case Pause pause when i < count - 1 && !midSpin[i]:
						extraBeats[i] += pause.Duration;
						break;
					case FreeRoam roam when i < count - 1 && roam.Duration >= 2:
						double tileBeats = angleLength[i] / Math.PI;
						extraBeats[i] += Math.Max(tileBeats, roam.Duration - tileBeats);
						break;
				}
			}
		}

		// Pass 4: absolute times and the tick/time cache, mirroring CalculateFloorEntryTimes.
		TimeSpan[] entryTime = new TimeSpan[count];
		float[] startTick = new float[count];
		double crotchet = 60.0 / levelBpm;
		double adjustedCountdown = Collection.Settings.CountdownTicks;
		startTick[0] = 1f;
		for (int i = 1; i < count; i++)
			startTick[i] = startTick[i - 1] + (float)(angleLength[i - 1] / Math.PI + extraBeats[i - 1]);

		if (count > 1)
		{
			double num = crotchet * (adjustedCountdown - 1)
				+ GetTimeBetweenAngles(entryAngle[0], exitAngle[0], speed[0], levelBpm, !isCCW[0]);
			entryTime[1] = TimeSpan.FromSeconds(num);
			for (int i = 1; i < count - 1; i++)
			{
				double num4 = GetTimeBetweenAngles(
					entryAngle[i] + GetAngleOffset(i, entryAngle, exitAngle, isCCW, planets, midSpin),
					exitAngle[i] + (midSpin[i] ? GetAngleOffset(i, entryAngle, exitAngle, isCCW, planets, midSpin) : 0.0),
					speed[i], levelBpm, !isCCW[i]);
				double timeBetweenAngles = GetTimeBetweenAngles(0.0, Math.PI, speed[i], levelBpm, isCW: false);
				double angleMoved = GetAngleMoved(
					entryAngle[i] + GetAngleOffset(i, entryAngle, exitAngle, isCCW, planets, midSpin),
					exitAngle[i] + (midSpin[i] ? GetAngleOffset(i, entryAngle, exitAngle, isCCW, planets, midSpin) : 0.0),
					!isCCW[i]);
				if (angleMoved <= AngleEpsilon || angleMoved >= TwoPi - AngleEpsilon)
					num4 = midSpin[i] ? 0.0 : 2.0 * timeBetweenAngles;
				num += num4;
				if (holdLength[i] > 0)
					num += holdLength[i] * 2.0 * timeBetweenAngles;
				num += extraBeats[i] * timeBetweenAngles;
				entryTime[i + 1] = TimeSpan.FromSeconds(num);
			}
		}

		FloorTime[] floors = new FloorTime[count];
		BpmCache[] bpmCache = new BpmCache[count];
		for (int i = 0; i < count; i++)
		{
			float effectiveBpm = levelBpm * speed[i];
			floors[i] = new FloorTime(startTick[i], (float)(angleLength[i] / Math.PI + extraBeats[i]), speed[i], effectiveBpm,
				entryTime[i], entryAngle[i], exitAngle[i], isCCW[i], planets[i], holdLength[i],
				extraBeats[i], midSpin[i], turnaround[i]);
			bpmCache[i] = new BpmCache(startTick[i], entryTime[i], effectiveBpm);
		}
		_floors = floors;
		_bpmCache = bpmCache;
	}

	private static double CalculateFloorAngleLength(int i, int count, double[] entryAngle, double[] exitAngle,
		bool[] isCCW, int[] planets, int[] holdLength, bool[] midSpin, out bool turnaround)
	{
		int prevIndex = Math.Max(0, i - 1);
		double dir = isCCW[i] ? -1.0 : 1.0;
		double num = GetInverseAnglePerBeatMultiplanet(planets[i]) * dir;
		if (midSpin[i])
			num = 0.0;
		if (midSpin[prevIndex] && planets[i] > 2)
			num -= (TwoPi + GetInverseAnglePerBeatMultiplanet(planets[i])) * dir;
		double moved = GetAngleMoved(entryAngle[i] + num, exitAngle[i] + (midSpin[i] ? num : 0.0), !isCCW[i]);
		double magnitude = Math.Abs(moved);
		if (magnitude <= AngleEpsilon || magnitude >= TwoPi - AngleEpsilon)
		{
			if (midSpin[i])
				moved = 0.0;
			else
				moved = TwoPi;
			turnaround = !midSpin[i];
		}
		else
		{
			turnaround = false;
		}
		if (holdLength[i] > 0)
			moved += holdLength[i] * 2.0 * Math.PI;
		return moved;
	}

	private static double GetAngleOffset(int i, double[] entryAngle, double[] exitAngle, bool[] isCCW, int[] planets, bool[] midSpin)
	{
		int prevIndex = Math.Max(0, i - 1);
		double dir = isCCW[i] ? -1.0 : 1.0;
		double num = GetInverseAnglePerBeatMultiplanet(planets[i]) * dir;
		if (midSpin[i])
			num = 0.0;
		if (midSpin[prevIndex] && planets[i] > 2)
			num -= (TwoPi + GetInverseAnglePerBeatMultiplanet(planets[i])) * dir;
		return num;
	}

	private void ApplySetSpeed(List<SetSpeed> setSpeeds, double entryAngle, double exitAngle, bool isCCW,
		float levelBpm, ref float speedAccum, out float tileSpeed)
	{
		float angleDegrees = ApproximatelyFloor(entryAngle, exitAngle)
			? 360f
			: (float)(GetAngleMoved(entryAngle, exitAngle, !isCCW) * 180.0 / Math.PI);
		List<float> offsets = [.. setSpeeds.Select(s => s.AngleOffset).Distinct().OrderBy(o => o)];
		float totalTime = offsets[0] / 180f * (60f / (levelBpm * speedAccum));
		bool hasOffsetInside = false;
		foreach (float offset in offsets)
		{
			SetSpeed last = setSpeeds.Last(s => s.AngleOffset == offset);
			float nextOffset = offsets.FirstOrDefault(o => o > offset);
			if (nextOffset == 0f)
				nextOffset = angleDegrees;
			float newBpm;
			if (last.SpeedType == SpeedType.Bpm)
			{
				newBpm = last.BeatsPerMinute;
			}
			else
			{
				newBpm = 0f;
				List<SetSpeed> same = setSpeeds.Where(s => s.AngleOffset == offset).ToList();
				for (int k = 0; k < same.Count; k++)
				{
					if (same[k].SpeedType == SpeedType.Bpm)
						newBpm = same[k].BeatsPerMinute;
					else
						newBpm = (k <= 0 ? levelBpm * speedAccum : newBpm) * same[k].BpmMultiplier;
				}
			}
			totalTime += (nextOffset - offset) / 180f * (60f / newBpm);
			if (offset > 0f && offset <= angleDegrees)
				hasOffsetInside = true;
			speedAccum = newBpm / levelBpm;
		}
		tileSpeed = hasOffsetInside ? 60f / totalTime * (angleDegrees / 180f) / levelBpm : speedAccum;
	}

	private static bool ApproximatelyFloor(double a, double b) => Math.Abs(a - b) < 0.001;

	private static double Mod(double x, double m) => (x % m + m) % m;

	private static double GetAngleMoved(double entryAngle, double exitAngle, bool isCW)
		=> Mod((exitAngle - entryAngle) * (isCW ? 1.0 : -1.0), TwoPi);

	private static double GetTimeBetweenAngles(double entryAngle, double exitAngle, double speed, double bpm, bool isCW)
	{
		if (speed <= 0.0 || bpm <= 0.0)
			return 0.0;
		return Mod((exitAngle - entryAngle) * (isCW ? 1.0 : -1.0), TwoPi) / Math.PI * (60.0 / bpm / speed);
	}

	private static double GetInverseAnglePerBeatMultiplanet(double planets) => Math.PI * (planets - 2.0) / planets;
}
