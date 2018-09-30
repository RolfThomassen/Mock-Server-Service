using System;
using System.Text;

namespace Socket.Shared.Code
{
    public class PackageBuilder
    {
        private byte[] buffer = new byte[] { };

        public PackageBuilder appendBytes(byte[] bytes)
        {
            int i = buffer.Length;
            Array.Resize<byte>(ref buffer, i + bytes.Length);
            bytes.CopyTo(buffer, i);

            return this;
        }

        public PackageBuilder appendBytesValue(byte[] bytes)
        {
            int i = buffer.Length;
            Array.Resize<byte>(ref buffer, i + bytes.Length + 4);

            byte[] lengthBytes = BitConverter.GetBytes(bytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBytes);

            lengthBytes.CopyTo(buffer, i);
            bytes.CopyTo(buffer, i + 4);

            return this;
        }


        public PackageBuilder appendInt(int value)
        {
            byte[] lengthBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBytes);

            return appendBytes(lengthBytes);
        }

        public PackageBuilder appendIntValue(int value)
        {
            byte[] lengthBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBytes);

            return appendBytesValue(lengthBytes);
        }

        public PackageBuilder appendString(String value)
        {
            return appendBytes(Encoding.UTF8.GetBytes(value));
        }

        public PackageBuilder appendStringValue(String value)
        {
            return appendBytesValue(Encoding.UTF8.GetBytes(value));
        }

        public byte[] Build()
        {
            byte[] result = new byte[4 + buffer.Length];

            byte[] lengthBytes = BitConverter.GetBytes(buffer.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBytes);

            lengthBytes.CopyTo(result, 0);
            buffer.CopyTo(result, 4);

            return result;
        }
    }
}
