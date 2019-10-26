using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utils.TypeUtils;

namespace InternetCapturesParsing
{
    public sealed class InternetCaptureEntry
    {
        public IP SourceIP { get; }
        public IP DestinationIP { get; }
        public DateTime Timestamp { get; }

        public InternetCaptureEntry(IP sourceIp, IP destinationIp, DateTime timestamp)
        {
            SourceIP = sourceIp;
            DestinationIP = destinationIp;
            Timestamp = timestamp;
        }

        public static InternetCaptureEntry SingleFromBinary(BinaryReader reader)
        {
            var sourceIp = IP.FromBinary(reader);
            var destionationIp = IP.FromBinary(reader);
            var timestamp = DateTime.FromBinary(reader.ReadInt64());
            return new InternetCaptureEntry(sourceIp, destionationIp, timestamp);
        }

        public static IEnumerable<InternetCaptureEntry> FromBinary(BinaryReader reader)
        {
            while (reader.IsEOF() == false)
                yield return SingleFromBinary(reader);
        }

        public void ToBinary(BinaryWriter writer)
        {
            SourceIP.WriteBinary(writer);
            DestinationIP.WriteBinary(writer);
            writer.Write(Timestamp.Ticks);
        }

        public static Maybe<InternetCaptureEntry> Parse(string csvLine)
        {
            var tokens = csvLine.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 8)
                return Maybe.None<InternetCaptureEntry>();

            return
                tokens[0].TryParseDateTime()
                    .Bind(timestamp => IP.TryParse(tokens[3])
                    .Bind(sourceIp => IP.TryParse(tokens[6])
                    .Map(destionationIp => new InternetCaptureEntry(sourceIp, destionationIp, timestamp))));
        }
    }
}
