using RhythmBase.Adofai.Utils;
using RhythmBase.Global.Components;
using RhythmBase.Global.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace RhythmBase.Adofai.Components;

	/// <summary>
	/// Represents a beat in the ADLevel.
	/// </summary>
	public struct TickTime : ITickTime<TickTime>
#if !NETSTANDARD
		,
		IAdditionOperators<TickTime, float, TickTime>,
		IAdditionOperators<TickTime, TimeSpan, TickTime>,
		ISubtractionOperators<TickTime, float, TickTime>,
		ISubtractionOperators<TickTime, TimeSpan, TickTime>,
		IComparisonOperators<TickTime,TickTime, bool>
#endif
	{
		internal readonly Level? _baseLevel => _calculator?.Collection;
		/// <summary>
		/// Gets or sets the beat only value.
		/// </summary>
		public readonly float Tick
		{
			get => _beat + 1f;
			set
			{
			}
		}
		/// <summary>
		/// Gets or sets the time span.
		/// </summary>
		public readonly TimeSpan TimeSpan
		{
			get => _timeSpan;
			set
			{
			}
		}
		/// <summary>
		/// Gets the collection of planets associated with the current context.
		/// </summary>
		public readonly Planets Planets
		{
			get
			{
				IfNullThrowException();
				throw new RhythmBaseException();
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="TickTime"/> struct with a specified beat.
		/// </summary>
		/// <param name="beat">The beat value.</param>
		public TickTime(float beat)
		{
			this = default;
			_beat = beat;
			_isBeatLoaded = true;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="TickTime"/> struct with a specified time span.
		/// </summary>
		/// <param name="timeSpan">The time span value.</param>
		public TickTime(TimeSpan timeSpan)
		{
			this = default;
			_timeSpan = timeSpan;
			_isTimeSpanLoaded = true;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="TickTime"/> struct with a specified calculator and beat.
		/// </summary>
		/// <param name="calculator">The beat calculator.</param>
		/// <param name="beat">The beat value.</param>
		public TickTime(BeatCalculator? calculator, float beat)
		{
			this = default;
			_calculator = calculator;
			_beat = beat;
			_isBeatLoaded = true;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="TickTime"/> struct with a specified calculator and time span.
		/// </summary>
		/// <param name="calculator">The beat calculator.</param>
		/// <param name="timeSpan">The time span value.</param>
		/// <exception cref="OverflowException">Thrown when the time span is less than zero.</exception>
		public TickTime(BeatCalculator? calculator, TimeSpan timeSpan)
		{
			this = default;
			if (timeSpan < TimeSpan.Zero)
			{
				throw new OverflowException($"The time must not be less than zero, but {timeSpan} is given");
			}
			_calculator = calculator;
			_timeSpan = timeSpan;
			_isTimeSpanLoaded = true;
		}
		/// <summary>
		/// Construct a beat of the 1st beat from the calculator
		/// </summary>
		/// <param name="calculator">Specified calculator.</param>
		/// <returns>The first beat tied to the level.</returns>
		public static TickTime Default(BeatCalculator calculator)
		{
			TickTime @default = new(calculator, 1f);
			return @default;
		}
		/// <summary>
		/// Determine if two beats come from the same level
		/// </summary>
		/// <param name="a">A beat.</param>
		/// <param name="b">Another beat.</param>
		/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same level.</param>
		/// <returns></returns>
		public static bool FromSameLevel(TickTime a, TickTime b, bool @throw = false)
		{
			bool flag = a._baseLevel?.Equals(b._baseLevel) ?? true;
			bool fromSameLevel;
			if (flag)
			{
				fromSameLevel = true;
			}
			else
			{
				if (@throw)
				{
					throw new RhythmBaseException("Beats must come from the same ADLevel.");
				}
				fromSameLevel = false;
			}
			return fromSameLevel;
		}
		/// <summary>
		/// Determine if two beats are from the same level.
		/// <br />
		/// If any of them does not come from any level, it will also return true.
		/// </summary>
		/// <param name="a">A beat.</param>
		/// <param name="b">Another beat.</param>
		/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same level.</param>
		/// <returns></returns>
		public static bool FromSameLevelOrNull(TickTime a, TickTime b, bool @throw = false) => a._baseLevel == null || b._baseLevel == null || FromSameLevel(a, b, @throw);
		/// <summary>  
		/// Determines if the current beat and the specified beat are from the same level.  
		/// </summary>  
		/// <param name="b">The beat to compare with the current beat.</param>  
		/// <param name="throw">If set to <c>true</c>, an exception will be thrown if the beats are not from the same level.</param>  
		/// <returns>  
		/// <c>true</c> if the beats are from the same level; otherwise, <c>false</c>.  
		/// </returns>  
		public readonly bool FromSameLevel(TickTime b, bool @throw = false) => FromSameLevel(this, b, @throw);
		/// <summary>
		/// Determine if two beats are from the same level.
		/// <br />
		/// If any of them does not come from any level, it will also return true.
		/// </summary>
		/// <param name="b">Another beat.</param>
		/// <param name="throw">If true, an exception will be thrown when two beats do not come from the same level.</param>
		/// <returns></returns>	
		public readonly bool FromSameLevelOrNull(TickTime b, bool @throw = false) => _baseLevel == null || b._baseLevel == null || FromSameLevel(b, @throw);
		/// <summary>
		/// Returns a new instance of unbinding the level.
		/// </summary>
		/// <returns>A new instance of unbinding the level.</returns>
		public readonly TickTime WithoutBinding()
		{
			TickTime result = this;
			result._calculator = null;
			return result;
		}
		private readonly void IfNullThrowException()
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
			_isTimeSpanLoaded = false;
		}
		internal void ResetBPM()
		{
			_isBeatLoaded = true;
			_isTimeSpanLoaded = false;
			_isBpmLoaded = false;
		}
		internal void ResetCPB() => _isBeatLoaded = true;
		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
		/// </value>
		public readonly bool IsEmpty => _calculator == null || (!_isBeatLoaded && !_isTimeSpanLoaded);

		/// <inheritdoc/>
		public static TickTime operator +(TickTime a, float b)
		{
			TickTime result = new(a._calculator, a.Tick + b);
			return result;
		}
		/// <inheritdoc/>
		public static TickTime operator +(TickTime a, TimeSpan b)
		{
			TickTime result = new(a._calculator, a.TimeSpan + b);
			return result;
		}
		/// <inheritdoc/>
		public static TickTime operator -(TickTime a, float b)
		{
			TickTime result = new(a._calculator, a.Tick - b);
			return result;
		}
		/// <inheritdoc/>
		public static TickTime operator -(TickTime a, TimeSpan b)
		{
			TickTime result = new(a._calculator, a.TimeSpan - b);
			return result;
		}
		/// <inheritdoc/>
		public static bool operator >(TickTime a, TickTime b) => FromSameLevel(a, b, true) && a.Tick > b.Tick;
		/// <inheritdoc/>
		public static bool operator <(TickTime a, TickTime b) => FromSameLevel(a, b, true) && a.Tick < b.Tick;
		/// <inheritdoc/>
		public static bool operator >=(TickTime a, TickTime b) => FromSameLevel(a, b, true) && a.Tick >= b.Tick;
		/// <inheritdoc/>
		public static bool operator <=(TickTime a, TickTime b) => FromSameLevel(a, b, true) && a.Tick <= b.Tick;
		/// <inheritdoc/>
		public static bool operator ==(TickTime a, TickTime b) => (FromSameLevel(a, b, true) && a._beat == b._beat) || (a._isTimeSpanLoaded && b._isTimeSpanLoaded && a._timeSpan == b._timeSpan) || a.Tick == b.Tick;
		/// <inheritdoc/>
		public static bool operator !=(TickTime a, TickTime b) => !(a == b);
		/// <inheritdoc/>
		public readonly int CompareTo(TickTime other) => checked((int)Math.Round((double)unchecked(_beat - other._beat)));
		/// <inheritdoc/>
		public readonly override string ToString() => $"[{Tick}]";
		/// <inheritdoc/>
		public readonly override bool Equals(object? obj) => obj is TickTime beat && Equals(beat);
		/// <inheritdoc/>
		public readonly bool Equals([NotNull] TickTime other) => this == other;
		/// <inheritdoc/>
#if NETSTANDARD
		public readonly override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + Tick.GetHashCode();
			hash = hash * 23 + (_baseLevel?.GetHashCode() ?? 0);
			return hash;
		}
#else
		public readonly override int GetHashCode() => HashCode.Combine(Tick, _baseLevel);
#endif
    internal BeatCalculator? _calculator;
		private bool _isBeatLoaded;
		private bool _isTimeSpanLoaded;
		private bool _isBpmLoaded;
		private readonly float _beat;
		private readonly TimeSpan _timeSpan;
		private float _bpm;
	}
