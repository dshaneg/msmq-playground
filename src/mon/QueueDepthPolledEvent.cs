using System;
using System.Collections.Generic;

namespace mon
{
    public class QueueDepthPolledEvent
    {
        public QueueDepthPolledEvent(string source, IEnumerable<QueueDepthEntry> entries)
        {
            Source = source;
            Timestamp = DateTime.Now;
            Entries = entries;
        }

        public DateTime Timestamp { get; private set; }

        public string Source { get; private set; }

        public IEnumerable<QueueDepthEntry> Entries { get; private set; } 
    }
}
