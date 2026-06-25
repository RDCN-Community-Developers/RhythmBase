using RhythmBase.RhythmDoctor.Utils;
namespace RhythmBase.RhythmDoctor.Components;

partial struct TickTime
{
	public partial float Tick
	{
		get
		{
			if ((!MustFromCache || !_isTickLoaded) && _calculator is not null)
			{
				if (_isBarBeatLoaded)
					_tick = _calculator.BarBeatToTick(_b_bar, _b_beat) - 1f;
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
				else
				{
					if (_isBarBeatLoaded)
					{
						_tick = _calculator.BarBeatToTick(_b_bar, _b_beat) - 1f;
						_isTickLoaded = true;
						_TimeSpan = _calculator.TickToTimeSpan(_tick + 1f);
					}
				}
				_isTimeSpanLoaded = true;
			}
			return _TimeSpan;
		}
	}
	partial void InitDefault()
	{
		_tick = 1f;
		(_b_bar, _b_beat) = (1, 1f);
		_TimeSpan = TimeSpan.Zero;
		_isTickLoaded = true;
		_isBarBeatLoaded = true;
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
		if (!left._isTickLoaded && !left._isBarBeatLoaded)
			throw new ArgumentNullException(nameof(left), "The beat cannot be calculated.");
		if (left._isTickLoaded)
		{
			result._tick = left._tick + right;
			result._isTickLoaded = true;
			result._isBarBeatLoaded = false;
		}
		else if (left._isBarBeatLoaded)
		{
			(result._b_bar, result._b_beat) = (left._b_bar, left._b_beat + right);
			result._isBarBeatLoaded = true;
			result._isTickLoaded = false;
		}
	}
	private static partial void AddTimeSpanAndCache(TickTime left, TimeSpan right, ref TickTime result)
	{
		result = new TickTime();
		if (left._isTimeSpanLoaded)
		{
			result._TimeSpan = left._TimeSpan + right;
			result._isTimeSpanLoaded = true;
			result._isTickLoaded = result._isBarBeatLoaded = false;
		}
		else
			throw new ArgumentNullException(nameof(left), "The beat cannot be calculated.");
	}
	private static partial void SubstractTickAndCache(TickTime left, float right, ref TickTime result)
	{
		if (!left._isTickLoaded && !left._isBarBeatLoaded)
			throw new ArgumentNullException(nameof(left), "The beat cannot be calculated.");
		if (left._isTickLoaded)
		{
			result._tick = left._tick - right;
			result._isTickLoaded = true;
			result._isBarBeatLoaded = false;
		}
		if (left._isBarBeatLoaded)
		{
			(result._b_bar, result._b_beat) = (left._b_bar, left._b_beat - right);
			result._isBarBeatLoaded = true;
			result._isTickLoaded = false;
		}
	}
	private static partial void SubstractTimeSpanAndCache(TickTime left, TimeSpan right, ref TickTime result)
	{
		if (left._isTimeSpanLoaded)
		{
			result._TimeSpan = left._TimeSpan - right;
			result._isTimeSpanLoaded = true;
			result._isTickLoaded = result._isBarBeatLoaded = false;
		}
		else
			throw new ArgumentNullException(nameof(left), "The beat cannot be calculated.");
	}
	/// <summary>
	/// The number of beats per bar followed at this moment.
	/// </summary>
	public int Cpb
	{
		get
		{
			if (!_isCPBLoaded)
			{
				_CPB = _calculator?.CrotchetsPerBarOf(this) ?? 0;
				_isCPBLoaded = true;
			}
			return _CPB;
		}
	}
	/// <summary>
	/// Constructs an instance of RDBeat with the specified bar and beat.
	/// </summary>
	/// <param name="bar">The actual bar of this moment. Must be greater than or equal to 1.</param>
	/// <param name="beat">The actual beat of this moment. Must be greater than or equal to 1.</param>
	public TickTime(int bar, float beat)
	{
		this = default;
		if (bar < 1)
			bar = 1;
		if (beat < 1)
			beat = 1;
		(_b_bar, _b_beat) = (bar, beat);
		_isBarBeatLoaded = true;
	}
	/// <summary>
	/// Construct an instance with specifying a calculator.
	/// </summary>
	/// <param name="calculator">Specified calculator.</param>
	/// <param name="bar">The actual bar of this moment.</param>
	/// <param name="beat">The actual beat of this moment.</param>
	public TickTime(BeatCalculator calculator, int bar, float beat)
	{
		this = new TickTime(bar, beat);
		_calculator = calculator;
		_tick = _calculator.BarBeatToTick(bar, beat) - 1f;
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
		else if (beat._isBarBeatLoaded)
		{
			(_b_bar, _b_beat) = (Math.Max(beat._b_bar, 1), Math.Max(beat._b_beat, 1));
			_isBarBeatLoaded = true;
			_calculator = calculator;
			_tick = _calculator.BarBeatToTick(beat._b_bar, beat._b_beat) - 1f;
		}
		else if (beat._isTimeSpanLoaded)
		{
			_TimeSpan = beat._TimeSpan > TimeSpan.Zero ? beat._TimeSpan : TimeSpan.Zero;
			_isTimeSpanLoaded = true;
			_calculator = calculator;
			_tick = _calculator.TimeSpanToTick(TimeSpan) - 1f;
		}
	}
	public readonly partial bool IsEmpty => _calculator == null || !_isTickLoaded && !_isBarBeatLoaded && !_isTimeSpanLoaded;
	public partial void ResetCache()
	{
		_ = Tick;
		_isBarBeatLoaded = false;
		_isTimeSpanLoaded = false;
	}
	public partial void Cache()
	{
		IfNullThrowException();
		_ = Tick;
		Deconstruct(out _, out _);
		_ = TimeSpan;
		_ = Bpm;
		_ = Cpb;
	}
	internal partial void ResetBPM()
	{
		if (!_isTickLoaded)
			_tick = _calculator?.TimeSpanToTick(_TimeSpan) - 1f ?? throw new InvalidRDBeatException();
		_isTickLoaded = true;
		_isTimeSpanLoaded = false;
		_isBPMLoaded = false;
	}
	internal void ResetCPB()
	{
		if (!_isTickLoaded)
			_tick = _calculator?.BarBeatToTick(_b_bar, _b_beat) - 1f ?? throw new InvalidRDBeatException();
		_isTickLoaded = true;
		_isBarBeatLoaded = false;
		_isCPBLoaded = false;
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
	/// <summary>
	/// Deconstructs the current instance into its bar and beat components.
	/// </summary>
	/// <remarks>This method calculates the bar and beat values if they are not already loaded.  The calculation
	/// depends on the internal state of the instance and may involve  converting other loaded values such as time span or
	/// beat-only values.</remarks>
	/// <param name="Bar">The bar component of the instance.</param>
	/// <param name="Beat">The beat component of the instance.</param>
	public void Deconstruct(out int Bar, out float Beat)
	{
		if (!_isBarBeatLoaded && _calculator is not null)
		{
			if (_isTickLoaded)
				(_b_bar, _b_beat) = _calculator.TickToBarBeat(_tick + 1f);
			else if (_isTimeSpanLoaded)
			{
				_tick = _calculator.TimeSpanToTick(_TimeSpan) - 1f;
				_isTickLoaded = true;
				(_b_bar, _b_beat) = _calculator.TickToBarBeat(_tick + 1f);
			}
			_isBarBeatLoaded = true;
		}
		(Bar, Beat) = (_b_bar, _b_beat);
	}
	/// <summary>
	/// Allows implicit conversion from a tuple of (int, float) representing bar and beat to an RDBeat instance.
	/// </summary>
	/// <param name="value"></param>
	public static implicit operator TickTime((int, float) value) => new(value.Item1, value.Item2);
	private static int CompareInternal(int bar1, float beat1, int bar2, float beat2)
	{
		int barComparison = bar1.CompareTo(bar2);
		return barComparison != 0 ? barComparison : beat1.CompareTo(beat2);
	}
	private static partial int CompareInternal(TickTime left, TickTime right)
	{
		if (left._isBarBeatLoaded && right._isBarBeatLoaded)
			return CompareInternal(left._b_bar, left._b_beat, right._b_bar, right._b_beat);
		if (left._isTickLoaded && right._isTickLoaded)
			return left._tick.CompareTo(right._tick);
		if (left._isTimeSpanLoaded && right._isTimeSpanLoaded)
			return left._TimeSpan.CompareTo(right._TimeSpan);

		if (left._calculator != null)
		{
			// 用 left 的单位比较
			left.Cache();
			return (right._isTickLoaded ? left.Tick.CompareTo(right._tick)
				: right._isBarBeatLoaded ? CompareInternal(left._b_bar, left._b_beat, right._b_bar, right._b_beat)
				: right._isTimeSpanLoaded ? left.Tick.CompareTo(left._calculator.TimeSpanToTick(right.TimeSpan))
				: throw new InvalidOperationException("The beat cannot be compared."));
		}

		if (right._calculator != null)
		{
			// 用 right 的单位比较
			right.Cache();
			return (left._isTickLoaded ? left._tick.CompareTo(right.Tick)
				: left._isBarBeatLoaded ? CompareInternal(left._b_bar, left._b_beat, right._b_bar, right._b_beat)
				: left._isTimeSpanLoaded ? right.Tick.CompareTo(right._calculator.TimeSpanToTick(left.TimeSpan))
				: throw new InvalidOperationException("The beat cannot be compared."));
		}

		throw new InvalidOperationException("The beat cannot be compared.");
	}
	public override partial string ToString()
	{
		string ToString;
		Deconstruct(out int bar, out float beat);
		if (IsEmpty)
			ToString = $"[{(
				_isTickLoaded ? _tick.ToString() : "?"
				)},{(
				_isBarBeatLoaded ? $"({bar},{beat})" : "?"
				)},{(
				_isTimeSpanLoaded ? _TimeSpan.ToString() : "?"
				)}]";
		else
			ToString = $"[{bar},{beat}]";
		return ToString;
	}
	private bool _isBarBeatLoaded;
	private bool _isCPBLoaded;
	private int _b_bar;
	private float _b_beat;
	private int _CPB;
}