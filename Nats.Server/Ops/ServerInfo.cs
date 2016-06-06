using Newtonsoft.Json;

namespace Nats.Server.Ops
{
    internal class ServerInfo : IOp
    {
        private readonly Configuration _config;

        public ServerInfo(Configuration config)
        {
            _config = config;
        }

        public string ToProtocolString()
        {
            var json = JsonConvert.SerializeObject(new
            {
                server_id = _config.ServerId,
                version = _config.Version.ToString(),
                host = _config.Host,
                port = _config.Port
            }, Formatting.None);


            return "INFO " + json;
        }
    }
}