using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace PeerCache.Console
{
    class Program
    {

        static void Main(string[] args)
        {
            UdpBroadcastListener listener = new UdpBroadcastListener(11885);

            listener.Init();
            listener.Received += Listener_Received;

            string input = "help";

            while (!string.IsNullOrEmpty(input))
            {
                switch (input.ToLowerInvariant().Substring(0, 1))
                {
                    case "q":
                    case "quit":
                        return;

                    case "h":
                    case "help":
                        System.Console.WriteLine("q: quit, h: help, u: send UDP command, c: [value] cache item, g: [key] get item");
                        break;

                    case "u":
                        SendUdp(input.TrimStart('u', ' '));
                        break;

                    case "c":
                        CacheText(input.TrimStart('c', ' '));
                        break;

                    case "k":
                        CacheKey(input.TrimStart('k', ' '), 10);
                        break;

                    case "l":
                        CacheKey(input.Remove(0, 2), 3600);
                        break;


                    case "g":
                        GetCache(input.TrimStart('g', ' '));
                        break;

                }

                System.Console.WriteLine("");
                System.Console.Write("$ ");

                input = System.Console.ReadLine();
            }

        }

        private static void CacheText(string v)
        {
            var key = Guid.NewGuid().ToString().ToLower().Substring(0, 3);
            var cache = PeerCache.PeerMemoryCache.Default;

            cache.Add<string>(key, v, new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.AddSeconds(20) });
            System.Console.WriteLine("ACK: OK - KEY = " + key);
        }

        private static void CacheKey(string key, int seconds)
        {
            var val = Guid.NewGuid().ToString();
            var cache = PeerCache.PeerMemoryCache.Default;

            cache.Add<string>(key, val, new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.AddSeconds(seconds) });
            System.Console.WriteLine("ACK: OK - KEY = " + key);
        }


        private static void GetCache(string key)
        {
            var cache = PeerCache.PeerMemoryCache.Default;

            var val = cache.Get<string>(key);
            System.Console.WriteLine("ACK: OK - Value = " + val);
        }

        private static void Listener_Received(object sender, GenericEventArgs<byte[]> e)
        {
            var message = Encoding.ASCII.GetString(e.Value);

            System.Console.WriteLine("<< Received message >>");
            System.Console.WriteLine(message);
            System.Console.WriteLine("<< EOM >>");
            System.Console.WriteLine("");
        }

        private static void SendUdp(string command)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, 11885);
                UdpClient client = new UdpClient();

                client.EnableBroadcast = true;
                client.Client.SetSocketOption(SocketOptionLevel.Socket,
                                                  SocketOptionName.ReuseAddress,
                                                  true);

                var byteCommand = Encoding.ASCII.GetBytes(command);
                client.Send(byteCommand, byteCommand.GetLength(0), ep);

                System.Console.WriteLine("ACK: OK");

            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error sending command");
                System.Console.WriteLine(e.Message + " " + e.ToString());
            }

        }
    }
}
