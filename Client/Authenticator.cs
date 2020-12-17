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
                IPEndPoint UserIp = client.Client.RemoteEndPoint as IPEndPoint;
                
                BinaryReader reader = null;
                BinaryWriter writer = null;

                try
                {
                    BufferedStream stream = new BufferedStream(client.GetStream());
                    reader = new BinaryReader(stream);
                    writer = new BinaryWriter(stream);

                    if (Server.AuthenticatedUsers.ContainsKey(UserIp.Address.ToString()))
                    {
                        ResponsePacket Response = new ResponsePacket(1, "You Already Have An Open Connection With This Server...");
                        byte[] EncResponse = Server.AuthenticatedUsers[UserIp.Address.ToString()].KeyManager.Encrypt(Response.GetRawPacket());
                        writer.Write(IPAddress.HostToNetworkOrder(EncResponse.Length));
                        writer.Write(EncResponse);
                        writer.Flush();
                    }
                    else if (!Server.NonAuthenticatedUsers.ContainsKey(UserIp.Address.ToString()))
                    {
                        ResponsePacket Response = new ResponsePacket(1, "Connection Failed...");
                        byte[] EncResponse = Server.AuthenticatedUsers[UserIp.Address.ToString()].KeyManager.Encrypt(Response.GetRawPacket());
                        writer.Write(IPAddress.HostToNetworkOrder(EncResponse.Length));
                        writer.Write(EncResponse);
                        writer.Flush();
                    }
                    else
                    {
                        KeyManager ClientKeyManager = Server.NonAuthenticatedUsers[UserIp.Address.ToString()].KeyManager;

                        ResponsePacket Response = new ResponsePacket(1, "Connection Successful...");
                        byte[] EncResponse = ClientKeyManager.Encrypt(Response.GetRawPacket());
                        writer.Write(IPAddress.HostToNetworkOrder(EncResponse.Length));
                        writer.Write(EncResponse);
                        writer.Flush();

                        // Read Auth Packet
                        int AuthPacketLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                        byte[] AuthPacketBytes = reader.ReadBytes(AuthPacketLength);

                        // Calculate New Aes IV

                        // Decrypt Auth Packet Using Shared Key And Aes IV
                        byte[] DeCyrptedAuthPacketBytes = ClientKeyManager.Decrypt(AuthPacketBytes);
                        AuthPacket AuthPacket = new AuthPacket(DeCyrptedAuthPacketBytes);

                        if (AuthPacket.Username == "PeterB" && AuthPacket.Password == "Password123")
                        {
                            Response = new ResponsePacket(1, "Login Successful...");
                            EncResponse = ClientKeyManager.Encrypt(Response.GetRawPacket());
                            writer.Write(IPAddress.HostToNetworkOrder(EncResponse.Length));
                            writer.Write(EncResponse);
                            writer.Flush();
                            Console.WriteLine("Login Valid");
                            Server.AuthenticatedUsers.Add(UserIp.Address.ToString(), Server.NonAuthenticatedUsers[UserIp.Address.ToString()]);
                        }
                        else
                        {
                            Response = new ResponsePacket(1, "Login Denied...");
                            EncResponse = ClientKeyManager.Encrypt(Response.GetRawPacket());
                            writer.Write(IPAddress.HostToNetworkOrder(EncResponse.Length));
                            writer.Write(EncResponse);
                            writer.Flush();
                            Console.WriteLine("Login Invalid");
                        }

                    }
                }
                catch (Exception ex)
                {
                    Server.PrintMSG("Error Creating Auth Stream...");
                }

            }
        }
    }
}
