using System.IO;
using System.Messaging;
using System.Text;
using Newtonsoft.Json;

namespace messaging
{
    public static class MsmqJsonMessageFactory
    {
        public static Message Create(object serializableBody, bool persistent = true, bool journaled = false)
        {
            var msgContent = JsonConvert.SerializeObject(serializableBody);

            var msg = new Message
            {
                Recoverable = persistent,
                UseJournalQueue = journaled,
                BodyStream = new MemoryStream(Encoding.Default.GetBytes(msgContent))
            };

            return msg;
        }
    }
}
