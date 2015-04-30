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
        private UdpClient mUdpClient;
        private IPEndPoint mGroupEP;

        #endregion

        public UdpCacheNotifier(int port)
        {
            mPort = port;
        }

        public void Init()
        {
            try
            {
                mGroupEP = new IPEndPoint(IPAddress.Broadcast, mPort);
                UdpClient mUdpClient = new UdpClient();

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

        private void Send(string command)
        {
            try
            {
                var byteCommand = Encoding.ASCII.GetBytes(command);
                mUdpClient.Send(byteCommand, byteCommand.GetLength(0), mGroupEP);


            }
            catch (Exception e)
            {
                Trace.TraceError("{0}. {1}", e.Message, e);
            }

        }

    }

}
