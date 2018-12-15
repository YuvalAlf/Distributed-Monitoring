using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using Utils.DataStructures;
using Utils.TypeUtils;

namespace DataParsing
{
    /*
    [Obsolete]
    public sealed class ArnonDataParser : IDisposable
    {
        public int VectorSize { get; }
        public Vector<double> ZeroVector { get; }
        public Vector<double> DoubleZeroVector { get; }
        public Dictionary<string, int> IndexOfString { get; }
        public SortedSet<string> OptionalValues { get; }
        public StreamReader StreamReader { get; }
        public ChunckedHistogram<string>[] Histograms { get; }

        public ArnonDataParser(Dictionary<string, int> indexOfString, SortedSet<string> optionalValues, StreamReader streamReader, Vector<double> zeroVector, Vector<double> doubleZeroVector, int vectorSize, ChunckedHistogram<string>[] histograms)
        {
            IndexOfString = indexOfString;
            OptionalValues = optionalValues;
            StreamReader = streamReader;
            ZeroVector = zeroVector;
            DoubleZeroVector = doubleZeroVector;
            VectorSize = vectorSize;
            Histograms = histograms;
        }

        public static ArnonDataParser Init(StreamReader dataText, int windowSize, string[] optionalValues, int numOfNodes)
        {
            var vectorSize = optionalValues.Length;
            var stringIndices = new Dictionary<string, int>(vectorSize);
            optionalValues.ForEach((str, index) => stringIndices.Add(str, index));
            var optionalStrings = new SortedSet<string>(optionalValues, StringComparer.OrdinalIgnoreCase);
            var zeroVector = Enumerable.Repeat(0.0, vectorSize).ToVector();
            var doubleZeroVector = Enumerable.Repeat(0.0, vectorSize * 2).ToVector();
            var histograms = ArrayUtils.Init(numOfNodes, _ => ChunckedHistogram<string>.Init(optionalStrings, stringIndices, numOfNodes));
            for (int i = 0; i < windowSize; i++)
            for (int node = 0; node < numOfNodes; node++)
            {
                var line = dataText.ReadLine();
                if (line == null)
                    throw new ArgumentException("Read Null");
                if (!line.Equals(""))
                {
                    var (uid, counts) = ParseLine(line);
                    histograms[node].AddToEnd(uid, counts);
                }
            }
            return new ArnonDataParser(stringIndices, optionalStrings, dataText, zeroVector, doubleZeroVector, vectorSize, histograms);
        }

        private static (int, Dictionary<string, int>) ParseLine(string line)
        {

            var dataArray = line.Split(';');
            var uid = int.Parse(dataArray[1]);
            var text = dataArray.Last();
            var tokenCount = new Dictionary<string, int>();
            var index = 1;
            while (true)
            {
                var (str, count, finished) = ParseToken(text, ref index);
                tokenCount[str] = count;
                if (finished)
                    break;
            }
            return (uid, tokenCount);
        }

        private static void LocalAssert(char a, char b)
        {
            if (a != b)
                throw new Exception();
        }

        private static (string, int, bool) ParseToken(string line, ref int index)
        {
            LocalAssert(line[index++], 'u');
            LocalAssert(line[index++], '\'');
            var token = new StringBuilder(10);
            while (true)
            {
                var ch = line[index++];
                if (ch == '\'')
                    break;
                token.Append(ch);
            }

            LocalAssert(line[index++], ':');
            LocalAssert(line[index++], ' ');
            var count = new StringBuilder(2);
            while (true)
            {
                var ch = line[index++];
                if (char.IsDigit(ch))
                    count.Append(ch);
                else
                    break;
            }

            var finished = index + 2 >= line.Length;
            if (!finished)
                LocalAssert(line[index++], ' ');
            return (token.ToString(), int.Parse(count.ToString()), finished);
        }

        public void Dispose() => StreamReader?.Dispose();

        public Vector<double>[] NextChange(out bool finishedStream)
        {
            foreach (var histogram in Histograms)
            {
                var line = StreamReader.ReadLine();
                if (line == null)
                {
                    finishedStream = true;
                    return Histograms.Map(_ => DoubleZeroVector);
                }
                if (!line.Equals(""))
                {
                    var (uid, counts) = ParseLine(line);
                    histogram.MoveWindow(uid, counts);
                }
            }

            finishedStream = false;
            return Histograms.Map(h => h.ChangedCountVector());
        }
    }*/
}
