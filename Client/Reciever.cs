using SSHProtocol;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;

namespace Server
{
    class Reciever
    {
        public int Port { get; set; }
        public bool Running { get; set; }
        public Server Server { get; set; }

        public Reciever()
        {

        }
        public void Run()
        {
            Running = true;
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            while (Running)
            {
                Server.PrintMSG(("Waiting For Commands Via Port " + Port.ToString() + "..."));
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
                    Server.PrintMSG(("Error Accepting Command..." + ex.Message + "..."));
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
                        ResponsePacket Access = new ResponsePacket(1, "Access Granted...");
                        byte[] EncAccess = Server.AuthenticatedUsers[UserIp.Address.ToString()].KeyManager.Encrypt(Access.GetRawPacket());
                        writer.Write(IPAddress.HostToNetworkOrder(EncAccess.Length));
                        writer.Write(EncAccess);
                        writer.Flush();

                        KeyManager ClientKeyManager = Server.AuthenticatedUsers[UserIp.Address.ToString()].KeyManager;

                        // Read Command
                        int CommandPacketLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                        byte[] CommandPacketBytes = reader.ReadBytes(CommandPacketLength);
                        CommandPacket Command = new CommandPacket(ClientKeyManager.Decrypt(CommandPacketBytes));

                        // Execute Command
                        string CommandResult = ExecuteCommand(Command.Command);

                        // Send Response
                        ResponsePacket Response = new ResponsePacket(1, CommandResult);
                        byte[] EncResponse = ClientKeyManager.Encrypt(Response.GetRawPacket());
                        writer.Write(IPAddress.HostToNetworkOrder(EncResponse.Length));
                        writer.Write(EncResponse);
                        writer.Flush();
                    }
                    else
                    {
                        ResponsePacket Response = new ResponsePacket(1, "Access Denied...");
                        byte[] EncResponse = Server.AuthenticatedUsers[UserIp.Address.ToString()].KeyManager.Encrypt(Response.GetRawPacket());
                        writer.Write(IPAddress.HostToNetworkOrder(EncResponse.Length));
                        writer.Write(EncResponse);
                        writer.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Server.PrintMSG("Error Reading Command Stream...");
                }

            }
        }
        public string ExecuteCommand(string Command)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "powershell.exe";
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.StandardInput.WriteLine(Command);
                proc.StandardInput.Flush();
                proc.StandardInput.Close();
                proc.WaitForExit();
                return proc.StandardOutput.ReadToEnd();
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
