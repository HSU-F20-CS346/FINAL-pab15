using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("127.0.0.1", 3500, 3501);
            if (client.Exchanger.InitiateExchange() == true)
            {
                while (client.Authenticated == false)
                {
                    client.Authenticator.Authenticate();
                }
                client.ClientMSG("\nAuthenticated Successfully, Openning Session...");
                bool runcommands = true;
                while (runcommands)
                {
                    Console.Write(">> ");
                    string command = Console.ReadLine();
                    Sender sender = new Sender("127.0.0.1", 3502, command, client);
                }
            }
            else
            {
                Console.WriteLine("Error Exchanging Keys...");
            }
        }
    }
}
