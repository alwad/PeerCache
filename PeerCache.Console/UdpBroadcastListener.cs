using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeerCache.Console
{
    public class GenericEventArgs<T> : EventArgs
    {
        public GenericEventArgs() : this(default(T)) { }
        public GenericEventArgs(T value) { Value = value; }
        public T Value { get; private set; }
    }

    public class UdpBroadcastListener
    {

        #region Members

        private readonly int mPort;
        private UdpClient mUdpClient;
        private IPEndPoint mGroupEP;

        public event EventHandler<GenericEventArgs<byte[]>> Received;

        private Thread mThread;

        #endregion

        public UdpBroadcastListener(int port)
        {
            mPort = port;
        }

        public void Init()
        {
            try
            {
                Trace.TraceWarning("Setting up the UDP packet listener on port {0}", mPort);
                mGroupEP = new IPEndPoint(IPAddress.Any, mPort);
                mUdpClient = new UdpClient();
                mUdpClient.EnableBroadcast = true;
                mUdpClient.Client.SetSocketOption(SocketOptionLevel.Socket,
                                                  SocketOptionName.ReuseAddress,
                                                  true);
                mUdpClient.Client.Bind(mGroupEP);
                Trace.TraceInformation("Successfully bound the UDP packet listener to the the port.");

                mThread = new Thread(UpdateThread);
                mThread.IsBackground = true;
                Trace.TraceInformation("Starting the background listener thread.");
                mThread.Start();
                Trace.TraceInformation("Background listener thread started ok.");
            }
            catch (Exception e)
            {
                Trace.TraceError("{0}. {1}", e.Message, e);
            }
        }

        public void Close()
        {
            if (mThread != null)
            {
                mThread.Abort();
            }

            if (mUdpClient != null)
            {
                mUdpClient.Close();
                mUdpClient = null;
            }
        }

        public Thread Thread
        {
            get { return mThread; }
        }

        private void UpdateThread()
        {
            Trace.TraceInformation("Listener thread started.");
            while (true)
            {
                try
                {
                    Trace.TraceInformation("Waiting for broadcast");

                    byte[] received = mUdpClient.Receive(ref mGroupEP);

                    Trace.TraceInformation("Received {0} bytes", received.Length);
                    OnReceived(received);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("{0}. {1}", ex.Message, ex);
                }
            }
        }

        protected void OnReceived(byte[] pData)
        {
            EventHandler<GenericEventArgs<byte[]>> handler = Received;
            if (handler != null) Received(this, new GenericEventArgs<byte[]>(pData));
        }

        #region Dispose

        private bool mDisposed = false;

        public void Dispose()
        {
            if (mDisposed) return;

            mDisposed = true;

            Close();
        }

        ~UdpBroadcastListener()
        {
            Dispose();
        }

        #endregion

    }


}
