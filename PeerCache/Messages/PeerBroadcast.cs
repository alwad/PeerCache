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
    public class PeerBroadcast
    {
        public const MessageTypes MessageId = MessageTypes.PeerBroadcast;
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageConstants.Commands Command { get; set; }
        public string Key { get; set; }
        public string Region { get; set; }
        public string SenderClientId { get; set; }
    }
}
