using System.IO;
using SourceQuery.Extensions;

namespace SourceQuery.Response
{
    /// <summary>
    /// Represents the A2S_INFO query response.
    /// </summary>
    public class InfoResponse
    {
        public byte Protocol { get; }

        public string Name { get; }

        public string Map { get; }

        public string Folder { get; }

        public string Game { get; }

        public short GameId { get; }

        public byte Players { get; }

        public byte MaxPlayers { get; }

        public byte Bots { get; }

        public byte ServerType { get; }

        public byte Environment { get; }

        public byte Visibility { get; }

        public byte Vac { get; }

        public string Version { get; }

        /// <summary>
        /// InfoQuery constructor.
        /// </summary>
        /// <param name="data">The raw data of the response.</param>
        public InfoResponse(byte[] data)
        {
            var reader = new BinaryReader(new MemoryStream(data));

            // Read the header.
            reader.ReadByte();

            // Read the protocol.
            Protocol = reader.ReadByte();
            
            // Read the name, map, folder, and game.
            Name = reader.ReadUtf8String();
            Map = reader.ReadUtf8String();
            Folder = reader.ReadUtf8String();
            Game = reader.ReadUtf8String();
            
            // Read the game ID.
            GameId = reader.ReadInt16();
            
            // Read the players, max players, and bots.
            Players = reader.ReadByte();
            MaxPlayers = reader.ReadByte();
            Bots = reader.ReadByte();
            
            // Read the server type, environment, visibility, and vac mode.
            ServerType = reader.ReadByte();
            Environment = reader.ReadByte();
            Visibility = reader.ReadByte();
            Vac = reader.ReadByte();
            
            // Read the version string.
            Version = reader.ReadUtf8String();
        }
    }
}