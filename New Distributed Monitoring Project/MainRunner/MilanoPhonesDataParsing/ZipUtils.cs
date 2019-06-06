using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilanoPhonesDataParsing
{
    class ZipUtils
    {
        public static IEnumerable<(string, Stream)> IterateEntries(string zipPath)
        {
            using (var file = File.OpenRead(zipPath))
            using (var zipFile = new ZipArchive(file, ZipArchiveMode.Read))
                foreach (var zipArchiveEntry in zipFile.Entries)
                    yield return (zipArchiveEntry.Name, zipArchiveEntry.Open());
        }
    }
}
