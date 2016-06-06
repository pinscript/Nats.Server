using System;

namespace Nats.Server
{
    class Configuration
    {
        public string ServerId => Guid.NewGuid().ToString();
        public Version Version => new Version("0.0.1");
        public string Host { get; set; }
        public int Port { get; set; }
    }
}