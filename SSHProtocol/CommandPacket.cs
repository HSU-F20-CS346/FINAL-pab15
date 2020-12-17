using System;
using System.Collections.Generic;
using System.Text;

namespace SSHProtocol
{
    public class CommandPacket
    {
        public int Code { get; set; }
        public string Command { get; set; }
        public CommandPacket()
        {

        }
        public CommandPacket(int _code, string _command)
        {
            Code = _code;
            Command = _command;
        }
        public CommandPacket(byte[] rawPacket)
        {
            string rawData = Encoding.UTF8.GetString(rawPacket);
            string[] splitData = rawData.Split("\n");
            Code = Int32.Parse(splitData[0]);
            if (Code == 3)
            {
                if (splitData.Length > 2)
                {
                    for (int i = 1; i < splitData.Length; i++)
                    {
                        Command = Command + "\n" + splitData[i];
                    }
                }
                else
                {
                    Command = splitData[1];
                }
            }
        }
        public byte[] getRawPacket()
        {
            return Encoding.UTF8.GetBytes(Code.ToString() + "\n" + Command);
        }
    }
}
