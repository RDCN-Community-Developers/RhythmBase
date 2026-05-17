using RhythmBase.Adofai.Components;
using RhythmBase.Adofai.Events;
namespace RhythmBase.Adofai.Utils
{
	/// <summary>
	/// Beat Calculator.
	/// </summary>
	public class BeatCalculator : IBeatCalculator<ADBeat>
	{
		internal BeatCalculator(ADLevel level)
		{
			Collection = level;
			Refresh();
		}
		private void Refresh()
		{
			_DefaultBpm = Collection.Settings.Bpm;
			////this._SetSpeeds = this.Collection.EventsWhere<ADSetSpeed>().ToList<ADSetSpeed>();
			////this._Twirls = this.Collection.EventsWhere<ADTwirl>().ToList<ADTwirl>();
			////this._Pauses = this.Collection.EventsWhere<ADPause>().ToList<ADPause>();
			////this._Holds = this.Collection.EventsWhere<ADHold>().ToList<ADHold>();
			////this._Freeroams = this.Collection.EventsWhere<ADFreeRoam>().ToList<ADFreeRoam>();
		}
		/// <summary>
		/// Calculates the total tick value for the specified tile.
		/// </summary>
		/// <param name="tile">The tile for which to calculate the tick.</param>
		/// <returns>The total tick value of the tile.</returns>
		public float TickOf(Tile tile)
		{
			float ticks = 0;
			_cacheTile ??= Collection.Start;
			Status s = _cacheTile._status;
			float lastAngle = 0;

			foreach (var t in Collection)
			{
				float tick = 0;
				float angle = t.Angle;
				float temp = 0;

				bool hairPin = Math.Abs(angle - lastAngle).ToleranceSequals(180f);
				if (t.ContainsType(EventType.Twirl))
					s._flip = !s._flip;
				if (t.ContainsType(EventType.MultiPlanet))
				{
					MultiPlanet mp = t.OfType<MultiPlanet>().Single()!;
					s._planet = mp.Planets;
				}
				if (t.IsMidSpin)
				{
					tick = 0;
					lastAngle += 180;
				}
				else
				{
					if (float.Abs(angle - lastAngle) == 180f)
					{
						// hearpin

					}
					else
					{
						temp = 0.5f - (lastAngle - angle) / 360f;
						var v = s._planet switch
						{
							Planets.TwoPlanets => 0.5f + (float)((lastAngle - angle) / 360f),
							Planets.ThreePlanets => 0.5f + (float)((lastAngle - angle) / 360f) - (1f / 6f),
							_ => throw new NotImplementedException(),
						};
						tick = (1 + v + float.Floor(-v)) * 2;
						if (s._flip)
						{
							tick = 2 - tick;
						}
					}
					lastAngle = angle;
				}
				//Console.WriteLine($"angle: {lastAngle,5} diff: {(temp),5:F2} tick: {tick,5:F2}");
				ticks += tick;
				s._tick = ticks;
				t._status = s;
				s._index++;
				_cacheTile = t;
			}
			return tile._status._tick;
		}

        /// <summary>
        /// Creates an <see cref="ADBeat"/> from the specified beat-only value.
        /// </summary>
        /// <param name="beatOnly">The beat-only value.</param>
        /// <returns>An <see cref="ADBeat"/> representing the specified beat.</returns>
        public ADBeat BeatOf(float beatOnly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an <see cref="ADBeat"/> from the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>An <see cref="ADBeat"/> representing the specified time span.</returns>
        public ADBeat BeatOf(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the specified beat-only value to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="beat">The beat-only value to convert.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the specified beat.</returns>
        public TimeSpan BeatOnlyToTimeSpan(float beat)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the beats per minute (BPM) at the specified beat.
        /// </summary>
        /// <param name="beat">The beat for which to get the BPM.</param>
        /// <returns>The beats per minute at the specified beat.</returns>
        public float BeatsPerMinuteOf(ADBeat beat)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the interval between two beats.
        /// </summary>
        /// <param name="beat1">The first beat.</param>
        /// <param name="beat2">The second beat.</param>
        /// <returns>An <see cref="IBeatRange{ADBeat}"/> representing the interval between the two beats.</returns>
        public IBeatRange<ADBeat> IntervalOf(ADBeat beat1, ADBeat beat2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the interval between two time spans.
        /// </summary>
        /// <param name="timeSpan1">The first time span.</param>
        /// <param name="timeSpan2">The second time span.</param>
        /// <returns>An <see cref="IBeatRange{ADBeat}"/> representing the interval between the two time spans.</returns>
        public IBeatRange<ADBeat> IntervalOf(TimeSpan timeSpan1, TimeSpan timeSpan2)
        {
            throw new NotImplementedException();
        }

        void IBeatCalculator<ADBeat>.Refresh()
        {
            Refresh();
        }

        /// <summary>
        /// Converts the specified time span to a beat-only value.
        /// </summary>
        /// <param name="timeSpan">The time span to convert.</param>
        /// <returns>The beat-only value representing the specified time span.</returns>
        public float TimeSpanToBeatOnly(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
#pragma warning disable IDE0052 // 删除未读的私有成员
        internal ADLevel Collection;
		private float _DefaultBpm = 100;
		private Tile? _cacheTile = null;
#pragma warning restore IDE0052 // 删除未读的私有成员

		/*
		 * 更新：
		 * - 事件添加进砖块
		 * - 事件移除出砖块
		 * - 砖块添加进关卡
		 * - 砖块移除出关卡
		 * - 砖块中旋
		 * - 砖块发卡弯
		 * - 事件属性更新
		 */
	}
}
