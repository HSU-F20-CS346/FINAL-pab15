using System;
using System.Numerics;
using System.Text;
using System.Security.Cryptography;
using SSHProtocol;

namespace TestSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            BigInteger p = BigInteger.Parse("B10B8F96A080E01DDE92DE5EAE5D54EC52C99FBCFB06A3C69A6A9DCA52D23B616073E28675A23D189838EF1E2EE652C013ECB4AEA906112324975C3CD49B83BFACCBDD7D90C4BD7098488E9C219A73724EFFD6FAE5644738FAA31A4FF55BCCC0A151AF5F0DC8B4BD45BF37DF365C1A65E68CFDA76D4DA708DF1FB2BC2E4A4371", System.Globalization.NumberStyles.HexNumber);
            BigInteger g = BigInteger.Parse("A4D1CBD5C3FD34126765A442EFB99905F8104DD258AC507FD6406CFF14266D31266FEA1E5C41564B777E690F5504F213160217B4B01B886A5E91547F9E2749F4D7FBD7D3B9A92EE1909D0D2263F80A76A6A24C087A091F531DBF0A0169B6A28AD662A4D18E73AFA32D779D5918D08BC8858F4DCEF97C2A24855E6EEB22B3B2E5", System.Globalization.NumberStyles.HexNumber);
            SSHProtocol.KeyManager client = new KeyManager(p, g);
            SSHProtocol.KeyManager server = new KeyManager(p, g);
            client.HostGeneratedKey = server.PublicGeneratedKey;
            server.HostGeneratedKey = client.PublicGeneratedKey;
            server.ComputeSharedKey();
            client.ComputeSharedKey();
            InitPacket IV = new InitPacket();
            server.AesIV = IV.AesIV;
            client.AesIV = IV.AesIV;
            byte[] msg = Encoding.UTF8.GetBytes("Hello World");
            msg = server.Encrypt(msg);
            byte[] demsg = client.Decrypt(msg);
            string d = Encoding.UTF8.GetString(demsg);
            Console.WriteLine(d);
        }
    }
}
