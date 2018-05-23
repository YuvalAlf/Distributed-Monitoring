using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;
using Utils.DataStructures;

namespace DataParsing
{
    public sealed class DataParser<T> : IDisposable
    {
        public Dictionary<StreamReader, IEnumerator<T>> DataEnumarators { get; }
        private Dictionary<StreamReader, WindowedHistogram<T>> HistogramsDictionary { get; }
        public IEnumerable<WindowedHistogram<T>> Histograms => HistogramsDictionary.Values;
        public SortedSet<T> OptionalValues { get; }

        public DataParser(Dictionary<StreamReader, WindowedHistogram<T>> histogramsDictionary, SortedSet<T> optionalValues, Dictionary<StreamReader, IEnumerator<T>> dataEnumarators)
        {
            HistogramsDictionary = histogramsDictionary;
            OptionalValues = optionalValues;
            DataEnumarators = dataEnumarators;
        }

        public static DataParser<T> Init(Func<StreamReader, IEnumerator<T>> dataEnumarator, int windowSize, SortedSet<T> optionalValues, params string[] pathes)
        {
            if (!pathes.All(File.Exists))
                throw new ArgumentException();

            var enumarators = new Dictionary<StreamReader, IEnumerator<T>>(pathes.Length);
            var histograms = new Dictionary<StreamReader, WindowedHistogram<T>>(pathes.Length);
            foreach (var path in pathes)
            {
                var streamReader = File.OpenText(path);
                var enumarator = dataEnumarator(streamReader);
                var histogram = WindowedHistogram<T>.Init(Read(enumarator, windowSize, optionalValues), optionalValues);
                histograms.Add(streamReader, histogram);
                enumarators.Add(streamReader, enumarator);
            }
            return new DataParser<T>(histograms , optionalValues, enumarators);
        }

        public bool Next(int stepSize)
        {
            var didntFinish = true;
            foreach (var streamReader in HistogramsDictionary.Keys)
            {
                var enumarator = DataEnumarators[streamReader];
                var histogram = HistogramsDictionary[streamReader];
                var streamDidntEnd = histogram.MoveWindow(Read(enumarator, stepSize, OptionalValues)) == stepSize;
                didntFinish = streamDidntEnd && didntFinish;
            }

            return didntFinish;
        }

        private static IEnumerable<T> Read(IEnumerator<T> dataEnumarator, int amountToRead, SortedSet<T> optionalValues)
        {
            for (int amount = 0; amount < amountToRead;)
            {
                if (!dataEnumarator.MoveNext())
                    break;
                if (optionalValues.Contains(dataEnumarator.Current))
                {
                    amount++;
                    yield return dataEnumarator.Current;
                }
            }
        }

        public void Dispose()
        {
            DataEnumarators.Keys.ForEach(s => s.Close());
        }
    }
}
