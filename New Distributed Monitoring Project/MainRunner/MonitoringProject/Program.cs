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
        public const string resultDir                 = @"C:\Users\Yuval\Desktop";
        public const string databaseAccessesPath      = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\TDADateSet.csv";
        public const string phoneActivitiesBaseFolder = @"C:\Users\Yuval\Desktop\Data\Milano Phone Activity\Data";
        public const string taxiBinDataPath           = @"C:\Users\Yuval\Desktop\Data\Taxi Data\Good Data\FOIL2013\TaxiData.bin";
        public const string stocksDirPath             = @"C:\Users\Yuval\Desktop\Data\Stock Values\Stocks";
        public const string ctuFilePath               = @"C:\Users\Yuval\Desktop\Data\CTUs Internet Traffic\ctu3.bin";
        //  public const string databaseAccessesPath = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\trimmed.csv";

        private static void RunMilanoPhonesSecondMomentSketch(Random random)
        {
            //int numOfNodes     = 25;
           // var dimensions     = new[] { (9, 9)};
            var window         = 24;
            // foreach (var numOfNodes in ArrayUtils.Init(16, 36, 64, 100, 144, 13 * 13, 14 * 14))
            foreach (var numOfNodes in ArrayUtils.Init(4)
                                                 .Select(x => x * x))
            {
                var distributingMethod = new GridDistributing(1, 10000, numOfNodes);
                //var approximation  = new MultiplicativeUpperLowerApproximation(0.3, 3.0);
                //var approximation  = new ThresholdApproximation(2700000);
                //var approximation = new CombinedApproximation(new MultiplicativeUpperLowerApproximation(0.5, 2.0),
                //                                              new AdditiveApproximation(100000));
              //  foreach (var approximation in ArrayUtils.Init(1.00E05, 2.00E05, 3.00E05, 4.00E05, 5.00E05, 6.00E05, 7.00E05, 8.00E05, 9.00E05, 1.00E06, 1.10E06, 1.20E06, 1.30E06, 1.40E06, 1.50E06, 1.60E06, 1.70E06, 1.80E06, 1.90E06, 2.00E06, 2.10E06, 2.20E06, 2.30E06, 2.40E06, 2.50E06, 2.60E06, 2.70E06, 2.80E06, 2.90E06, 3.00E06, 3.10E06, 3.20E06, 3.30E06, 3.40E06, 3.50E06, 3.60E06, 3.70E06, 3.80E06, 3.90E06, 4.00E06, 4.10E06, 4.20E06, 4.30E06, 4.40E06, 4.50E06, 4.60E06, 4.70E06, 4.80E06, 4.90E06, 5.00E06, 5.10E06, 5.20E06, 5.30E06, 5.40E06, 5.50E06, 5.60E06, 5.70E06, 5.80E06, 5.90E06, 6.00E06, 6.10E06, 6.20E06, 6.30E06, 6.40E06, 6.50E06, 6.60E06, 6.70E06, 6.80E06, 6.90E06, 7.00E06, 7.10E06, 7.20E06, 7.30E06, 7.40E06, 7.50E06, 7.60E06, 7.70E06, 7.80E06, 7.90E06, 8.00E06, 8.10E06, 8.20E06, 8.30E06, 8.40E06, 8.50E06, 8.60E06, 8.70E06, 8.80E06, 8.90E06, 9.00E06, 9.10E06, 9.20E06, 9.30E06, 9.40E06, 9.50E06, 9.60E06, 9.70E06, 9.80E06, 9.90E06, 1.00E07).Select(x => new AdditiveApproximation(x)))
                { 
                    var approximation = new AdditiveApproximation(0.5E06);
                    foreach (var (width, height) in ArrayUtils
                                                   .Init(2, 5, 10, 14, 18, 22, 27, 31, 36, 40, 45, 50, 54, 59, 63, 68, 72, 77, 81, 86, 90)
                                                   .Select(x => (14, x)))
                        SecondMomentRunner.RunMilanoPhoneActivity(random, numOfNodes, window, approximation, width,
                                                                  height,
                                                                  distributingMethod, phoneActivitiesBaseFolder,
                                                                  resultDir);
                }
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


        private static void RunCtuSketchEntropy(Random random)
        {
            //var numOfNodes             = 12;
            var window                 = 60 * 6;
            var approximation          = new AdditiveApproximation(0.5);
            var maxIterations          = 4000;
            for (int numOfNodes = 10; numOfNodes <= 10; numOfNodes += 10)
            for (var reducedSketchDimension = 50; reducedSketchDimension <= 1000; reducedSketchDimension += 50)
                EntropySketchRunner.RunCTU(random, maxIterations, numOfNodes, window, reducedSketchDimension, approximation,
                                       ctuFilePath, resultDir);
        }

        private static void RunStocksSketchEntropy(Random random)
        {
            //int      numOfNodes       = 20;
            var      window           = 60;
            var      minVolumeBucket  = 100.0;
            var      maxVolumeBucket  = 4.0 * 10E8;
            var      approximation    = new MultiplicativeApproximation(0.0035);
            DateTime startingDateTime = new DateTime(2006, 1, 3);
            int      minAmountAtDay   = 1000;
            var      iterations       = 2500;
            for (var baseVectorLength = 100; baseVectorLength <= 100; baseVectorLength += 100)
                for (var numOfNodes = 10; numOfNodes <= 10; numOfNodes += 10)
                    for (var reducedDimension = 30; reducedDimension <= 30; reducedDimension += 100)
                    {
                        var mulFactor = Math.Pow(maxVolumeBucket / minVolumeBucket, 1.0 / baseVectorLength);
                        var closestValueQuery =
                            ClosestValueQuery.InitExponential((long) minVolumeBucket, (long) maxVolumeBucket,
                                                              mulFactor);
                        var vectorLength = closestValueQuery.Data.Length;
                        EntropySketchRunner.RunStocks(random, iterations, closestValueQuery, numOfNodes,
                                                      window, reducedDimension, startingDateTime,
                                                      minAmountAtDay, approximation, stocksDirPath, resultDir);
                    }
        }

        private static void RunRandomAms(Random random)
        {
            int  iterations    = 4000;
            bool oneChanges = true;
            for (var numOfNodes = 41; numOfNodes <= 41; numOfNodes += 2)
                for (var width = 21; width <= 21; width += 10)
                {
                    var height = 21;

                    //ApproximationType approximation = new CombinedApproximation(new MultiplicativeUpperLowerApproximation(0.5, 2.0));
                    ApproximationType approximation = new CombinedApproximation(new MultiplicativeUpperLowerApproximation(0.5, 2.0));
                    SecondMomentRunner.RunRandomly(random, width, height, numOfNodes, iterations, approximation, oneChanges, resultDir);
                }
        }

        private static void RunRandomInnerProduct(Random random)
        {
            ApproximationType approximation = new MultiplicativeUpperLowerApproximation(0.5, 2.0);
            int iterations = 3000;
            var vectorLength = 1000;
            //var numOfNodes = 40;
            //for (int vectorLength = 100; vectorLength <= 3000; vectorLength += 100)
            for (int numOfNodes = 71; numOfNodes <= 71; numOfNodes += 10)
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
            //var          iterations       = 750;
            var iterations = 300;
            var tipSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.Tip > 0, "Tip");
            var vendorSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.TaxiVendor == TaxiVendor.CMT, "Vendor");
            var paymentSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.PaymentType == PaymentType.Cash, "PaymentType");
            var passengersSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.NumOfPassangers > 1, "Passengers");
            var splitters = ArrayUtils.Init(tipSplitter, vendorSplitter, paymentSplitter, passengersSplitter);
            var approximation = new MultiplicativeUpperLowerApproximation(0.7, 1.3);
            var chosenSplitter = vendorSplitter;


           // foreach (var sqrtVectorLength in ArrayUtils.Init(101))
            foreach (var sqrtVectorLength in ArrayUtils.Init(30))  //150, 158, 165, 173, 180, 187, 193, 200, 206, 212, 217, 223))
             //   foreach (var sqrtNumOfNodes in ArrayUtils.Init(/*2,4,*/ 6,8,9,10,11,12,13,14,15,16,17,18,19,20))
                foreach (var sqrtNumOfNodes in ArrayUtils.Init(5, 10, 13, 15))
                    InnerProductRunner.RunTaxiTrips(random, iterations, sqrtNumOfNodes, hoursInWindow, approximation,
                                            sqrtVectorLength, chosenSplitter, nycCityRegion, taxiBinDataPath, resultDir);
        }

        public static void Main(string[] args)
        {
            var random = new Random(1631);

            //  RunRandomEntropy(random);
            // RunStocksEntropy(random);
            //  RunStocksSketchEntropy(random);
            //RunRandomInnerProduct(random);
            //  RunRandomAms(random);

            //RunCtuSketchEntropy(random);

            RunTaxiTripsInnerProduct(random);

            //RunMilanoPhonesSecondMomentSketch(random);
        }

    }
}
