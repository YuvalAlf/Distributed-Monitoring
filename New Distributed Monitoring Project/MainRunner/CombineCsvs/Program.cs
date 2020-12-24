using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.TypeUtils;

namespace CombineCsvs
{
    public static class Program
    {

        public static int GetIterationNumber(this string line) => new string(line.TakeWhile(char.IsDigit).ToArray()).TryParseInt().ValueOrElse(-1);

        public static void Main(string[] args)
        {
            Console.Write("Enter source dir: ");
            var sourceDir = Console.ReadLine();
            if (!Directory.Exists(sourceDir))
            {
                Console.WriteLine("Directory doesnt exist");
                return;
            }

            var sourceFiles = Directory.EnumerateFiles(sourceDir)
                                       .Where(f => f.EndsWith(".csv"))
                                       .ToArray();

            var header = File.ReadAllLines(sourceFiles.First()).First();
            var resultPath = Path.Combine(sourceDir, "combined.csv");
            int i = 0;
            using (var resultFile = File.CreateText(resultPath))
            {
                resultFile.WriteLine(header);
                foreach (var sourceFile in sourceFiles)
                {
                    Console.WriteLine(++i + "/" + sourceFiles.Length);
                    var lines = File.ReadAllLines(sourceFile);
                    //var releventIteration = lines.First(IsRelevent).GetIterationNumber();
                    var releventIteration = lines.Last().GetIterationNumber();
                    foreach (var line in lines.Where(l => l.GetIterationNumber() == releventIteration))
                        resultFile.WriteLine(line);
                        
                }

            }
            
            Process.Start(resultPath);
        }

        private static bool IsRelevent(string line)
        {
            var tokens = line.Split(ArrayUtils.Init(','), StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 8)
                return false;
            return tokens[1].Equals("Oracle") && tokens[7].TryParseInt().Map(fullSyncs => fullSyncs == 10).ValueOrElse(false);
        }
    }
}
