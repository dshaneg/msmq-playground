using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Messaging;

namespace mon
{
    public class QueueDepthPoller : IPoller<QueueDepthPolledEvent>
    {
        public event EventHandler<QueueDepthPolledEvent> Polled;

        public void Poll()
        {
            var queues = MessageQueue.GetPrivateQueuesByMachine(".");

            var entries = new List<QueueDepthEntry>();

            foreach (var q in queues)
            {
                try
                {
                    using (var c = new PerformanceCounter("MSMQ Queue", "Messages in Queue",
                        string.Format(@"{0}\{1}", q.MachineName, q.QueueName), true))
                    {
                        var depth = (int)c.NextValue();

                        entries.Add(new QueueDepthEntry(q.QueueName, depth));
                    }
                }
                catch (Exception ex)
                {
                    entries.Add(new QueueDepthEntry(q.QueueName, null));
                }
                finally
                {
                    q.Dispose();
                    Console.ResetColor();
                }

            }

            OnPolled(new QueueDepthPolledEvent(Environment.MachineName, entries));
        }

        private void OnPolled(QueueDepthPolledEvent args)
        {
            if (Polled != null)
                Polled(this, args);
        }
    }
}
