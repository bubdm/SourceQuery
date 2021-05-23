using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Checksum;

namespace SourceQuery
{
    /// <summary>
    /// A2SClient options.
    /// </summary>
    public class A2SClientOptions
    {
        public IPAddress Address { get; set; }
        public int Port { get; set; } = 27015;
    }
    
    /// <summary>
    /// Provides communication with a Source Engine server using A2S queries. 
    /// </summary>
    public class A2SClient
    {
        private readonly UdpClient _udpClient;
        private IPEndPoint _endpoint;
        
        private readonly A2SClientOptions _options;

        /// <summary>
        /// A2SClient constructor.
        /// </summary>
        /// <param name="options">Options for the A2SClient</param>
        public A2SClient(A2SClientOptions options)
        {
            _udpClient = new UdpClient();
            _endpoint = new IPEndPoint(options.Address, options.Port);
            _options = options;
        }

        /// <summary>
        /// Connects to the server host.
        /// </summary>
        public async Task Connect()
        {
            _udpClient.Connect(_options.Address, _options.Port);

            var data = new byte[]
            {
                0xff, 0xff, 0xff, 0xff, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75,
                0x65, 0x72, 0x79, 0x00
            };
            await _udpClient.SendAsync(data, data.Length);
            var bytes = await Receive();
            
            foreach (var b in bytes)
                Console.WriteLine($"0x{b:X2}"); 
        }

        /// <summary>
        /// Closes the connection to the server host.
        /// </summary>
        public void Close()
        {
            _udpClient.Close();
        }

        public async Task<byte[]> Receive()
        {
            byte[][] packets = null;
            int packetCount = 1, packetIndex = 0;
            
            bool isCompressed = false;
            int crc = 0;

            do
            {
                // Read the packet the UDP client.
                var result = await _udpClient.ReceiveAsync();
                var reader = new BinaryReader(new MemoryStream(result.Buffer));
                
                // Check if the packet is split.
                if (reader.ReadInt32() == -2)
                {
                    // Check if the request is compressed by masking the first bit.
                    int requestId = reader.ReadInt32();
                    isCompressed = (0x80000000 & requestId) == 0x80000000;

                    // Contains the current packet index, and the number of packets sent.
                    packetCount = reader.ReadByte();
                    packetIndex = reader.ReadByte();

                    if (isCompressed && packetIndex == 0)
                    {
                        reader.ReadInt32();
                        crc = reader.ReadInt32();
                    }
                }

                // Initialize the packets array.
                
                packets ??= new byte[packetCount][];
                
                // Set the current packet.
                packets[packetIndex] = result.Buffer;
            } while (packets.Any(packet => packet == null));
            
            // Combine all the packets into one byte array.
            int size = packets.Sum(packet => packet.Length);
            var data = new byte[size];
            
            var offset = 0;
            foreach (var packet in packets)
            {
                Buffer.BlockCopy(packet, 0, data, offset, packet.Length);
                offset += packet.Length;
            }
            
            // Decompress the data.
            if (isCompressed)
            {
                var compressedStream = new MemoryStream(data);
                var decompressedStream = new MemoryStream();
                
                BZip2.Decompress(compressedStream, decompressedStream, true);
                data = decompressedStream.ToArray();
                
                var crc32 = new Crc32();
                crc32.Update(data);
                if (crc32.Value != crc) throw new Exception("Invalid CRC for compressed packet.");
            }

            return data;
        }
    }
}