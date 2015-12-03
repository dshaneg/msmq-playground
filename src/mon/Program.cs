using System;
using System.Configuration;

namespace mon
{
    class Program
    {
        static void Main(string[] args)
        {
            var pollDelayMilliseconds = _GetPollDelayMilliseconds();
            var thresholds = _GetThresholds();

            IPoller<QueueDepthPolledEvent> poller = new QueueDepthPoller();
            IPolledHandler<QueueDepthPolledEvent> handler = new ConsoleDepthPolledHandler(thresholds);

            var driver = new PollDriver<QueueDepthPolledEvent>(poller, handler, pollDelayMilliseconds);

            driver.Start();

            Console.ReadLine();
            driver.Stop();
        }

        private static QueueDepthThresholdSet _GetThresholds()
        {
            var warningThreshold = int.Parse(ConfigurationManager.AppSettings["warningThreshold"]);
            var errorThreshold = int.Parse(ConfigurationManager.AppSettings["errorThreshold"]);

            return new QueueDepthThresholdSet(warningThreshold, errorThreshold);
        }

        private static int _GetPollDelayMilliseconds()
        {
            return int.Parse(ConfigurationManager.AppSettings["pollDelayMilliseconds"]);
        }
    }
}
