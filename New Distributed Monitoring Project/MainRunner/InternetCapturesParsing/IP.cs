using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Utils.TypeUtils;

namespace InternetCapturesParsing
{
    [StructLayout(LayoutKind.Explicit)]
    public struct IP
    {
        [FieldOffset(0)] public int  Address;
        [FieldOffset(0)] public byte Byte0;
        [FieldOffset(1)] public byte Byte1;
        [FieldOffset(2)] public byte Byte2;
        [FieldOffset(3)] public byte Byte3;

        public IP(byte byte0, byte byte1, byte byte2, byte byte3) : this()
        {
            Byte0 = byte0;
            Byte1 = byte1;
            Byte2 = byte2;
            Byte3 = byte3;
        }
        public IP(int address) : this()
        {
            Address = address;
        }

        public void WriteBinary(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Address);
        }
        public static IP FromBinary(BinaryReader binaryReader)
        {
            return new IP(binaryReader.ReadInt32());
        }

        public static Maybe<IP> TryParse(string ip)
        {
            var tokens = ip.Split(".".ToCharArray());
            if (tokens.Length != 4)
                return Maybe.None<IP>();

            return tokens[0].TryParseByte()
              .Bind(byte0 => tokens[1].TryParseByte()
              .Bind(byte1 => tokens[2].TryParseByte()
              .Bind(byte2 => tokens[3].TryParseByte()
              .Map(byte3 => new IP(byte0, byte1,byte2,byte3)))));
        }
    }
}