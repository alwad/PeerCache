using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeerCache
{
    public class PeerCacheSection : ConfigurationSection
    {
        private static readonly PeerCacheSection Self =
            ConfigurationManager.GetSection("peerCache") as PeerCacheSection;

        public static PeerCacheSection Instance
        {
            get { return Self; }
        }

        [ConfigurationProperty("port", IsRequired = false, DefaultValue = 11885)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("networkTransport", IsRequired = false, DefaultValue = "udp")]
        public string NetworkTransport
        {
            get { return (string)this["networkTransport"]; }
            set { this["networkTransport"] = value; }
        }

        [ConfigurationProperty("regionName", IsRequired = false, DefaultValue = "DefaultPeerCache")]
        public string RegionName
        {
            get { return (string)this["regionName"]; }
            set { this["regionName"] = value; }
        }

    }
}
