using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;

namespace Utils.TypeUtils
{
    public sealed class AutoFlushedTextFile : IDisposable
    {
        private string FilePath { get; }
        private StreamWriter Stream { get; }
        public static string SublimePath = @"C:\Program Files\Sublime Text 3\sublime_text.exe";

        public AutoFlushedTextFile(StreamWriter stream, string filePath)
        {
            Stream = stream;
            this.FilePath = filePath;
        }

        public void WriteLine(string line) => Stream.WriteLine(line);

        public static AutoFlushedTextFile Create(string textFilePath, string firstLile)
        {
            var stream = File.CreateText(textFilePath);
            stream.AutoFlush = true;
            stream.WriteLine(firstLile);
            if (File.Exists(SublimePath))
                Process.Start(SublimePath, textFilePath);
            return new AutoFlushedTextFile(stream, textFilePath);
        }

        public void Dispose()
        {
            Stream.Dispose();
            Process.Start(FilePath);
        }
    }
}
