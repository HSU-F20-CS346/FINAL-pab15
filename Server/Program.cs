using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("127.0.0.1", 3500);
            client.Exchanger.InitiateExchange();
        }
    }
}
