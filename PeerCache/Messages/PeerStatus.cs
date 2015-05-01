using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PeerCache.Messages.MessageConstants;

namespace PeerCache.Messages
{
    public class PeerStatus
    {
        public const MessageTypes MessageId = MessageTypes.PeerStatus;
        public string BroadcastId { get; set; }
        public string ClientId { get; set; }
        public string Hostname { get; set; }
        public string FriendlyName { get; set; }
        public string[] Regions { get; set; }
    }
}
