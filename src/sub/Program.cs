using System;
using System.Configuration;
using System.IO;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sub
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueInfo = _AcquireQueuePath();

            _EnsureQueue(queueInfo);

            var cancelSource = new CancellationTokenSource();
            var token = cancelSource.Token;

            var task = _DrainQueue(queueInfo.Item1, token);

            Console.WriteLine("done. <Enter> to cancel the queue reader.");
            Console.ReadLine();

            cancelSource.Cancel();

            Console.WriteLine("after cancel. about to exit.");
            Console.ReadLine();
        }

        private static Tuple<string, string> _AcquireQueuePath()
        {
            var path = ConfigurationManager.AppSettings["queuePath"];
            var multicastAddress = ConfigurationManager.AppSettings["multicastAddress"];

            if (path == null)
                throw new ConfigurationErrorsException("'queuePath' not defined in appsettings.");
            if (multicastAddress == null)
                throw new ConfigurationErrorsException("'multicastAddress' not defined in appsettings.");

            return new Tuple<string, string>(path, multicastAddress);
        }

        private static void _EnsureQueue(Tuple<string, string> queueInfo)
        {
            if (!MessageQueue.Exists(queueInfo.Item1))
            {
                
                using (var queue = MessageQueue.Create(queueInfo.Item1))
                {
                    queue.MulticastAddress = queueInfo.Item2;
                    //queue.Refresh();
                }
            }
        }

        // returning async void!
        private static async Task _DrainQueue(string path, CancellationToken token)
        {
            using (var q = new MessageQueue(path, QueueAccessMode.Receive))
            {
                if (!q.CanRead)
                    throw new ApplicationException("Can't read from queue!");

                while (!token.IsCancellationRequested)
                {
                    var message = await ReceiveAsync(q, token);
                    if (message == null)
                        continue;

                    using (var bodyReader = new StreamReader(message.BodyStream))
                    {
                        var body = bodyReader.ReadToEnd();
                        Console.WriteLine(body);
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(1), token);
                }

                Console.WriteLine("Canceled.");
            }
        }

        public static Task<Message> ReceiveAsync(MessageQueue queue, CancellationToken cancelToken)
        {
            return Task<Message>.Factory.StartNew(() => Receive(queue, cancelToken), cancelToken);
        }

        private static Message Receive(MessageQueue queue, CancellationToken cancelToken)
        {
            var receiveAsyncResult = queue.BeginReceive();
            WaitHandle.WaitAny(new[] { receiveAsyncResult.AsyncWaitHandle, cancelToken.WaitHandle });

            Message message = null;

            if (!cancelToken.IsCancellationRequested)
            {
                message = queue.EndReceive(receiveAsyncResult);
            }

            return message;
        }
    }
}
