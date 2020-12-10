using SSHProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;

namespace Server
{
    class Exchange
    {
        public int Port { get; set; }
        public bool Running { get; set; }
        public Server Server { get; set; }

        public Exchange()
        {

        }

        public void Run()
        {
            Running = true;
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            while (Running)
            {
                Server.PrintMSG(("Waiting For Exchange Via Port " + Port.ToString() + "..."));
                TcpClient client = null;
                try
                {
                    client = listener.AcceptTcpClient();
#if DEBUG == false
                    client.ReceiveTimeout = 5000;
#endif

                }
                catch (Exception ex)
                {
                    Server.PrintMSG(("Error Accepting Client " + ex.Message + "..."));
                }

                Server.PrintMSG("Client Connection Successful From: " + client.Client.RemoteEndPoint + "...");

                BinaryReader reader = null;
                BinaryWriter writer = null;

                try
                {
                    KeyManager ClientKeyManager = new KeyManager(Server.P, Server.G);
                    BufferedStream stream = new BufferedStream(client.GetStream());
                    reader = new BinaryReader(stream);
                    writer = new BinaryWriter(stream);

                    // Read Client Partial Key 
                    int ClientKeyLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    byte[] ClientKeyBytes = reader.ReadBytes(ClientKeyLength);

                    // Send Our Partial Key
                    byte[] PubGenKey = Encoding.UTF8.GetBytes(ClientKeyManager.PublicGeneratedKey.ToString());
                    writer.Write(IPAddress.HostToNetworkOrder(PubGenKey.Length));
                    writer.Write(PubGenKey);
                    writer.Flush();

                    // Generate Shared Key
                    ClientKeyManager.HostGeneratedKey = BigInteger.Parse(Encoding.UTF8.GetString(ClientKeyBytes));
                    ClientKeyManager.ComputeSharedKey();
                    Server.PrintMSG(ClientKeyManager.SharedSecretKey.ToString() + "\n\n");

                    // Send Init Packet With Aes IV
                    InitPacket initializer = new InitPacket();
                    ClientKeyManager.AesIV = initializer.AesIV;
                    writer.Write(IPAddress.HostToNetworkOrder(initializer.GetRawPacket().Length));
                    writer.Write(initializer.GetRawPacket());
                    writer.Flush();
                    Server.PrintMSG(Encoding.UTF8.GetString(initializer.AesIV));


                    if (Server.NonAuthenticatedUsers.ContainsKey(client.Client.RemoteEndPoint.ToString()))
                    {
                        IPEndPoint UserIp = client.Client.RemoteEndPoint as IPEndPoint;
                        Server.NonAuthenticatedUsers[UserIp.Address.ToString()] = new Tracker(ClientKeyManager);
                    }
                    else
                    {
                        IPEndPoint UserIp = client.Client.RemoteEndPoint as IPEndPoint;
                        Server.NonAuthenticatedUsers.Add(UserIp.Address.ToString(), new Tracker(ClientKeyManager));
                    }
                }
                catch (Exception ex)
                {
                    Server.PrintMSG("Error Creating Stream...");
                }
            }
        }
    }
}
