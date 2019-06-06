using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;

namespace MilanoPhonesDataParsing
{
    public sealed class StreamWriterManger : IDisposable
    {
        public string BaseFolder { get; }
        public string FilesExtension { get; }
        public Dictionary<string, BinaryWriter> Streams { get; }

        public StreamWriterManger(string baseFolder, string filesExtension, Dictionary<string, BinaryWriter> streams)
        {
            BaseFolder = baseFolder;
            FilesExtension = filesExtension;
            Streams = streams;
        }

        public static StreamWriterManger Create(string baseFolder, string filesExtension)
        {
            return new StreamWriterManger(baseFolder, filesExtension, new Dictionary<string, BinaryWriter>());
        }


        public BinaryWriter GetStream(string streamName)
        {
            if (!Streams.ContainsKey(streamName))
                Streams[streamName] =
                    new BinaryWriter(File.Create(Path.Combine(BaseFolder, streamName + "." + FilesExtension)));
            return Streams[streamName];
        }

        public void Dispose()
        {
            Streams?.Values.ForEach(s => s?.Close());
        }
    }
}
