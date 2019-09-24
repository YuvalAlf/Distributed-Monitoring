using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombineCsvs
{
    public static class Program
    {
        public const string SourceDir = @"C:\Users\Yuval\Desktop\Synthetic Results\# Nodes\Inner Product";
        public const string DestinationDir = SourceDir;

        public static int GetIterationNumber(this string line) => int.Parse(new string(line.TakeWhile(char.IsDigit).ToArray()));

        public static void Main(string[] args)
        {
            var sourceFiles = Directory.EnumerateFiles(SourceDir)
                                       .Where(f => f.EndsWith(".csv"))
                                       .ToArray();

            var header = File.ReadAllLines(sourceFiles.First()).First();
            var resultPath = Path.Combine(DestinationDir, "combined.csv");
            using (var resultFile = File.CreateText(resultPath))
            {
                resultFile.WriteLine(header);
                foreach (var sourceFile in sourceFiles)
                {
                    var lines = File.ReadAllLines(sourceFile);
                    var token = lines.Last().GetIterationNumber();
                    for (int i = lines.Length - 1; lines[i].GetIterationNumber() == token; i--)
                        resultFile.WriteLine(lines[i]);
                }
            }

            Process.Start(resultPath);
        }
    }
}
