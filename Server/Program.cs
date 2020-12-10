using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("127.0.0.1", 3500, 3501);
            if (client.Exchanger.InitiateExchange() == true)
                client.Authenticator.Authenticate();
            else
            {
                Console.WriteLine("Error Exchanging Keys...");
            }
        }
    }
}
