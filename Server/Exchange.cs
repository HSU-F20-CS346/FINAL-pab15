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
    class Exchange
    {
        public string DestinationAddress { get; set; }
        public int Port { get; set; }
        public Client Client { get; set; }

        public Exchange (string _DestinationAddress, int _Port)
        {
            DestinationAddress = _DestinationAddress;
            Port = _Port;
        }

        public void InitiateExchange()
        {
            try
            {
                TcpClient client = new TcpClient(DestinationAddress, Port);

                using (BufferedStream stream = new BufferedStream(client.GetStream()))
                {
                    BinaryReader reader = new BinaryReader(stream);
                    BinaryWriter writer = new BinaryWriter(stream);

                    // Write Our Partial Key
                    byte[] PubGenKey = Encoding.UTF8.GetBytes(Client.ClientKeyManager.PublicGeneratedKey.ToString());
                    writer.Write(IPAddress.HostToNetworkOrder(PubGenKey.Length));
                    writer.Write(PubGenKey);
                    writer.Flush();

                    // Read Server Partial Key
                    int ServerKeyLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    byte[] ServerKey = reader.ReadBytes(ServerKeyLength);

                    // Generate Shared Key
                    Client.ClientKeyManager.HostGeneratedKey = BigInteger.Parse(Encoding.UTF8.GetString(ServerKey));
                    Client.ClientKeyManager.ComputeSharedKey();
                    Client.ClientMSG(Client.ClientKeyManager.SharedSecretKey.ToString() + "\n\n");


                    // Get Init Packet And Print To AesIV Screen
                    int PacketBytesLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    byte[] PacketBytes = reader.ReadBytes(PacketBytesLength);
                    InitPacket initPacket = new InitPacket(PacketBytes);
                    Client.ClientKeyManager.AesIV = initPacket.AesIV;
                    Client.ClientMSG(Encoding.UTF8.GetString(initPacket.AesIV));
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
