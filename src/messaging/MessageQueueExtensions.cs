using System.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace messaging
{
    public static class MessageQueueExtensions
    {
        public static Task<Message> ReceiveAsync(this MessageQueue queue, CancellationToken cancelToken)
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
