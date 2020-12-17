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
        public byte[] AesResizedKey { get; set; }
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
            ResizeSharedKey();
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
        public void RecalculateIV(int TransmissionNum)
        {
            // Modify
            AesIV = AesIV;
        }
        public static byte[] HexStringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public void ResizeSharedKey()
        {
            byte[] oldkey = Encoding.UTF8.GetBytes(SharedSecretKey.ToString());
            if (oldkey.Length > 32)
            {
                byte[] newkey = new byte[32];
                for (int i = 0; i < newkey.Length; i++)
                {
                    newkey[i] = oldkey[i];
                }
                AesResizedKey = newkey;
            }
        }

        public byte[] Encrypt(byte[] msg)
        {
            byte[] encmsg = null;

            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.Key = AesResizedKey;
                aes.IV = AesIV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    encmsg = encryptor.TransformFinalBlock(msg, 0, msg.Length);
                }
            }
            return encmsg;
        }
        public byte[] Decrypt(byte[] msg)
        {

            byte[] decmsg = null;

            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.Key = AesResizedKey;
                aes.IV = AesIV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    decmsg = decryptor.TransformFinalBlock(msg, 0, msg.Length);
                }
            }
            return decmsg;
        }
    }
}
