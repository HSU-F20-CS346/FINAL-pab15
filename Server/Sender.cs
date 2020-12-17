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
    class Sender
    {
        public string DestinationAddress { get; set; }
        public int Port { get; set; }
        public Client Client { get; set; }

        public Sender (string _DestinationAddress, int _Port, string _command, Client _client)
        {
            DestinationAddress = _DestinationAddress;
            Port = _Port;
            Client = _client;
            Send(_command);
        }

        public void Send(string ToSend)
        {
            try
            {
                TcpClient client = new TcpClient(DestinationAddress, Port);

                using (BufferedStream stream = new BufferedStream(client.GetStream()))
                {
                    BinaryReader reader = new BinaryReader(stream);
                    BinaryWriter writer = new BinaryWriter(stream);

                    int ValidatorPacketLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                    byte[] ValidatorBytes = reader.ReadBytes(ValidatorPacketLength);
                    ResponsePacket ValidatorPacket = new ResponsePacket(Client.Tracker.KeyManager.Decrypt(ValidatorBytes));
                    if (ValidatorPacket.Data == "Access Granted...")
                    {
                        // Write Our Encrypted Command Packet
                        CommandPacket CmdPacket = new CommandPacket(3, ToSend);
                        byte[] CmdPacketBytes = Client.Tracker.KeyManager.Encrypt(CmdPacket.getRawPacket());
                        writer.Write(IPAddress.HostToNetworkOrder(CmdPacketBytes.Length));
                        writer.Write(CmdPacketBytes);
                        writer.Flush();

                        // Recieve Response
                        int ResponsePacketLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                        ResponsePacket ResponsePacket = new ResponsePacket(Client.Tracker.KeyManager.Decrypt(reader.ReadBytes(ResponsePacketLength)));
                        Client.ClientMSG(ResponsePacket.Data);
                    }
                    else
                    {
                        Client.ClientMSG(ValidatorPacket.Data);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
