using System;
using System.Collections.Generic;
using System.Text;

namespace SSHProtocol
{
    public class ResponsePacket
    {
        public int Code { get; set; }
        public string Data { get; set; }
        public ResponsePacket()
        {

        }
        public ResponsePacket(byte[] data)
        {
            string rawData = Encoding.UTF8.GetString(data);
            string[] splitData = rawData.Split("\n");
            Code = Int32.Parse(splitData[0]);
            if (Code == 1)
            {
                if (splitData.Length > 2)
                {
                    for (int i = 1; i < splitData.Length; i++)
                    {
                        Data = Data + "\n" + splitData[i];
                    }
                }
                else
                {
                    Data = splitData[1];
                }
            }
        }
        public ResponsePacket(int _Code, string _Data)
        {
            Code = _Code;
            Data = _Data;
        }
        public virtual byte[] GetRawPacket()
        {
            return Encoding.UTF8.GetBytes(Code.ToString() + "\n" + Data);
        }
    }
}
