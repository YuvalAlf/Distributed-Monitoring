using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Entropy;
using EntropyMathematics;
using EntropySketch;
using InnerProduct;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Approximation;
using Monitoring.Utils.DataDistributing;
using MoreLinq.Extensions;
using SecondMomentSketch;
using SecondMomentSketch.Hashing;
using Sphere;
using TaxiTripsDataParsing;
using Utils.AiderTypes.TaxiTrips;
using Utils.TypeUtils;

namespace MonitoringProject
{
    public static class Program
    {
        public static readonly string resultDir = @"C:\Users\Yuval\Desktop\New Entropy Results After Bug Fix";
        public static readonly string databaseAccessesPath = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\TDADateSet.csv";
        public static readonly string phoneActivitiesBaseFolder = @"C:\Users\Yuval\Desktop\Data\Milano Phone Activity\Data";
        public static readonly string taxiBinDataPath = @"C:\Users\Yuval\Desktop\Data\Taxi Data\Good Data\FOIL2013\TaxiData.bin";
        public static readonly string stocksDirPath = @"C:\Users\Yuval\Desktop\Data\Stock Values\Stocks";
        //  public static readonly string databaseAccessesPath = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\trimmed.csv";

        private static void RunMilanoPhonesSecondMomentSketch(Random random)
        {
            int numOfNodes     = 36;
           // var dimensions     = new[] { (9, 9)};
            var window         = 24;
           // foreach (var numOfNodes in ArrayUtils.Init(16, 36, 64, 100, 144, 13 * 13, 14 * 14))
            //foreach (var numOfNodes in ArrayUtils.Init(10,20,26,31,36,40,43,46).Select(x => x * x))
            {
                var distributingMethod = new GridDistributing(1, 10000, numOfNodes);
                //var approximation  = new MultiplicativeUpperLowerApproximation(0.3, 3.0);
                //var approximation  = new ThresholdApproximation(2700000);
                var approximation = new CombinedApproximation(new MultiplicativeUpperLowerApproximation(0.5, 2.0),
                                                              new AdditiveApproximation(100000));
                foreach (var (width, height) in ArrayUtils.Init(73).Select(x => (x, 11)))
                    SecondMomentRunner.RunMilanoPhoneActivity(random, numOfNodes, window, approximation, width, height,
                                                              distributingMethod, phoneActivitiesBaseFolder, resultDir);
            }
        }
        
        private static void RunDatabaseAccessesEntropy(Random random)
        {
            int numOfNodes     = 10;
            var window         = 14;
            var vectorLength   = 11654;
            var distrubteUsers = UsersDistributing.RoundRobin();
            var approximation = new MultiplicativeApproximation(0.02);
            EntropyRunner.RunDatabaseAccesses(random, numOfNodes, window, approximation, vectorLength, distrubteUsers, databaseAccessesPath, resultDir);
        }

        private static void RunStocksEntropy(Random random)
        {
            //int      numOfNodes       = 20;
            var      window           = 50;
            var      minVolumeBucket  = 100.0;
            var      maxVolumeBucket  = 4.0 * 10E8;
            var      approximation    = new MultiplicativeApproximation(0.005);
            DateTime startingDateTime = new DateTime(2006, 1, 3);
            int      minAmountAtDay   = 1000;
            var      iterations       = 1000;
            //var wantedVectorLength = 100;
            //foreach (var wantedVectorLength in ArrayUtils.Init(50, 100, 200, 400, 800, 1300, 1600, 2000, 2400))
            for (var wantedVectorLength = 4000; wantedVectorLength <= 4000; wantedVectorLength += 100)
                for (var numOfNodes = 30; numOfNodes <= 30; numOfNodes += 30)
                    //foreach (var numOfNodes in ArrayUtils.Init(30, 40))
                {
                    var mulFactor = Math.Pow(maxVolumeBucket / minVolumeBucket, 1.0 / wantedVectorLength);
                    var closestValueQuery =
                        ClosestValueQuery.InitExponential((long) minVolumeBucket, (long) maxVolumeBucket, mulFactor);
                    var vectorLength = closestValueQuery.Data.Length;
                    foreach (var (entropy, cbName) in
                        ArrayUtils.Init((new EntropyFunction(vectorLength).MonitoredFunction, "RegularCB")))
                                       // (new SpecialCBEntropy(vectorLength).MonitoredFunction, "SmartCB")))
                    {
                        EntropyRunner.RunStocks(random, entropy, cbName, iterations, closestValueQuery, numOfNodes,
                                                window, startingDateTime,
                                                minAmountAtDay, approximation, stocksDirPath, resultDir);
                    }
                }
        }

        private static void RunStocksSketchEntropy(Random random)
        {
            //int      numOfNodes       = 20;
            var      window           = 50;
            var      minVolumeBucket  = 100.0;
            var      maxVolumeBucket  = 4.0 * 10E8;
            var      approximation    = new MultiplicativeApproximation(0.0022);
            DateTime startingDateTime = new DateTime(2006, 1, 3);
            int      minAmountAtDay   = 1000;
            var      iterations       = 1000;
            for (var baseVectorLength = 1000; baseVectorLength <= 1000; baseVectorLength += 100)
                for (var numOfNodes = 10; numOfNodes <= 10; numOfNodes += 10)
                    for (var reducedDimension = 100; reducedDimension <= 100; reducedDimension += 100)
                    {
                        var mulFactor = Math.Pow(maxVolumeBucket / minVolumeBucket, 1.0 / baseVectorLength);
                        var closestValueQuery =
                            ClosestValueQuery.InitExponential((long) minVolumeBucket, (long) maxVolumeBucket,
                                                              mulFactor);
                        var vectorLength = closestValueQuery.Data.Length;
                        EntropySketchRunner.RunStocks(random, iterations, closestValueQuery, numOfNodes,
                                                      reducedDimension,
                                                      window, startingDateTime,
                                                      minAmountAtDay, approximation, stocksDirPath, resultDir);
                    }
        }

        private static void RunRandomAms(Random random)
        {
            int  iterations    = 2000;
            bool oneChanges = true;
            for (var numOfNodes = 25; numOfNodes <= 25; numOfNodes += 5)
                for (var width = 21; width < 201; width += 10)
                {
                    var height = 20;

                    ApproximationType approximation = new CombinedApproximation(new MultiplicativeUpperLowerApproximation(0.3, 3.0));
                    SecondMomentRunner.RunRandomly(random, width, height, numOfNodes, iterations, approximation, oneChanges, resultDir);
                }
        }

        private static void RunRandomInnerProduct(Random random)
        {
            ApproximationType approximation = new MultiplicativeUpperLowerApproximation(0.5, 2.0);
            int iterations = 3000;
            var vectorLength = 100;
            //var numOfNodes = 40;
            //for (int vectorLength = 100; vectorLength <= 3000; vectorLength += 100)
            for (int numOfNodes = 260; numOfNodes <= 400; numOfNodes += 10)
                InnerProductRunner.RunRandomly(random, numOfNodes, approximation, vectorLength, iterations, resultDir);
        }

        private static void RunRandomEntropy(Random random)
        {
           // int numOfNodes = 10;
            int iterations = 10000;
            bool oneChanges = false;
            var vectorLength = 1000;
            var collapseDimension = 500;
            //foreach(var vectorLength in ArrayUtils.Init(200, 500, 1000, 1700, 2400, 3500, 4200, 5000, 5800, 7000))
           // for (var vectorLength = 50; vectorLength <= 2000; vectorLength += 50)
            for (var numOfNodes = 10; numOfNodes <= 10; numOfNodes += 5)
            {
                var approximation = new MultiplicativeApproximation(0.05 / Math.Pow(numOfNodes, 0.5));
                EntropyRunner.RunRandomly(random, numOfNodes, approximation, vectorLength, collapseDimension, iterations, oneChanges, resultDir);
            }
                
        }

        private static void RunTaxiTripsInnerProduct(Random random)
        {
            // NYC Coords
            // Big Square
            const double minLat           = 40.49;
            const double maxLat           = 40.91;
            const double minLong          = -74.26;
            const double maxLong          = -73.66;
            // Small Square
            /*const double minLat = 40.623;
            const double maxLat = 40.846;
            const double minLong = -74.04;
            const double maxLong = -73.74;*/

            var nycCityRegion    = new CityRegion(minLat, maxLat, minLong, maxLong);
            //var          sqrtNumOfNodes   = 5;
            //var          sqrtVectorLength = 10;
            var          hoursInWindow    = 24;
            var          iterations       = 750;
            var tipSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.Tip > 0, "Tip");
            var vendorSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.TaxiVendor == TaxiVendor.CMT, "Vendor");
            var paymentSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.PaymentType == PaymentType.Cash, "PaymentType");
            var passangersSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.NumOfPassangers > 1, "Passangers");
            var splitters = ArrayUtils.Init(tipSplitter, vendorSplitter, paymentSplitter, passangersSplitter);
            var approximation = new MultiplicativeUpperLowerApproximation(0.5, 2.0);
            var chosenSplitter = vendorSplitter;


            foreach (var sqrtVectorLength in ArrayUtils.Init(22,70,100,122,141,158,173,187))
                foreach (var sqrtNumOfNodes in ArrayUtils.Init(9))
                    InnerProductRunner.RunTaxiTrips(random, iterations, sqrtNumOfNodes, hoursInWindow, approximation,
                                            sqrtVectorLength, chosenSplitter, nycCityRegion, taxiBinDataPath, resultDir);
        }

        public static void Main(string[] args)
        {
            var random = new Random(1631);

           //  RunRandomEntropy(random);
           //  RunStocksEntropy(random);


             RunStocksSketchEntropy(random);

            // RunRandomInnerProduct(random);
           //   RunTaxiTripsInnerProduct(random);

            // RunRandomAms(random);
           //     RunMilanoPhonesSecondMomentSketch(random);
        }

    }
}
