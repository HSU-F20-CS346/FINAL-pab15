using System;
using System.IO;
using System.Net;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;
using SSHProtocol;
using System.Threading;
using System.Collections.Generic;

namespace Server
{
    class Server
    {
        public Dictionary<string, Tracker> NonAuthenticatedUsers { get; set; }
        public Dictionary<string, Tracker> AuthenticatedUsers { get; set; }

        public BigInteger P = BigInteger.Parse("B10B8F96A080E01DDE92DE5EAE5D54EC52C99FBCFB06A3C69A6A9DCA52D23B616073E28675A23D189838EF1E2EE652C013ECB4AEA906112324975C3CD49B83BFACCBDD7D90C4BD7098488E9C219A73724EFFD6FAE5644738FAA31A4FF55BCCC0A151AF5F0DC8B4BD45BF37DF365C1A65E68CFDA76D4DA708DF1FB2BC2E4A4371", System.Globalization.NumberStyles.HexNumber);
        public BigInteger G = BigInteger.Parse("A4D1CBD5C3FD34126765A442EFB99905F8104DD258AC507FD6406CFF14266D31266FEA1E5C41564B777E690F5504F213160217B4B01B886A5E91547F9E2749F4D7FBD7D3B9A92EE1909D0D2263F80A76A6A24C087A091F531DBF0A0169B6A28AD662A4D18E73AFA32D779D5918D08BC8858F4DCEF97C2A24855E6EEB22B3B2E5", System.Globalization.NumberStyles.HexNumber);
        private Exchange Exchanger { get; set; }
        private Thread ExchangerThread = null;
        private Authenticator Authenticator { get; set; }
        private Thread AuthThread = null;
        private Reciever Reciever { get; set; }
        private Thread RecieverThread = null;
        public Server(int _ExchangePort, int _AuthPort, int _RecievePort)
        {
            NonAuthenticatedUsers = new Dictionary<string, Tracker>();
            AuthenticatedUsers = new Dictionary<string, Tracker>();
            Exchanger = new Exchange();
            Exchanger.Server = this;
            Exchanger.Port = _ExchangePort;
            Authenticator = new Authenticator();
            Authenticator.Server = this;
            Authenticator.Port = _AuthPort;
            Reciever = new Reciever();
            Reciever.Server = this;
            Reciever.Port = _RecievePort;
        }
        public void Run()
        {
            ThreadStart ExchangerThreadStart = Exchanger.Run;
            ExchangerThread = new Thread(ExchangerThreadStart);
            ExchangerThread.Start();
            ThreadStart AuthThreadStart = Authenticator.Run;
            AuthThread = new Thread(AuthThreadStart);
            AuthThread.Start();
            ThreadStart RecieverThreadStart = Reciever.Run;
            RecieverThread = new Thread(RecieverThreadStart);
            RecieverThread.Start();

        }
        public void PrintMSG(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
