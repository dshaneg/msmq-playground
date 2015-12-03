using System.IO;
using System.Messaging;
using Newtonsoft.Json;

namespace messaging
{
    public static class MessageExtensions
    {
        public static T GetBody<T>(this Message message)
        {
            using (var bodyReader = new StreamReader(message.BodyStream))
            {
                var bodyText = bodyReader.ReadToEnd();
                var bodyObject = JsonConvert.DeserializeObject<T>(bodyText);

                return bodyObject;
            }
        }
    }
}
