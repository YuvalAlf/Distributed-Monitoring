using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using Utils.DataStructures;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace DataParsing
{
    public sealed class TextParser<T> : IDisposable
    {
        public Dictionary<StreamReader, IEnumerator<T>> DataEnumarators { get; }
        private Dictionary<StreamReader, WindowedHistogram<T>> HistogramsDictionary { get; }
        public IEnumerable<WindowedHistogram<T>> Histograms => HistogramsDictionary.Values;
        public SortedSet<T> OptionalValues { get; }

        public TextParser(Dictionary<StreamReader, WindowedHistogram<T>> histogramsDictionary, SortedSet<T> optionalValues, Dictionary<StreamReader, IEnumerator<T>> dataEnumarators)
        {
            HistogramsDictionary = histogramsDictionary;
            OptionalValues = optionalValues;
            DataEnumarators = dataEnumarators;
        }

        public static TextParser<T> Init(Func<StreamReader, IEnumerator<T>> dataEnumarator, int windowSize, SortedSet<T> optionalValues, params string[] pathes)
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
            return new TextParser<T>(histograms , optionalValues, enumarators);
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

        public IEnumerable<Vector[]> AllCountVectors(int stepSize)
        {
            while (this.Next(stepSize))
                yield return Histograms.Map(h => h.ChangedCountVector());
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

        public void Dispose() => DataEnumarators.Keys.ForEach(s => s.Close());
    }
}
