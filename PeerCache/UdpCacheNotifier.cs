using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeerCache
{
    public class UdpCacheNotifier
    {

        #region Members

        private readonly int mPort;
        private readonly string _clientId;
        private UdpClient mUdpClient;
        private IPEndPoint mGroupEP;

        #endregion

        public UdpCacheNotifier(int port, string clientId)
        {
            mPort = port;
            _clientId = clientId;
        }

        public void Init()
        {
            try
            {
                mUdpClient = new UdpClient();
                mGroupEP = new IPEndPoint(IPAddress.Broadcast, mPort);

                mUdpClient.EnableBroadcast = true;
                mUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket,
                                                  SocketOptionName.ReuseAddress,
                                                  true);

            }
            catch (Exception e)
            {
                Trace.TraceError("{0}. {1}", e.Message, e);
            }
        }

        public void Close()
        {
            if (mUdpClient != null)
            {
                mUdpClient.Close();
                mUdpClient = null;
            }
        }

        public void InvalidateItem(string key, string region)
        {
            try
            {
                if (mUdpClient == null)
                    throw new InvalidOperationException("Invalid when connection not initialized or closed.");

                PeerBroadcast message = new PeerBroadcast()
                {
                    Command = "expire",
                    Key = key,
                    SenderClientId = _clientId,
                    Region = region
                };

                var byteCommand = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));
                mUdpClient.Send(byteCommand, byteCommand.GetLength(0), mGroupEP);


            }
            catch (Exception e)
            {
                Trace.TraceError("{0}. {1}", e.Message, e);
            }

        }

    }

}
