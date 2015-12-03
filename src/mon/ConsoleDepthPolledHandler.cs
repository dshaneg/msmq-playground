using System;

namespace mon
{
    public class ConsoleDepthPolledHandler : IPolledHandler<QueueDepthPolledEvent>
    {
        private const ConsoleColor NotFoundColor = ConsoleColor.Blue;
        private const ConsoleColor NormalColor = ConsoleColor.Green;
        private const ConsoleColor WarningColor = ConsoleColor.Yellow;
        private const ConsoleColor ErrorColor = ConsoleColor.Red;

        private readonly QueueDepthThresholdSet _thresholds;

        public ConsoleDepthPolledHandler(QueueDepthThresholdSet thresholds)
        {
            if (thresholds == null) throw new ArgumentNullException("thresholds");
            _thresholds = thresholds;
        }

        public void HandlePolled(object source, QueueDepthPolledEvent args)
        {
            Console.WriteLine("{0}: {1}", args.Source, args.Timestamp);

            foreach (var entry in args.Entries)
            {
                Console.ForegroundColor = _DetermineOutputColor(entry.Depth);

                if (entry.Depth.HasValue)
                    Console.WriteLine("{0}: {1}", entry.QueueName, entry.Depth);
                else
                    Console.WriteLine("{0}: Unknown", entry.QueueName);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        private ConsoleColor _DetermineOutputColor(int? depth)
        {
            if (!depth.HasValue)
                return NotFoundColor;

            return depth < _thresholds.Warning
                ? NormalColor
                : depth < _thresholds.Error
                    ? WarningColor
                    : ErrorColor;
        }

    }
}
