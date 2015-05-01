using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PeerCache.Messages.MessageConstants;

namespace PeerCache.Messages
{
    public class GenericMessage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageTypes MessageType { get; set; }
        public string MessageId { get; set; }
        public string MessageData { get; set; }
        public DateTime SendTs { get; set; }
    }
}
