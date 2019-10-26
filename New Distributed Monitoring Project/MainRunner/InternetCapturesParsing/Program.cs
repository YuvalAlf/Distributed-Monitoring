using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;

namespace InternetCapturesParsing
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var sourcePath      = @"C:\Users\Yuval\Desktop\Data\CTUs Internet Traffic\capture20110812.binetflow";
            var destinationPath = @"C:\Users\Yuval\Desktop\Data\CTUs Internet Traffic\ctu3.bin";
            var binaryPath = destinationPath;
            Statistics(binaryPath);
            //TransformToBinary(sourcePath, destinationPath);
        }

        public static void TransformToBinary(String sourcePath, string destinationPath)
        {
            using (var destinationFile = File.Create(destinationPath))
            using (var binaryWriter = new BinaryWriter(destinationFile))
                foreach (var csvLine in File.ReadLines(sourcePath).Skip(1))
                    InternetCaptureEntry.Parse(csvLine).Iter(capture => capture.ToBinary(binaryWriter));
        }

        public static void Statistics(string binaryPath)
        {
            using (var sourceFile = File.OpenRead(binaryPath))
            using (var binaryReader = new BinaryReader(sourceFile))
            {
                InternetCaptureEntry.FromBinary(binaryReader)
                                                .GroupBy(ip => ip.SourceIP.Byte0 / 64)
                                                .OrderBy(x => x.Key)
                                                .Select(group => group.Key.ToString() +"\t" + group.Count().ToString())
                                                .ForEach(date => Console.WriteLine(date));
                //Console.WriteLine("Count: " + count);
            }
        }

        public static DateTime RoundMinute(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
        }
    }
}
