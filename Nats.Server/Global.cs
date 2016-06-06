using System.Text;

namespace Nats.Server
{
    public class Global
    {
        public static string CrLf = "\r\n";
        public static byte[] CrLfBytes = {(byte)'\r', (byte)'\n'};

        public static byte[] Encode(string text) => Encoding.UTF8.GetBytes(text);
        public static string Decode(byte[] buffer) => Encoding.UTF8.GetString(buffer);
    }
}