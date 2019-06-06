using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Parsing;
using SecondMomentSketch.Hashing;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace SecondMomentSketch
{
    public sealed class PhonesActivityWindowManger
    {
        private int NumOfNodes { get; }
        private int AmsVectorLength { get; }
        private HashFunctionTable[] HashFunctionTable { get; }
        private PhonesActivityDataParser DataParser { get; set; }
        private bool ended = false;
        private Lazy<WindowedStatistics> Window { get; }

        public Vector[] GetChangeVector() => Window.Value.GetChangeCountVectors();
        public Vector[] GetCurrentVectors() => Window.Value.CurrentNodesCountVectors();

        public PhonesActivityWindowManger(int numOfNodes, int amsVectorLength, HashFunctionTable[] hashFunctionTable, PhonesActivityDataParser dataParser, Lazy<WindowedStatistics> window)
        {
            NumOfNodes = numOfNodes;
            AmsVectorLength = amsVectorLength;
            HashFunctionTable = hashFunctionTable;
            DataParser = dataParser;
            Window = window;
        }

        public Vector[] GetNextAmsVectors()
        {
            const double maxValue = 10000;
            var (maybeNewDataParser, timestampPhoneActivities) =  DataParser.NextTimestamp().ToValueTuple();
            if (maybeNewDataParser.IsNone)
                ended = true;
            else
                DataParser = maybeNewDataParser.ValueUnsafe;
            Vector[] amsVectors = Vector.Init(NumOfNodes);
            double partsInANode = maxValue / NumOfNodes;
            foreach (var phoneActivity in timestampPhoneActivities.Invoke())
            {
                var node = (int)((phoneActivity.From - 1) / partsInANode);
                Debug.Assert(node < NumOfNodes);
                var nodeHashes = HashFunctionTable[node];
                for (int index = 0; index < AmsVectorLength; index++)
                    amsVectors[node][index] += phoneActivity.Amount * nodeHashes[index](phoneActivity.From) * nodeHashes[index](phoneActivity.To);
            }

            return amsVectors;
        }


        public bool TakeStep()
        {
            if (ended)
                return false;

            Window.Value.Move(GetNextAmsVectors());
            return true;
        }

        public static PhonesActivityWindowManger Init(int window, int numOfNodes, int amsVectorLength, HashFunctionTable[] hashFunctionTable, PhonesActivityDataParser phonesActivityDataParser)
        {
            StrongBox<PhonesActivityWindowManger> phonesActivityWindowManager = new StrongBox<PhonesActivityWindowManger>(null);
            var lazyWindow = new Lazy<WindowedStatistics>(() => WindowedStatistics.Init(ArrayUtils.Init(window, _ => phonesActivityWindowManager.Value.GetNextAmsVectors())));
            phonesActivityWindowManager.Value = new PhonesActivityWindowManger(numOfNodes, amsVectorLength, hashFunctionTable, phonesActivityDataParser, lazyWindow);
            return phonesActivityWindowManager.Value;
        }
    }
}
