﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeerCache
{
    public class PeerBroadcast
    {
        public string Command { get; set; }
        public string Key { get; set; }
        public string Region { get; set; }
        public string SenderClientId { get; set; }
    }
}
