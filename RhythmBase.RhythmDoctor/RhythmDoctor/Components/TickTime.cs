using RhythmBase.RhythmDoctor.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// A beat.
/// </summary>
public struct TickTime :ITickTime<TickTime>
#if NET7_0_OR_GREATER
	, IComparisonOperators<TickTime, TickTime, bool>
#endif
{
	internal readonly Level? BaseLevel => _calculator?.Collection;
	/// <summary>
	/// Whether this beat cannot be calculated.
	/// </summary>
	internal static bool MustFromCache { get; } = false;
	/// <summary>
	/// Gets a value indicating whether the current instance contains no loaded data or associated calculator.
	/// </summary>
	/// <remarks>The property returns <see langword="true"/> if the calculator is not set or if none of the data
	/// components are loaded. Use this property to check whether the instance is in an uninitialized or empty state before
	/// performing operations that require loaded data.</remarks>
	[MemberNotNullWhen(false, nameof(_calculator))]
	public readonly bool IsEmpty => _calculator == null || !_isBeatLoaded && !_isBarBeatLoaded && !_isTimeSpanLoaded;
	/// <summary>
	/// The total number of beats from this moment to the beginning of the level.
	/// </summary>
	public float Tick
	{
		get
		{
			if ((!MustFromCache || !_isBeatLoaded) && _calculator is not null)
			{
				if (_isBarBeatLoaded)
					_beat = _calculator.BarBeatToBeatOnly(_b_bar, _b_beat) - 1f;
				else if (_isTimeSpanLoaded)
					_beat = _calculator.TimeSpanToBeatOnly(_TimeSpan) - 1f;
				_isBeatLoaded = true;
			}
			return _beat + 1f;
		}
	}
	/// <summary>
	/// The total amount of time from the beginning of the level to this beat.
	/// </summary>
	public TimeSpan TimeSpan
	{
		get
		{
			if ((!MustFromCache || !_isTimeSpanLoaded) && _calculator is not null)
			{
				if (_isBeatLoaded)
					_TimeSpan = _calculator.BeatOnlyToTimeSpan(_beat + 1f);
				else
				{
					if (_isBarBeatLoaded)
					{
						_beat = _calculator.BarBeatToBeatOnly(_b_bar, _b_beat) - 1f;
						_isBeatLoaded = true;
						_TimeSpan = _calculator.BeatOnlyToTimeSpan(_beat + 1f);
					}
				}
				_isTimeSpanLoaded = true;
			}
			return _TimeSpan;
		}
	}
	/// <summary>
	/// The number of beats per minute followed at this moment.
	/// </summary>
	public float Bpm
	{
		get
		{
			if (!_isBPMLoaded)
			{
				_BPM = _calculator?.BeatsPerMinuteOf(this) ?? 0;
				_isBPMLoaded = true;
			}
			return _BPM;
		}
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
	/// Initializes a new instance of the <see cref="TickTime"/> class with default values.
	/// </summary>
	/// <remarks>This constructor sets the initial state of the <see cref="TickTime"/> instance, including default
	/// values for internal fields. The instance is initialized with a beat value of 1, a bar-beat tuple of (1, 1f), a
	/// zero <see cref="System.TimeSpan"/>, and flags indicating that the beat, bar-beat, and time span are loaded.</remarks>
	public TickTime()
	{
		this = default;
		_beat = 1f;
		(_b_bar, _b_beat) = (1, 1f);
		_TimeSpan = TimeSpan.Zero;
		_isBeatLoaded = true;
		_isBarBeatLoaded = true;
		_isTimeSpanLoaded = true;
	}
	/// <summary>
	/// Construct an instance without specifying a calculator.
	/// </summary>
	/// <param name="beatOnly">The total number of beats from this moment to the beginning of the level.</param>
	public TickTime(float beatOnly)
	{
		this = default;
		if (beatOnly < 1)
			beatOnly = 1;
		_beat = beatOnly - 1f;
		_isBeatLoaded = true;
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
	/// Constructs an instance of RDBeat with the specified time span.
	/// </summary>
	/// <param name="timeSpan">The total amount of time from the start of the level to the moment.</param>
	public TickTime(TimeSpan timeSpan)
	{
		this = default;
		if (timeSpan < TimeSpan.Zero)
			timeSpan = TimeSpan.Zero;
		_TimeSpan = timeSpan;
		_isTimeSpanLoaded = true;
	}
	/// <summary>
	/// Construct an instance with specifying a calculator.
	/// </summary>
	/// <param name="calculator">Specified calculator.</param>
	/// <param name="beatOnly">The total number of beats from this moment to the beginning of the level.</param>
	public TickTime(BeatCalculator calculator, float beatOnly)
	{
		this = new TickTime(beatOnly);
		_calculator = calculator;
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
		_beat = _calculator.BarBeatToBeatOnly(bar, beat) - 1f;
	}
	/// <summary>
	/// Construct an instance with specifying a calculator.
	/// </summary>
	/// <param name="calculator">Specified calculator.</param>
	/// <param name="timeSpan">The total amount of time from the start of the level to the moment</param>
	public TickTime(BeatCalculator calculator, TimeSpan timeSpan)
	{
		this = new TickTime(timeSpan);
		_calculator = calculator;
		_beat = _calculator.TimeSpanToBeatOnly(timeSpan) - 1f;
	}
	/// <summary>
	/// Construct an instance with specifying a calculator.
	/// </summary>
	/// <param name="calculator">Specified calculator.</param>
	/// <param name="beat">Another instance.</param>
	public TickTime(BeatCalculator calculator, TickTime beat)
	{
		this = default;
		if (beat._isBeatLoaded)
		{
			_beat = Math.Max(beat._beat, 0f);
			_isBeatLoaded = true;
			_calculator = calculator;
		}
		else if (beat._isBarBeatLoaded)
		{
			(_b_bar, _b_beat) = (Math.Max(beat._b_bar, 1), Math.Max(beat._b_beat, 1));
			_isBarBeatLoaded = true;
			_calculator = calculator;
			_beat = _calculator.BarBeatToBeatOnly(beat._b_bar, beat._b_beat) - 1f;
		}
		else if (beat._isTimeSpanLoaded)
		{
			_TimeSpan = beat._TimeSpan > TimeSpan.Zero ? beat._TimeSpan : TimeSpan.Zero;
			_isTimeSpanLoaded = true;
			_calculator = calculator;
			_beat = _calculator.TimeSpanToBeatOnly(TimeSpan) - 1f;
		}
	}
	/// <summary>
	/// Construct a beat of the 1st beat from the calculator
	/// </summary>
	/// <param name="calculator">Specified calculator.</param>
	/// <returns>The first beat tied to the level.</returns>
	public static TickTime Default(BeatCalculator calculator)
	{
		TickTime Default = new(calculator, 1f);
		return Default;
	}
	/// <summary>
	/// Determine if two beats come from the same level
	/// </summary>
	/// <param name="a">A beat.</param>
	/// <param name="b">Another beat.</param>
	/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same level.</param>
	/// <returns></returns>
	public static bool FromSameLevel(TickTime a, TickTime b, bool @throw = false) =>
		(a._calculator?.Equals(b._calculator) ?? true)
		|| (@throw ? throw new InvalidOperationException("Beats must come from the same RDLevel.") : false);
	/// <summary>
	/// Determine if two beats are from the same level.
	/// <br />
	/// If any of them does not come from any level, it will also return true.
	/// </summary>
	/// <param name="a">A beat.</param>
	/// <param name="b">Another beat.</param>
	/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same level.</param>
	/// <returns></returns>
	public static bool FromSameLevelOrNull(TickTime? a, TickTime? b, bool @throw = false) => a?._calculator == null || b?._calculator == null || FromSameLevel(a.Value, b.Value, @throw);
	/// <summary>
	/// Determine if two beats are from the same level.
	/// </summary>
	/// <param name="b">Another beat.</param>
	/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same level.</param>
	/// <returns></returns>
	[MemberNotNullWhen(true)]
	public readonly bool FromSameLevel(TickTime b, bool @throw = false) => FromSameLevel(this, b, @throw);
	/// <summary>
	/// Determine if two beats are from the same level.
	/// <br />
	/// If any of them does not come from any level, it will also return true.
	/// </summary>
	/// <param name="b">Another beat.</param>
	/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same level.</param>
	/// <returns></returns>	
	public readonly bool FromSameLevelOrNull(TickTime b, bool @throw = false) => BaseLevel == null || b.BaseLevel == null || FromSameLevel(b, @throw);
	/// <summary>
	/// Returns a new instance of unbinding the level.
	/// </summary>
	/// <returns>A new instance of unbinding the level.</returns>
	public readonly TickTime WithoutLink()
	{
		TickTime result = this;
		if (result._calculator != null)
			result.Cache();
		result._calculator = null;
		return result;
	}
	/// <summary>
	/// Links the current beat to the specified <see cref="BeatCalculator"/>, enabling it to be used for beat calculations.
	/// </summary>
	/// <remarks>If the current beat is already linked to a <see cref="BeatCalculator"/>, providing a different
	/// calculator will result in an exception. This method does not modify the original instance if it is already linked
	/// to the specified calculator.</remarks>
	/// <param name="calculator">The <see cref="BeatCalculator"/> instance to associate with this beat. This parameter cannot be null.</param>
	/// <returns>The current <see cref="TickTime"/> instance with the linked <see cref="BeatCalculator"/>.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the beat is already linked to a different <see cref="BeatCalculator"/>.</exception>
	public readonly TickTime WithLink(BeatCalculator calculator)
	{
		TickTime result = this;
		if (result._calculator == null)
			result._calculator = calculator;
		else if (!ReferenceEquals(result._calculator, calculator))
			throw new InvalidOperationException("The beat is already linked to another level.");
		return result;
    }
	/// <summary>
	/// Returns a new RDBeat instance with the specified calculator assigned if the current instance does not already have
	/// a calculator.
	/// </summary>
	/// <remarks>Use this method to ensure that an RDBeat instance has an associated calculator without overwriting
	/// an existing one.</remarks>
	/// <param name="calculator">The BeatCalculator to assign if the current instance's calculator is null.</param>
	/// <returns>An RDBeat instance with the calculator set to the specified value if it was previously null; otherwise, the current
	/// instance.</returns>
	public readonly TickTime WithLinkIfNull(BeatCalculator calculator)
	{
		TickTime result = this;
		if (result._calculator == null)
			result._calculator = calculator;
		return result;
    }
	/// <summary>
	/// Creates a new RDBeat instance that is linked to the specified RDLevel.
	/// </summary>
	/// <param name="level">The RDLevel to which the new RDBeat instance will be linked. Cannot be null.</param>
	/// <returns>A new RDBeat instance associated with the provided RDLevel.</returns>
    public readonly TickTime WithLink(Level level) => WithLink(level.Calculator);
	/// <summary>
	/// Returns a new RDBeat instance that is linked to the specified level's calculator if it is not already linked.
	/// </summary>
	/// <param name="level">The RDLevel whose calculator will be used to link the RDBeat instance. This parameter must not be null.</param>
	/// <returns>A new RDBeat instance linked to the calculator of the specified level.</returns>
	public readonly TickTime WithLinkIfNull(Level level) => WithLinkIfNull(level.Calculator);
    [MemberNotNull(nameof(_calculator))]
	internal readonly void IfNullThrowException()
	{
		if (IsEmpty)
		{
			throw new InvalidRDBeatException();
		}
	}
	/// <summary>
	/// Refresh the cache.
	/// </summary>
	public void ResetCache()
	{
		_ = Tick;
		_isBarBeatLoaded = false;
		_isTimeSpanLoaded = false;
	}
	/// <summary>
	/// Caches the current state of the beat by accessing its properties.
	/// </summary>
	/// <exception cref="InvalidRDBeatException">Thrown when the beat cannot be calculated.</exception>
	public void Cache()
	{
		IfNullThrowException();
		_ = Tick;
		Deconstruct(out _, out _);
		_ = TimeSpan;
		_ = Bpm;
		_ = Cpb;
	}
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
			if (_isBeatLoaded)
				(_b_bar, _b_beat) = _calculator.BeatOnlyToBarBeat(_beat + 1f);
			else if (_isTimeSpanLoaded)
			{
				_beat = _calculator.TimeSpanToBeatOnly(_TimeSpan) - 1f;
				_isBeatLoaded = true;
				(_b_bar, _b_beat) = _calculator.BeatOnlyToBarBeat(_beat + 1f);
			}
			_isBarBeatLoaded = true;
		}
		(Bar, Beat) = (_b_bar, _b_beat);
	}
	internal void ResetBPM()
	{
		if (!_isBeatLoaded)
			_beat = _calculator?.TimeSpanToBeatOnly(_TimeSpan) - 1f ?? throw new InvalidRDBeatException();
		_isBeatLoaded = true;
		_isTimeSpanLoaded = false;
		_isBPMLoaded = false;
	}
	internal void ResetCPB()
	{
		if (!_isBeatLoaded)
			_beat = _calculator?.BarBeatToBeatOnly(_b_bar, _b_beat) - 1f ?? throw new InvalidRDBeatException();
		_isBeatLoaded = true;
		_isBarBeatLoaded = false;
		_isCPBLoaded = false;
	}
	///  <inheritdoc/>
	public static TickTime operator +(TickTime a, float b)
	{
		if (b == 0) return a;
		TickTime result;
		if (!a.IsEmpty)
			result = new TickTime(a._calculator!, a.Tick + b);
		else
		{
			result = new TickTime();
			if (!a._isBeatLoaded && !a._isBarBeatLoaded)
				throw new ArgumentNullException(nameof(a), "The beat cannot be calculated.");
			if (a._isBeatLoaded)
			{
				result._beat = a._beat + b;
				result._isBeatLoaded = true;
				result._isBarBeatLoaded = false;
			}
			else if (a._isBarBeatLoaded)
			{
				(result._b_bar, result._b_beat) = (a._b_bar, a._b_beat + b);
				result._isBarBeatLoaded = true;
				result._isBeatLoaded = false;
			}
			result._isTimeSpanLoaded = false;
		}
		return result;
	}
	///  <inheritdoc/>
	public static TickTime operator +(TickTime a, TimeSpan b)
	{
		if (b == TimeSpan.Zero) return a;
		TickTime result;
		if (!a.IsEmpty)
			result = new TickTime(a._calculator!, a.TimeSpan + b);
		else
		{
			result = new TickTime();
			if (a._isTimeSpanLoaded)
			{
				result._TimeSpan = a._TimeSpan + b;
				result._isTimeSpanLoaded = true;
				result._isBeatLoaded = result._isBarBeatLoaded = false;
			}
			else
				throw new ArgumentNullException(nameof(a), "The beat cannot be calculated.");
		}
		return result;
	}
	///  <inheritdoc/>
	public static TickTime operator -(TickTime a, float b)
	{
		if (b == 0f) return a;
		TickTime result;
		if (!a.IsEmpty)
			result = new TickTime(a._calculator!, a.Tick - b);
		else
		{
			result = new TickTime();
			if (!a._isBeatLoaded && !a._isBarBeatLoaded)
				throw new ArgumentNullException(nameof(a), "The beat cannot be calculated.");
			if (a._isBeatLoaded)
			{
				result._beat = a._beat - b;
				result._isBeatLoaded = true;
				result._isBarBeatLoaded = false;
			}
			if (a._isBarBeatLoaded)
			{
				(result._b_bar, result._b_beat) = (a._b_bar, a._b_beat - b);
				result._isBarBeatLoaded = true;
				result._isBeatLoaded = false;
			}
			result._isTimeSpanLoaded = false;
		}
		return result;
	}
	///  <inheritdoc/>
	public static TickTime operator -(TickTime a, TimeSpan b)
	{
		if (b == TimeSpan.Zero) return a;
		TickTime result;
		if (!a.IsEmpty)
			result = new TickTime(a._calculator!, a.TimeSpan - b);
		else
		{
			result = new TickTime();
			if (a._isTimeSpanLoaded)
			{
				result._TimeSpan = a._TimeSpan - b;
				result._isTimeSpanLoaded = true;
				result._isBeatLoaded = result._isBarBeatLoaded = false;
			}
			else
				throw new ArgumentNullException(nameof(a), "The beat cannot be calculated.");
		}
		return result;
	}
	///  <inheritdoc/>
	public static bool operator >(TickTime a, TickTime b) => CompareInternal(a, b) > 0;
	///  <inheritdoc/>
	public static bool operator <(TickTime a, TickTime b) => CompareInternal(a, b) < 0;
	///  <inheritdoc/>
	public static bool operator >=(TickTime a, TickTime b) => CompareInternal(a, b) >= 0;
	///  <inheritdoc/>
	public static bool operator <=(TickTime a, TickTime b) => CompareInternal(a, b) <= 0;
	///  <inheritdoc/>
	public static bool operator ==(TickTime a, TickTime b) => CompareInternal(a, b) == 0;
	///  <inheritdoc/>
	public static bool operator !=(TickTime a, TickTime b) => CompareInternal(a, b) != 0;
    /// <summary>
    /// Allows implicit conversion from a tuple of (int, float) representing bar and beat to an RDBeat instance.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator TickTime((int, float) value) => new(value.Item1, value.Item2);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int CompareInternal(int bar1, float beat1, int bar2, float beat2)
	{
		int barComparison = bar1.CompareTo(bar2);
		return barComparison != 0 ? barComparison : beat1.CompareTo(beat2);
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int CompareInternal(TickTime left, TickTime right)
	{
		if (left._isBarBeatLoaded && right._isBarBeatLoaded)
			return CompareInternal(left._b_bar, left._b_beat, right._b_bar, right._b_beat);
		if (left._isBeatLoaded && right._isBeatLoaded)
			return left._beat.CompareTo(right._beat);
		if (left._isTimeSpanLoaded && right._isTimeSpanLoaded)
			return left._TimeSpan.CompareTo(right._TimeSpan);

		if (left._calculator != null)
		{
			// 用 left 的单位比较
			left.Cache();
			return (right._isBeatLoaded ? left.Tick.CompareTo(right._beat)
				: right._isBarBeatLoaded ? CompareInternal(left._b_bar, left._b_beat, right._b_bar, right._b_beat)
				: right._isTimeSpanLoaded ? left.Tick.CompareTo(left._calculator.TimeSpanToBeatOnly(right.TimeSpan))
				: throw new InvalidOperationException("The beat cannot be compared."));
		}

		if (right._calculator != null)
		{
			// 用 right 的单位比较
			right.Cache();
			return (left._isBeatLoaded ? left._beat.CompareTo(right.Tick)
				: left._isBarBeatLoaded ? CompareInternal(left._b_bar, left._b_beat, right._b_bar, right._b_beat)
				: left._isTimeSpanLoaded ? right.Tick.CompareTo(right._calculator.TimeSpanToBeatOnly(left.TimeSpan))
				: throw new InvalidOperationException("The beat cannot be compared."));
		}

		throw new InvalidOperationException("The beat cannot be compared.");
	}
	/// <inheritdoc/>
	public override string ToString()
	{
		string ToString;
		Deconstruct(out int bar, out float beat);
		if (IsEmpty)
			ToString = $"[{(
				_isBeatLoaded ? _beat.ToString() : "?"
				)},{(
				_isBarBeatLoaded ? $"({bar},{beat})" : "?"
				)},{(
				_isTimeSpanLoaded ? _TimeSpan.ToString() : "?"
				)}]";
		else
			ToString = $"[{bar},{beat}]";
		return ToString;
	}
	///  <inheritdoc/>
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is TickTime e && Equals(e);
	///  <inheritdoc/>
	public readonly bool Equals(TickTime other) => this == other;
	///  <inheritdoc/>
#if NETSTANDARD
	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + (_calculator?.GetHashCode() ?? 0);
		hash = hash * 31 + _isBeatLoaded.GetHashCode();
		hash = hash * 31 + _isBarBeatLoaded.GetHashCode();
		hash = hash * 31 + _isTimeSpanLoaded.GetHashCode();
		hash = hash * 31 + _beat.GetHashCode();
		hash = hash * 31 + _b_bar.GetHashCode();
		hash = hash * 31 + _b_beat.GetHashCode();
		hash = hash * 31 + _TimeSpan.GetHashCode();
		return hash;
	}
#else
	public override int GetHashCode() => HashCode.Combine(Tick, BaseLevel);
#endif
	///  <inheritdoc/>
	public readonly int CompareTo(TickTime other) => this > other ? 1 : this == other ? 0 : -1;
//#if NET8_0_OR_GREATER
//	static RDBeat IBeat<RDBeat>.FromBeatOnly(float beatOnly)
//    {
//		return new RDBeat(beatOnly);
//    }

//    static RDBeat IBeat<RDBeat>.FromTimeSpan(TimeSpan timeSpan)
//    {
//		return new RDBeat(timeSpan);
//    }
//#endif

    internal BeatCalculator? _calculator;
	private bool _isBeatLoaded;
	private bool _isBarBeatLoaded;
	private bool _isTimeSpanLoaded;
	private bool _isBPMLoaded;
	private bool _isCPBLoaded;
	private float _beat;
	private int _b_bar;
	private float _b_beat;
	private TimeSpan _TimeSpan;
	private float _BPM;
	private int _CPB;
}
