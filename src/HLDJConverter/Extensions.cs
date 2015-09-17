using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLDJConverter
{
    public static class MemoryStreamExtensions
    {
        public static byte[] ReadBytes(this MemoryStream stream, int offset, int count)
        {
            byte[] data = new byte[count];
            stream.Read(data, offset, count);
            return data;
        }
    }

    public static class FileStreamExtensions
    {
        public static string ReadString(this FileStream stream, int length)
        {
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }

        public static int ReadInt(this FileStream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static uint ReadUnsignedInt(this FileStream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static short ReadShort(this FileStream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }
    }
}
