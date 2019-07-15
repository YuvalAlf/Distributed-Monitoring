using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parsing;
using Utils.AiderTypes.TaxiTrips;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace TaxiTripsDataParsing
{
    public sealed class TaxiTripsWindowManger
    {
        public int SqrtNumOfNodes { get; }
        public int SqrtVectorLength { get; }
        private IEnumerator<(TaxiTripEntry, TaxiTripEntry)> TaxiTrips { get; }
        private CityRegion CityRegion { get; }
        private DataSplitter<TaxiTripEntry> Splitter { get; }
        private Lazy<WindowedStatistics> Window { get; }
        private bool ended = false;

        public TaxiTripsWindowManger(int sqrtNumOfNodes, int sqrtVectorLength, IEnumerator<(TaxiTripEntry, TaxiTripEntry)> taxiTrips, CityRegion cityRegion, DataSplitter<TaxiTripEntry> splitter, Lazy<WindowedStatistics> window)
        {
            SqrtNumOfNodes = sqrtNumOfNodes;
            SqrtVectorLength = sqrtVectorLength;
            TaxiTrips = taxiTrips;
            CityRegion = cityRegion;
            Splitter = splitter;
            Window = window;
        }

        public Vector[] GetCurrentVectors() => Window.Value.CurrentNodesCountVectors();
        public Vector[] GetChangeVector() => Window.Value.GetChangeCountVectors();

        public bool TakeStep()
        {
            if (ended)
                return false;

            Window.Value.Move(GetNextCountVector());
            return true;
        }

        private Vector[] GetNextCountVector()
        {
            var vectors = Vector.Init(SqrtNumOfNodes * SqrtNumOfNodes);

            while (TaxiTrips.MoveNext())
            {
                var (t1, t2) = TaxiTrips.Current;
                var node = CityRegion.Get(SqrtNumOfNodes, t1.PickupLatitude, t1.PickupLongtitude);
                var index = CityRegion.Get(SqrtVectorLength, t1.DropoffLatitude, t1.DropoffLongtitude);
                if (Splitter.IsY(t1))
                    index += SqrtVectorLength * SqrtVectorLength;

                vectors[node][index] += 1;

                if (t1.PickupTime.Hour != t2.PickupTime.Hour)
                    return vectors;
            }

            ended = true;
            return vectors;


        }

        public static TaxiTripsWindowManger Init(int hoursInWindow, int sqrtNumOfNodes, int sqrtVectorLengh, DataSplitter<TaxiTripEntry> splitter, CityRegion cityRegion, IEnumerable<TaxiTripEntry> taxiTrips)
        {
            var windowManager = new StrongBox<TaxiTripsWindowManger>(null);
            var lazyWindow = new Lazy<WindowedStatistics>(() => WindowedStatistics.Init(ArrayUtils.Init(hoursInWindow, _ => windowManager.Value.GetNextCountVector())));
            windowManager.Value = new TaxiTripsWindowManger(sqrtNumOfNodes, sqrtVectorLengh, taxiTrips.Pairs(), cityRegion, splitter, lazyWindow);
            return windowManager.Value;
        }
    }
}