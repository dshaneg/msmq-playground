namespace mon
{
    public class QueueDepthThresholdSet
    {
        public QueueDepthThresholdSet(int warningThreshold, int errorThreshold)
        {
            Warning = warningThreshold;
            Error = errorThreshold;
        }

        public int Warning { get; private set; }

        public int Error { get; private set; }
    }
}