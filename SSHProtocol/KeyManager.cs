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
        public BigInteger PublicGeneratedKey { get; set; }
        public BigInteger HostGeneratedKey { get; set; }
        public BigInteger SharedSecretKey { get; set; }
        public byte[] AesIV { get; set; }
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
            PublicGeneratedKey = (BigInteger.ModPow(PublicKeyG, UserPrivateKey, PublicKeyP));
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

        public byte[] Encrypt(string msg)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SharedSecretKey.ToString());
                aes.IV = AesIV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using(StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(msg);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }
        public byte[] Decrypt(string msg)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SharedSecretKey.ToString());
                aes.IV = AesIV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swDecrypt = new StreamWriter(csDecrypt))
                        {
                            swDecrypt.Write(msg);
                        }
                        return msDecrypt.ToArray();
                    }
                }
            }
        }
    }
}
