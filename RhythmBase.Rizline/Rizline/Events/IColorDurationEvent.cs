using RhythmBase.Rizline.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.Rizline.Rizline.Events;

public interface IColorDurationEvent : IBaseEvent
{
	public Color StartColor { get; set; }
	public Color EndColor { get; set; }
}
