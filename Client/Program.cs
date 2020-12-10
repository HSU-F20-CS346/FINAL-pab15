using System;
using System.IO;
using System.Net;
using System.Text;
using System.Numerics;
using System.Threading;
using System.Security.Cryptography;
using SSHProtocol;

namespace Server
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Server server = new Server(3500, 3501);
            server.Run();

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
