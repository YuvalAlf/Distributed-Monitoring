using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;
using Utils.DataStructures;
using Utils.MathUtils;

namespace DataParsing
{
    public sealed class CharDataParser : IDisposable
    {
        private Dictionary<StreamReader, WindowedHistogram<char>> Histograms { get; }
        public IEnumerable<WindowedHistogram<char>> CharHistograms => Histograms.Values;
        public SortedSet<char> OptionalChars { get; }

        public CharDataParser(Dictionary<StreamReader, WindowedHistogram<char>> histograms, SortedSet<char> optionalChars)
        {
            Histograms = histograms;
            OptionalChars = optionalChars;
        }

        public static CharDataParser Init(int windowSize, string chars, params string[] pathes)
        {
            if (!pathes.All(File.Exists))
                throw new ArgumentException();

            var optionalChars = new SortedSet<char>(chars);

            var histograms = new Dictionary<StreamReader, WindowedHistogram<char>>(pathes.Length);
            foreach (var path in pathes)
            {
                var streamReader = File.OpenText(path);
                var histogram = WindowedHistogram<char>.Init(Read(streamReader, windowSize, optionalChars), optionalChars);
                histograms.Add(streamReader, histogram);
            }
            return new CharDataParser(histograms ,optionalChars);
        }

        public bool Next(int stepSize)
        {
            var didntFinish = true;
            foreach (var streamReader in Histograms.Keys)
            {
                var histogram = Histograms[streamReader];
                var streamDidntEnd = histogram.MoveWindow(Read(streamReader, stepSize, OptionalChars)) == stepSize;
                didntFinish = streamDidntEnd && didntFinish;
            }

            return didntFinish;
        }

        private static IEnumerable<char> Read(StreamReader stream, int amountToRead, SortedSet<char> optionalChars)
        {
            var amount = 0;
            while (amount < amountToRead)
            {
                var ch = stream.Read();
                if (ch == -1)
                   break;
                var character = (char) ch;
                if (optionalChars.Contains(character))
                {
                    yield return character;
                    amount++;
                }
            }
        }

        public void Dispose()
        {
            Histograms.Keys.ForEach(s => s.Close());
        }
    }
}
