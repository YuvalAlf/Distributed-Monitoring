using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Utils.TypeUtils;

namespace Utils.AiderTypes
{
    public struct PhoneActivityEntry
    {
        public int From { get; }
        public int To { get; }
        public double Amount { get; }

        public PhoneActivityEntry(int @from, int to, double amount)
        {
            From = @from;
            To = to;
            Amount = amount;
        }

        public bool Equals(PhoneActivityEntry other) => From == other.From && To == other.To && Amount.Equals(other.Amount);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PhoneActivityEntry entry && Equals(entry);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = From;
                hashCode = (hashCode * 397) ^ To;
                hashCode = (hashCode * 397) ^ Amount.GetHashCode();
                return hashCode;
            }
        }

        public static IEnumerable<(string, PhoneActivityEntry)> ExtractWithTimestamp(Stream dataStream)
        {
            foreach (var line in dataStream.ReadLines())
            {
                var phoneActivityEntry = PhoneActivityEntry.TryParse(line);
                if (phoneActivityEntry.HasValue)
                    yield return phoneActivityEntry.Value;
            }
        }

        private static (string, PhoneActivityEntry)? TryParse(string line)
        {
            var entries = line.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (entries.Length != 4)
                return null;
            var fromOk   = int.TryParse(entries[1], out int from);
            var toOk     = int.TryParse(entries[2], out int to);
            var amountOk = double.TryParse(entries[3], out double amount);
            if (!fromOk || !toOk || !amountOk)
                return null;
            return (entries[0], new PhoneActivityEntry(from, to, amount));
        }

        public void ToBinary(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(From);
            binaryWriter.Write(To);
            binaryWriter.Write(Amount);
        }
        
        public static IEnumerable<PhoneActivityEntry> FromBinary(BinaryReader binaryReader)
        {
            while (!binaryReader.IsEOF())
                yield return PhoneActivityEntry.SingleFromBinary(binaryReader);
        }

        private static PhoneActivityEntry SingleFromBinary(BinaryReader binaryReader)
        {
            var from = binaryReader.ReadInt32();
            var to = binaryReader.ReadInt32();
            var amount = binaryReader.ReadDouble();
            return new PhoneActivityEntry(from, to, amount);
        }

        public override string ToString()
        {
            return $"{nameof(From)}: {From}, {nameof(To)}: {To}, {nameof(Amount)}: {Amount}";
        }
    }
}
