using System;
using System.Linq;

namespace Nats.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Configuration
            {
                Host = "127.0.0.1",
                Port = 4222
            };

            var server = new NatsServer(config);
            server.Start();

            Console.ReadLine();
        }
    }
}
