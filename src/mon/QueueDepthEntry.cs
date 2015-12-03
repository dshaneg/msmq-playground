namespace mon
{
    public class QueueDepthEntry
    {
        public QueueDepthEntry(string queueName, int? depth)
        {
            QueueName = queueName;
            Depth = depth;
        }

        public string QueueName { get; private set; }

        public int? Depth { get; private set; }
    }
}
