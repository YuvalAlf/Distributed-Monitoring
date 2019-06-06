using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.AiderTypes;
using Utils.TypeUtils;
using Path = System.IO.Path;

namespace MilanoPhonesDataParsing
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var milanoZipFile = @"C:\Users\Yuval\Downloads\november.zip";
            var milanoOutputFolder = @"C:\Users\Yuval\Downloads\MilanoPhoneActivity";

            foreach (var (dateFileName, dataStream) in ZipUtils.IterateEntries(milanoZipFile).Take(2))
                using (dataStream)
                {
                    var date       = Path.GetFileNameWithoutExtension(dateFileName);
                    var dateFolder = Path.Combine(milanoOutputFolder, date ?? throw new NullReferenceException());
                    Directory.CreateDirectory(dateFolder);

                    using (var streamWriterManger = StreamWriterManger.Create(dateFolder, "bin"))
                        foreach (var (timestamp, phoneActivityEntry) in PhoneActivityEntry.ExtractWithTimestamp(dataStream))
                        {
                            var timestampStream = streamWriterManger.GetStream(timestamp);
                            phoneActivityEntry.ToBinary(timestampStream);
                        }
                }
        }
    }
}
