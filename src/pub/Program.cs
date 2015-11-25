using System;
using System.Configuration;
using System.IO;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pub
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = _AcquireQueuePath();

            _EnsureQueue(path);

            var cancelSource = new CancellationTokenSource();
            var token = cancelSource.Token;

            var task = _PopulateQueueAsync(path, token);
            
            Console.WriteLine("done. <Enter> to cancel the queue writer.");
            Console.ReadLine();

            cancelSource.Cancel();

            Console.WriteLine("after cancel. about to exit.");
            Console.ReadLine();
        }

        private static string _AcquireQueuePath()
        {
            var path = ConfigurationManager.AppSettings["queuePath"];

            if (path == null)
                throw new ConfigurationErrorsException("'queuePath' not defined in appsettings.");

            return path;
        }

        private static void _EnsureQueue(string path)
        {
            if (!MessageQueue.Exists(path))
            {
                MessageQueue.Create(path);
            }
        }

        private static async Task _PopulateQueueAsync(string path, CancellationToken token)
        {
            using (var q = new MessageQueue(path, QueueAccessMode.Send))
            {
                if (!q.CanWrite)
                    throw new ApplicationException("Can't write to queue!");

                for (var i = 0; i < 1000; i++)
                {
                    var msgText = "hello world " + i;

                    var msg = new Message
                    {
                        Recoverable = true,
                        BodyStream = new MemoryStream(Encoding.Default.GetBytes(msgText))
                    };

                    q.Send(msg);
                    Console.WriteLine("{0}: Sent '{1}'.", DateTime.Now, msgText);

                    await Task.Delay(TimeSpan.FromSeconds(1), token);

                    if (token.IsCancellationRequested)
                        break;
                }
            }
        }
    }
}
