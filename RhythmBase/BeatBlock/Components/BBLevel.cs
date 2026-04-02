using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.BeatBlock.Components
{
    public class OrderedEventCollection { 
        public List<Events.BaseEvent> Events { get; set; } = new List<Events.BaseEvent>();
    }
    public class BBLevel : IDisposable
    {
        public Properties Properties { get; set; }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
