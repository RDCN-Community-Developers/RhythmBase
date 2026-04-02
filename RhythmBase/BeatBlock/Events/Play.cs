using RhythmBase.Global.Components.Easing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.BeatBlock.Events
{
    public record class Play : BaseEvent
    {
        public override Enums Type => Enums.Play;
        public FileReference File { get; set; }
        public float Bpm { get; set; }
        public float Volume { get; set; }
    }
}
