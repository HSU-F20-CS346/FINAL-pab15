﻿using System;
using System.Text;
using System.Numerics;
using System.Collections.Generic;
using SSHProtocol;

namespace Client
{
    class Client
    {
        private BigInteger P = BigInteger.Parse("B10B8F96A080E01DDE92DE5EAE5D54EC52C99FBCFB06A3C69A6A9DCA52D23B616073E28675A23D189838EF1E2EE652C013ECB4AEA906112324975C3CD49B83BFACCBDD7D90C4BD7098488E9C219A73724EFFD6FAE5644738FAA31A4FF55BCCC0A151AF5F0DC8B4BD45BF37DF365C1A65E68CFDA76D4DA708DF1FB2BC2E4A4371", System.Globalization.NumberStyles.HexNumber);
        private BigInteger G = BigInteger.Parse("A4D1CBD5C3FD34126765A442EFB99905F8104DD258AC507FD6406CFF14266D31266FEA1E5C41564B777E690F5504F213160217B4B01B886A5E91547F9E2749F4D7FBD7D3B9A92EE1909D0D2263F80A76A6A24C087A091F531DBF0A0169B6A28AD662A4D18E73AFA32D779D5918D08BC8858F4DCEF97C2A24855E6EEB22B3B2E5", System.Globalization.NumberStyles.HexNumber);
        public Tracker Tracker { get; set; }
        public Exchange Exchanger { get; set; }
        public Authenticator Authenticator {get; set;}
        public string DestinationAddress { get; set; }
        public bool Authenticated { get; set; }
        public Client(string DestinationAddress, int ExchangePort, int AuthPort)
        {
            Tracker = new Tracker(new KeyManager(P, G));
            Exchanger = new Exchange(DestinationAddress, ExchangePort);
            Authenticator = new Authenticator(DestinationAddress, AuthPort);
            Exchanger.Client = this;
            Authenticator.Client = this;
            Authenticated = false;
        }
        public void ClientMSG(string msg)
        {
            Console.WriteLine(msg);
        }
        public KeyValuePair<string, string> GetLogin()
        {
            Console.WriteLine("Enter Your Username:");
            string username = Console.ReadLine();
            Console.WriteLine("Enter Your Password:");
            var password = "";
            ConsoleKeyInfo ch = Console.ReadKey(true);
            while(ch.Key != ConsoleKey.Enter)
            {
                if (ch.Key == ConsoleKey.Backspace)
                {
                    password = password.Substring(0, password.Length - 1);
                }
                else
                {
                    password += ch.KeyChar;
                }
                ch = Console.ReadKey(true);
            }
            return new KeyValuePair<string, string>(username, password);
        }
    }
}
