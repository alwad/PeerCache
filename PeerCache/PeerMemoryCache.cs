using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using PeerCache.Messages;

namespace PeerCache
{
    public class PeerMemoryCache : ICacheService
    {
        private const string DEFAULT_REGION_NAME = "DefaultPeerCache";

        private readonly MemoryCache _cache;
        static PeerMemoryCache _currentCache;
        static object _lock = new object();

        private UdpBroadcastListener _udpListener;
        private UdpCacheNotifier _udpClient;

        private readonly string _peerId;
        private readonly string _regionName;

        private Dictionary<string, bool> _supressNotify;

        public static ICacheService Default
        {
            get
            {
                lock (_lock)
                {
                    if (_currentCache == null)
                        _currentCache = new PeerMemoryCache();
                }

                return _currentCache;
            }
        }

        public PeerMemoryCache() : this(null, null)
        {
        }

        public PeerMemoryCache(string regionName, NameValueCollection config)
        {
            _cache = MemoryCache.Default;
            _peerId = Guid.NewGuid().ToString();
            _regionName = regionName;

            var configSettings = PeerCacheSection.Instance;

            if (string.IsNullOrWhiteSpace(_regionName))
                _regionName = configSettings.RegionName;

            _supressNotify = new Dictionary<string, bool>();

            _udpListener = new UdpBroadcastListener(configSettings.Port);
            _udpClient = new UdpCacheNotifier(configSettings.Port, _peerId);

            _udpListener.Init();
            _udpClient.Init();

            _udpListener.Received += _udpListener_Received;
        }

        private void _udpListener_Received(object sender, GenericEventArgs<byte[]> e)
        {
            var messageText = Encoding.ASCII.GetString(e.Value);
            Trace.TraceInformation("Peer cache received message: {0}", messageText);

            var message = JsonConvert.DeserializeObject<Messages.GenericMessage>(messageText);
            HandleMessage(message);

        }

        private void HandleMessage(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case MessageConstants.MessageTypes.PeerBroadcast:
                    HandlePeerBroadcastMessage(message);
                    break;

            }
        }

        private void HandlePeerBroadcastMessage(GenericMessage message)
        {
            var broadcastMessage = JsonConvert.DeserializeObject<Messages.PeerBroadcast>(message.MessageData);
            if (broadcastMessage.SenderClientId != _peerId && broadcastMessage.Region == _regionName)
            {
                Remove(broadcastMessage.Key, true);
            }
        }

        private PeerCacheItemPolicy CreateCachePolicy(CacheItemPolicy cacheDetails)
        {
            PeerCacheItemPolicy policy = new PeerCacheItemPolicy(cacheDetails);
            policy.OnPeerCacheChanged = OnLocalCacheChanged;

            return policy;
        }

        private void OnLocalCacheChanged(string key, string reason)
        {
            Trace.TraceWarning("Peer cache detected local cache change. key={0}, reason={1}", key, reason);

            if (!_supressNotify[key])
                _udpClient.InvalidateItem(key, _regionName);
        }

        public void Add<T>(string key, T item, CacheItemPolicy cacheDetails)
        {
            Remove(key);
            var ret = _cache.Add(key, item, CreateCachePolicy(cacheDetails));

            if (ret)
            {
                var monitor = new PeerCacheMemoryCacheMonitor(key);
                cacheDetails.ChangeMonitors.Add(monitor);
            }
        }

        public T Get<T>(string key)
        {
            return (T)_cache[key];
        }

        public T Get<T>(string key, Func<string, T> notFoundCallback)
        {
            return Get(key, notFoundCallback, new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(1) });
        }

        public T Get<T>(string key, Func<string, T> notFoundCallback, CacheItemPolicy cacheDetails)
        {
            object itemInCache = _cache[key];
            if (itemInCache == null)
            {
                itemInCache = notFoundCallback(key);
                if (_cache[key] == null)
                {
                    _cache.Add(key, (T)itemInCache, CreateCachePolicy(cacheDetails));
                }
            }
            return (T)itemInCache;
        }
        public void Remove(string key)
        {
            Remove(key, false);
        }

        private void Remove(string key, bool supressNotify)
        {
            if (supressNotify)
                _supressNotify[key] = true;

            _cache.Remove(key);

            _supressNotify[key] = false;
        }
    }
}
