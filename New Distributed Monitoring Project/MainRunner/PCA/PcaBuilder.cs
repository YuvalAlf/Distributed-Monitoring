using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Analysis;
using MoreLinq;

namespace PCA
{
    public sealed class PcaBuilder
    {
        private LinkedList<double[]> Samples { get; }

        private PcaBuilder(LinkedList<double[]> samples) => Samples = samples;

        public static PcaBuilder Create() => new PcaBuilder(new LinkedList<double[]>());
        public void Add(double[] sample) => Samples.AddLast(sample);

        public double[] Eigenvalues()
        {
            var pca = new PrincipalComponentAnalysis();
            pca.Learn(Samples.ToArray());

            return pca.Eigenvalues.OrderByDescending(x => x).ToArray();
        }

        public static PcaBuilder Combine(params PcaBuilder[] builders)
        {
            var samples = new LinkedList<double[]>();
            foreach (var builder in builders)
                builder.Samples.ForEach(s => samples.AddLast(s));
            return new PcaBuilder(samples);
        }
    }
}
