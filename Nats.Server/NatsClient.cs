using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Nats.Server
{
    internal class NatsClient
    {
        private readonly TcpClient _client;

        private readonly Task _readLoop;
        private readonly Task _writeLoop;
        private Queue<IOp> _writeQueue;


        private readonly CancellationToken _cancellationToken;

        public NatsClient(TcpClient client)
        {
            _client = client;
            _cancellationToken = new CancellationToken(false);
            
            _readLoop = Task.Factory.StartNew(ReadLoop, _cancellationToken);
            _writeLoop = Task.Factory.StartNew(WriteLoop, _cancellationToken);
            _writeQueue = new Queue<IOp>();
        }

        private async void WriteLoop()
        {
            // For now, easier debugging
            while (!_cancellationToken.IsCancellationRequested)
            {
                if (_writeQueue.Count > 0)
                {
                    var stream = _client.GetStream();
                    var op = _writeQueue.Dequeue();
                    var message = op.ToProtocolString();
                    var buffer = Global.Encode(message + Global.CrLf);

                    await stream.WriteAsync(buffer, 0, buffer.Length, _cancellationToken);
                }
                else
                {
                    await Task.Delay(500);
                }
            }
        }

        private void ReadLoop()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var stream = _client.GetStream();

                var buffer = new CircularBuffer(128);

                while (true)
                {
                    Console.Write("?");
                    var chr = (byte)stream.ReadByte();

                    if (buffer.Length > 0 && buffer.HasSuffix(Global.CrLfBytes))
                    {
                        // Got message
                        Console.WriteLine("Got message");
                        var msg = Global.Decode(buffer.Buffer);
                    }
                    else
                    {
                        buffer.Append(chr);
                    }
                }
            }
        }

        public void EnqueueSend(IOp op)
        {
            _writeQueue.Enqueue(op);
        }
    }

    internal class CircularBuffer
    {
        private byte[] _buffer;

        public int Size { get; }
        public int Length { get; private set; }
        public byte[] Buffer => _buffer.Take(Length).ToArray();

        public CircularBuffer(int size)
        {
            Size = size;
            _buffer = new byte[size];
        }

        public void Append(byte chr)
        {
            _buffer[Length++] = chr;
        }

        public bool HasSuffix(byte[] suffix)
        {
            return _buffer.Skip(Length - suffix.Length)
                        .Take(suffix.Length)
                        .ToArray()
                        .SequenceEqual(suffix);
        }
    }
}