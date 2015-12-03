using System;
using System.Configuration;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using domain;
using messaging;

namespace pub
{
    class Program
    {
        static void Main(string[] args)
        {
            var queuePath = _AcquireQueuePath();

            var cancelSource = new CancellationTokenSource();
            var token = cancelSource.Token;

            var task = _PopulateQueueAsync(queuePath, token);
            
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

        
        private static async Task _PopulateQueueAsync(string multicastAddress, CancellationToken token)
        {
            using (var q = new MessageQueue(multicastAddress, QueueAccessMode.Send))
            {
                if (!q.CanWrite)
                    throw new ApplicationException("Can't write to queue!");

                for (var i = 0; i < 1000; i++)
                {
                    var msg = MsmqJsonMessageFactory.Create(_BuildOrder(i), true, true);

                    q.Send(msg);
                    Console.WriteLine("{0}: Sent '{1}'.", DateTime.Now, msg.Id);

                    await Task.Delay(TimeSpan.FromSeconds(1), token);

                    if (token.IsCancellationRequested)
                        break;
                }
            }
        }

        private static Order _BuildOrder(int id)
        {
            return new Order
            {
                Id = id.ToString(),
                Customer =
                    new Customer
                    {
                        Id = "cust" + id,
                        Email = "cust" + id + "@gamestop.com",
                        Name = "Customer X",
                        Phone = "888-555-1212"
                    },
                LineItems = new[]
                {
                    new LineItem {Id = "1", Price = 15.99m, Quantity = 1, Sku = "123456"},
                    new LineItem {Id = "2", Price = 1.00m, Quantity = 10, Sku = "145378"},
                    new LineItem {Id = "3", Price = 299.99m, Quantity = 1, Sku = "243522"},
                }
            };
        }
    }
}
