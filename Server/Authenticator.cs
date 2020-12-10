using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using SSHProtocol;

namespace Client
{
    class Authenticator
    {
        public string DestinationAddress { get; set; }
        public int Port { get; set; }
        public Client Client { get; set; }

        public Authenticator (string _DestinationAddress, int _Port)
        {
            DestinationAddress = _DestinationAddress;
            Port = _Port;
        }

        public void Authenticate()
        {
            KeyValuePair<string, string> userLogin = Client.GetLogin();

            try
            {
                TcpClient client = new TcpClient(DestinationAddress, Port);

                using (BufferedStream stream = new BufferedStream(client.GetStream()))
                {
                    BinaryReader reader = new BinaryReader(stream);
                    BinaryWriter writer = new BinaryWriter(stream);

                    AuthPacket Auth = new AuthPacket(userLogin.Key, userLogin.Value);

                    // Write Our Encrypted Auth Packet
                    byte[] AuthBytes = Client.Tracker.KeyManager.Encrypt(Auth.GetRawPacket());
                    writer.Write(IPAddress.HostToNetworkOrder(AuthBytes.Length));
                    writer.Write(AuthBytes);
                    writer.Flush();

                    // Recieve Response
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
