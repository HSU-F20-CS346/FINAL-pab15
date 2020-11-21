using System;
using System.IO;
using System.Net;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;

namespace SSHProtocol
{
    public class KeyManager
    {
        public BigInteger PublicKeyP { get; set; }
        public BigInteger PublicKeyG { get; set; }
        public BigInteger UserPrivateKey { get; set; }
        public BigInteger PublicGenratedKey { get; set; }
        public BigInteger HostGeneratedKey { get; set; }
        public BigInteger SharedSecretKey { get; set; }
        public KeyManager()
        {

        }
        public KeyManager(BigInteger _publicKeyA, BigInteger _publicKeyB)
        {
            PublicKeyP = BigInteger.Abs(_publicKeyA);
            PublicKeyG = BigInteger.Abs(_publicKeyB);
            GenerateUserPrivateKey();
            ComputePublicGenKeyForServer();
        }
        private void ComputePublicGenKeyForServer()
        {
            PublicGenratedKey = (BigInteger.ModPow(PublicKeyG, UserPrivateKey, PublicKeyP));
        }
        public void ComputeSharedKey()
        {
            SharedSecretKey = (BigInteger.ModPow(HostGeneratedKey, UserPrivateKey, PublicKeyP));
        }
        public void GenerateUserPrivateKey()
        {
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            byte[] userPrivateKey = new byte[128];
            randomNumberGenerator.GetBytes(userPrivateKey);
            UserPrivateKey = BigInteger.Abs(BigInteger.Parse(ByteArrayToHexString(userPrivateKey), System.Globalization.NumberStyles.HexNumber));
            randomNumberGenerator.Dispose();
        }
        public string ByteArrayToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            string hexChars = "0123456789ABCDEF";
            for (int i = 0; i < array.Length; i++)
            {
                hex.Append(hexChars[(int)(array[i] >> 4)]);
                hex.Append(hexChars[(int)(array[i] & 0xF)]);
            }
            return hex.ToString();
        }
    }
}
