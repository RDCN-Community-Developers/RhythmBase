using RhythmBase.Adofai.Utils;

namespace RhythmBase.Adofai.Components;

/// <summary>
/// Represents a beat in the ADLevel.
/// </summary>
partial struct TickTime
{
	public partial float Tick
	{
		get
		{
			if ((!MustFromCache || !_isTickLoaded) && _calculator is not null)
			{
				if (_isTileLoaded && _tile is not null)
					_tick = _calculator.TileOffsetToTick(_tile.Index, _b_angleOffset) - 1f;
				else if (_isTimeSpanLoaded)
					_tick = _calculator.TimeSpanToTick(_TimeSpan) - 1f;
				_isTickLoaded = true;
			}
			return _tick + 1f;
		}
	}
	public partial TimeSpan TimeSpan
	{
		get
		{
			if ((!MustFromCache || !_isTimeSpanLoaded) && _calculator is not null)
			{
				if (_isTickLoaded)
					_TimeSpan = _calculator.TickToTimeSpan(_tick + 1f);
				_isTimeSpanLoaded = true;
			}
			return _TimeSpan;
		}
	}
	partial void InitDefault()
	{
		_tick = 1f;
		_TimeSpan = TimeSpan.Zero;
		_isTickLoaded = true;
		_isTimeSpanLoaded = true;
	}
	partial void NormalizeTick()
	{
		if (_tick < 1f) _tick = 1f;
		_tick -= 1f;
	}
	partial void NormalizeTimeSpan()
	{
		if (_TimeSpan < TimeSpan.Zero) _TimeSpan = TimeSpan.Zero;
	}
	private static partial void AddTickAndCache(TickTime left, float right, ref TickTime result)
	{
		if (!left._isTickLoaded)
			throw new ArgumentNullException(nameof(left), "The beat cannot be calculated.");
		if (left._isTickLoaded)
		{
			result._tick = left._tick + right;
			result._isTickLoaded = true;
		}
	}
	private static partial void AddTimeSpanAndCache(TickTime left, TimeSpan right, ref TickTime result)
	{
		result = new TickTime();
		if (left._isTimeSpanLoaded)
		{
			result._TimeSpan = left._TimeSpan + right;
			result._isTimeSpanLoaded = true;
			result._isTickLoaded = false;
		}
		else
			throw new ArgumentNullException(nameof(left), "The beat cannot be calculated.");
	}
	private static partial void SubstractTickAndCache(TickTime left, float right, ref TickTime result)
	{
		if (!left._isTickLoaded)
			throw new ArgumentNullException(nameof(left), "The beat cannot be calculated.");
		if (left._isTickLoaded)
		{
			result._tick = left._tick - right;
			result._isTickLoaded = true;
		}
	}
	private static partial void SubstractTimeSpanAndCache(TickTime left, TimeSpan right, ref TickTime result)
	{
		if (left._isTimeSpanLoaded)
		{
			result._TimeSpan = left._TimeSpan - right;
			result._isTimeSpanLoaded = true;
			result._isTickLoaded = false;
		}
		else
			throw new ArgumentNullException(nameof(left), "The beat cannot be calculated.");
	}
	public partial TickTime(BeatCalculator calculator, TickTime beat)
	{
		this = default;
		if (beat._isTickLoaded)
		{
			_tick = Math.Max(beat._tick, 0f);
			_isTickLoaded = true;
			_calculator = calculator;
		}
		else if (beat._isTimeSpanLoaded)
		{
			_TimeSpan = beat._TimeSpan > TimeSpan.Zero ? beat._TimeSpan : TimeSpan.Zero;
			_isTimeSpanLoaded = true;
			_calculator = calculator;
			_tick = _calculator.TimeSpanToTick(TimeSpan) - 1f;
		}
	}
	/// <summary>
	/// Creates a tile-authoritative position from a tile plus an angle offset (degrees).
	/// </summary>
	/// <param name="calculator">The owning calculator.</param>
	/// <param name="tile">The owning tile. Its current index is resolved lazily, so tile insertions or removals are picked up.</param>
	/// <param name="angleOffset">The angle offset (degrees) into the tile.</param>
	public TickTime(BeatCalculator calculator, Tile tile, float angleOffset)
	{
		this = default;
		_calculator = calculator;
		_tile = tile;
		_b_angleOffset = angleOffset;
		_isTileLoaded = true;
	}
	/// <summary>
	/// Gets the owning tile when this value is tile-authoritative; otherwise <see langword="null"/>.
	/// </summary>
	public readonly Tile? Tile => _isTileLoaded ? _tile : null;
	/// <summary>
	/// Gets the angle offset (degrees) when this value is tile-authoritative; otherwise 0.
	/// </summary>
	public readonly float AngleOffset => _isTileLoaded ? _b_angleOffset : 0f;
	public readonly partial bool IsEmpty => _calculator == null || !_isTickLoaded && !_isTimeSpanLoaded && !_isTileLoaded;
	public partial void ResetCache()
	{
		_ = Tick;
		_isTimeSpanLoaded = false;
	}
	public partial void Cache()
	{
		IfNullThrowException();
		_ = Tick;
		_ = TimeSpan;
		_ = Bpm;
	}
	internal partial void ResetBPM()
	{
		if (!_isTickLoaded)
			_tick = _calculator?.TimeSpanToTick(_TimeSpan) - 1f ?? throw new InvalidRDBeatException();
		_isTickLoaded = true;
		_isTimeSpanLoaded = false;
		_isBPMLoaded = false;
	}
	public static partial TickTime Min(TickTime left, TickTime right) =>
		left.FromSameChartOrNull(right, false) ?
			left.Tick < right.Tick ?
				left :
				right :
			left.TimeSpan < right.TimeSpan ?
				left :
				right;
	public static partial TickTime Max(TickTime left, TickTime right) =>
	left.FromSameChartOrNull(right, false) ?
		left.Tick > right.Tick ?
			left :
			right :
		left.TimeSpan > right.TimeSpan ?
			left :
			right;
	private static int CompareInternal(int bar1, float beat1, int bar2, float beat2)
	{
		int barComparison = bar1.CompareTo(bar2);
		return barComparison != 0 ? barComparison : beat1.CompareTo(beat2);
	}
	private static partial int CompareInternal(TickTime left, TickTime right)
	{
		if (left._isTickLoaded && right._isTickLoaded)
			return left._tick.CompareTo(right._tick);
		if (left._isTimeSpanLoaded && right._isTimeSpanLoaded)
			return left._TimeSpan.CompareTo(right._TimeSpan);

		if (left._calculator != null)
		{
			// 用 left 的单位比较
			left.Cache();
			return (right._isTickLoaded ? left.Tick.CompareTo(right._tick)
				: right._isTimeSpanLoaded ? left.Tick.CompareTo(left._calculator.TimeSpanToTick(right.TimeSpan))
				: throw new InvalidOperationException("The beat cannot be compared."));
		}

		if (right._calculator != null)
		{
			// 用 right 的单位比较
			right.Cache();
			return (left._isTickLoaded ? left._tick.CompareTo(right.Tick)
				: left._isTimeSpanLoaded ? right.Tick.CompareTo(right._calculator.TimeSpanToTick(left.TimeSpan))
				: throw new InvalidOperationException("The beat cannot be compared."));
		}

		throw new InvalidOperationException("The beat cannot be compared.");
	}
	public override partial string ToString()
	{
		string ToString;
		if (IsEmpty)
			ToString = $"[{(
				_isTickLoaded ? _tick.ToString() : "?"
				)},{(
				_isTimeSpanLoaded ? _TimeSpan.ToString() : "?"
				)}]";
		else
			ToString = $"[{_tick}?]";
		return ToString;
	}
	private bool _isTileLoaded;
	private Tile? _tile;
	private float _b_angleOffset;
}
