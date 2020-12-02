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
    class Authenticator
    {
        public int Port { get; set; }
        public bool Running { get; set; }
        public Server Server { get; set; }

        public Authenticator()
        {

        }

        public void Run()
        {
            Running = true;
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            while (Running)
            {
                Server.PrintMSG(("Waiting For Authentication Via Port " + Port.ToString() + "..."));
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
                    Server.PrintMSG(("Error Authenticating Client " + ex.Message + "..."));
                }

                Server.PrintMSG("Client Connection Successful From: " + client.Client.RemoteEndPoint + "...");

                BinaryReader reader = null;
                BinaryWriter writer = null;

                try
                {
                    KeyManager ClientKeyManager = Server.NonAuthenticatedUsers[client.Client.RemoteEndPoint.ToString()];
                    BufferedStream stream = new BufferedStream(client.GetStream());
                    reader = new BinaryReader(stream);
                    writer = new BinaryWriter(stream);

                    // Read Auth Packet
                    int AuthPacketLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    byte[] AuthPacketBytes = reader.ReadBytes(AuthPacketLength);

                    // Calculate New Aes IV

                    // Decrypt Auth Packet Using Shared Key And Aes IV

                    // Translate Bytes To and AuthPacket()

                    // Get Username and Password And Validate

                }
                catch (Exception ex)
                {
                    Server.PrintMSG("Error Creating Stream...");
                }
            }
        }
    }
}
