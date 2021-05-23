using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SourceQuery.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static string ReadUtf8String(this BinaryReader reader)
        {
            var bytes = new List<byte>();
            byte currentByte;

            while ((currentByte = reader.ReadByte()) != 0)
                bytes.Add(currentByte);

            return Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}