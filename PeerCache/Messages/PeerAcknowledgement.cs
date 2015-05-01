using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PeerCache.Messages.MessageConstants;

namespace PeerCache.Messages
{
    public class PeerAcknowledgement
    {
        public const MessageTypes MessageId = MessageTypes.PeerAcknowledgement;
        public string BroadcastId { get; set; }
        public bool Success { get; set; }
        public string ReceiverClientId { get; set; }
        public string Message { get; set; }
    }
}
