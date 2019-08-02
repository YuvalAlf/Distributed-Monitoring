using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Entropy;
using EntropyMathematics;
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
        public static readonly string resultDir = @"C:\Users\Yuval\Desktop";
        public static readonly string databaseAccessesPath = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\TDADateSet.csv";
        public static readonly string phoneActivitiesBaseFolder = @"C:\Users\Yuval\Desktop\Data\Milano Phone Activity\Data";
        public static readonly string taxiBinDataPath = @"C:\Users\Yuval\Desktop\Data\Taxi Data\Good Data\FOIL2013\TaxiData.bin";
        public static readonly string stocksDirPath = @"C:\Users\Yuval\Desktop\Data\Stock Values\Stocks";
        //  public static readonly string databaseAccessesPath = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\trimmed.csv";

        private static void RunMilanoPhonesSecondMomentSketch(Random random)
        {
            int numOfNodes     = 9;
            var dimensions     = new[] { (5,5)};
            var window         = 24;
            var distributingMethod = new GridDistributing(1, 10000, numOfNodes);
            //var approximation  = new MultiplicativeUpperLowerApproximation(0.3, 3.0);
            //var approximation  = new ThresholdApproximation(2700000);
            var approximation  = new CombinedApproximation(new MultiplicativeUpperLowerApproximation(0.5, 2.0), new AdditiveApproximation(100000));
            foreach (var (width, height) in dimensions)
                SecondMomentRunner.RunMilanoPhoneActivity(random, numOfNodes, window, approximation, width, height, distributingMethod, phoneActivitiesBaseFolder, resultDir);
        }

        private static void RunDatabaseSecondMomentSketch(Random random)
        {
            int numOfNodes     = 10;
            var values         = new[] { (30, 31) };
            var window         = 100;
            var distrubteUsers = UsersDistributing.UnevenHashing();
            var approximation        = new MultiplicativeApproximation(0.9);
            foreach (var (width, height) in values)
                SecondMomentRunner.RunDatabaseAccesses(random, numOfNodes, window, approximation, width, height, distrubteUsers, databaseAccessesPath, resultDir);
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
            int numOfNodes     = 10;
            var window         = 3;
            var wantedVectorLength = 1000;
            var minVolumeBucket = 100.0;
            var maxVolumeBucket = 4.0 * 10E8;
            var mulFactor = Math.Pow(maxVolumeBucket / minVolumeBucket, 1.0 / wantedVectorLength);
            var closestValueQuery = ClosestValueQuery.InitExponential((long)minVolumeBucket, (long)maxVolumeBucket, mulFactor);
            var approximation = new MultiplicativeApproximation(0.1);
            DateTime startingDateTime = new DateTime(2006, 1, 3);
            int minAmountAtDay = 1000;
            var iterations = 1000;
            EntropyRunner.RunStocks(random, iterations, closestValueQuery, numOfNodes, window, startingDateTime, minAmountAtDay, approximation, stocksDirPath, resultDir);
        }

        private static void RunRandomAms(Random random)
        {
            int               numOfNodes    = 16;
            int               iterations    = 10000;
            //var width = 21;
            //var height = 21;
            foreach (var (width, height) in new[]
                                            {
                                                (7, 7), (11, 11), (31, 31), (51, 51), (71, 71), (91, 91),
                                                (121, 121), (151, 151), (201, 201)
                                            })
            {
                ApproximationType approximation = new CombinedApproximation(new MultiplicativeUpperLowerApproximation(0.5, 2.0), new AdditiveApproximation(width * height * 40));
                SecondMomentRunner.RunRandomly(random, width, height, numOfNodes, iterations, approximation, resultDir);
            }
        }
        private static void RunRandomInnerProduct(Random random)
        {
            int numOfNodes = 10;
            ApproximationType approximation = new MultiplicativeUpperLowerApproximation(0.5, 2.0);
            int vectorLength = 500;
            int iterations = 200;
            InnerProductRunner.RunRandomly(random, numOfNodes, approximation, vectorLength, iterations, resultDir);
        }

        private static void RunTaxiTripsInnerProduct(Random random)
        {
            // NYC Coords
            // Big Square
                    /*const double minLat           = 40.49;
                    const double maxLat           = 40.91;
                    const double minLong          = -74.26;
                    const double maxLong          = -73.66; */
            // Small Square
            const double minLat = 40.623;
            const double maxLat = 40.846;
            const double minLong = -74.04;
            const double maxLong = -73.74;

            var          nycCityRegion    = new CityRegion(minLat, maxLat, minLong, maxLong);
            var          sqrtNumOfNodes   = 5;
            var          sqrtVectorLength = 10;
            var          hoursInWindow    = 24;
            var          iterations       = 1000;
            var tipSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.Tip > 0, "Tip");
            var vendorSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.TaxiVendor == TaxiVendor.CMT, "Vendor");
            var paymentSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.PaymentType == PaymentType.Cash, "PaymentType");
            var passangersSplitter = new DataSplitter<TaxiTripEntry>(entry => entry.NumOfPassangers > 1, "Passangers");
            var splitters = ArrayUtils.Init(tipSplitter, vendorSplitter, paymentSplitter, passangersSplitter);
            var approximation = new MultiplicativeUpperLowerApproximation(0.5, 2.0);
            
            foreach (var splitter in splitters)
                InnerProductRunner.RunTaxiTrips(random, iterations, sqrtNumOfNodes, hoursInWindow, approximation,
                                            sqrtVectorLength, splitter, nycCityRegion, taxiBinDataPath, resultDir);
        }

        public static void Main(string[] args)
        {
            var random = new Random(1631);

            RunStocksEntropy(random);

            //  RunRandomInnerProduct(random);

             // RunTaxiTripsInnerProduct(random);

            // RunRandomAms(random);
            //   RunMilanoPhonesSecondMomentSketch(random);
            // RunEntropy(random);
            // RunDatabaseSecondMomentSketch(random);
           //   RunInnerProduct(random);
        }

    }
}
