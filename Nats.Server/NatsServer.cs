using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Nats.Server.Ops;

namespace Nats.Server
{
    internal class NatsServer
    {
        private readonly Configuration _config;
        private readonly TcpListener _tcpListener;
        private readonly List<NatsClient> _clients; 


        public NatsServer(Configuration config)
        {
            _config = config;
            _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Parse(config.Host), config.Port));
            _clients = new List<NatsClient>();
        }


        public async void Start()
        {
            _tcpListener.Start();

            while (true)
            {
                Console.WriteLine("Waiting for a new client ..");
                var client = await _tcpListener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected ..");
                var natsClient = new NatsClient(client);
                _clients.Add(natsClient);

                natsClient.EnqueueSend(new ServerInfo(_config));
            }
        }
    }
}