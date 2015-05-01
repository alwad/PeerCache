using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeerCache.Messages
{
    public static class MessageConstants
    {
        public static Type MessageType(MessageTypes headerValue)
        {
            switch (headerValue)
            {
                case MessageTypes.PeerBroadcast:
                    return typeof(PeerBroadcast);
                case MessageTypes.PeerAcknowledgement:
                    return typeof(PeerAcknowledgement);
                case MessageTypes.PeerStatus:
                    return typeof(PeerStatus);
            }

            return null;
        }
        public enum MessageTypes
        {
            PeerBroadcast = 1,
            PeerAcknowledgement,
            PeerStatus
        }

        public enum Commands
        {
            ExpireCache = 1,
            PeerStatus            
        }

    }
}
