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

        public ADBeat BeatOf(float beatOnly)
        {
            throw new NotImplementedException();
        }

        public ADBeat BeatOf(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        public TimeSpan BeatOnlyToTimeSpan(float beat)
        {
            throw new NotImplementedException();
        }

        public float BeatsPerMinuteOf(ADBeat beat)
        {
            throw new NotImplementedException();
        }

        public IBeatRange<ADBeat> IntervalOf(ADBeat beat1, ADBeat beat2)
        {
            throw new NotImplementedException();
        }

        public IBeatRange<ADBeat> IntervalOf(TimeSpan timeSpan1, TimeSpan timeSpan2)
        {
            throw new NotImplementedException();
        }

        void IBeatCalculator<ADBeat>.Refresh()
        {
            Refresh();
        }

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
