using System;
using System.IO;
using System.Net;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;

namespace SSHProtocol
{
    public class InitPacket
    {
        public byte PacketValue = 00000000;
        public int IVSize = 16;
        public byte[] AesIV { get; set; }

        public InitPacket()
        {
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            AesIV = new byte[IVSize];
            randomNumberGenerator.GetBytes(AesIV);
            randomNumberGenerator.Dispose();
        }
        public InitPacket(byte[] rawData)
        {
            if (rawData[0] == 00000000)
            {
                IVSize = rawData.Length - 1;
                AesIV = new byte[IVSize];
                for (int i = 0; i < IVSize; i++)
                {
                    AesIV[i] = rawData[i + 1];
                }
            }
            else throw new InvalidDataException();

        }
        public byte[] GetRawPacket()
        {
            byte[] rawBytes = new byte[1 + IVSize];
            rawBytes[0] = PacketValue;
            for(int i = 0; i < IVSize; i++)
            {
                rawBytes[i + 1] = AesIV[i];
            }
            return rawBytes;
        }
    }
}
