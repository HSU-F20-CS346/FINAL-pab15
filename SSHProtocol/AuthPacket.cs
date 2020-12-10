using System;
using System.Collections.Generic;
using System.Text;

namespace SSHProtocol
{
    public class AuthPacket
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public AuthPacket()
        {

        }
        public AuthPacket(string _username, string _password)
        {
            Username = _username;
            Password = _password;
        }
        public AuthPacket(byte[] rawPacket)
        {
            string packet = Encoding.UTF8.GetString(rawPacket);
            string[] splitPacket = packet.Split("\n");
            Username = splitPacket[0];
            Password = splitPacket[1];
        }
        public byte[] GetRawPacket()
        {
            string packet = Username + "\n" + Password;
            return Encoding.UTF8.GetBytes(packet);
        }
    }
}
