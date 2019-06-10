using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.AiderTypes.TaxiTrips;

namespace TaxiTripsDataParsing
{
    public static class Program
    {

        public static void Main(string[] args)
        {
            const int cacheSize = 1000;
            var baseDir = @"C:\Users\Yuval\Desktop\Data\Taxi Data\Good Data\FOIL2013";
            var outputFile = @"C:\Users\Yuval\Desktop\Data\Taxi Data\Good Data\FOIL2013\result.bin";

            using (var binOutputFile = File.Create(outputFile, 4096, FileOptions.SequentialScan))
            using (var binaryWriter = new BinaryWriter(binOutputFile))
            using (var cachedWriter = CachedMinWriter<TaxiTripEntry>.Create(cacheSize, t => t.ToBinary(binaryWriter)))
                foreach (var innerDir in Directory.EnumerateDirectories(baseDir).OrderBy(x => x))
                {
                    var files = Directory.EnumerateFiles(innerDir).ToArray();
                    var dataCsv = files.First(p => Path.GetFileNameWithoutExtension(p).Contains("data"));
                    var fareCsv = files.First(p => Path.GetFileNameWithoutExtension(p).Contains("fare"));

                    using (var dataCsvFile = File.OpenText(dataCsv))
                    using (var fareCsvFile = File.OpenText(fareCsv))
                    {
                        while (true)
                        {
                            if (dataCsvFile.EndOfStream || fareCsvFile.EndOfStream)
                                break;
                            TaxiTripEntry.TryParse(dataCsvFile.ReadLine(), fareCsvFile.ReadLine())
                                         .Iter(cachedWriter.WriteItem);
                        }
                    }
                }
        }
    }
}
