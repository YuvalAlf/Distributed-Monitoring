using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.Statistics.Analysis;

namespace Utils.MathUtils.Sketches
{
    public sealed class PcaBuilder : IDisposable
    {
        private LinkedList<double[]> Data { get; set; } = new LinkedList<double[]>();

        public void AddData(double[] vector) => Data.AddLast(vector);

        public SketchFunction GetPcaSketchFunction()
        {
            var pca = new PrincipalComponentAnalysis();
            var transform = pca.Learn(Data.ToArray());
            return new PcaSketchFunction(transform, transform.Inverse());
        }

        public void Dispose()
        {
            Data = null;
        }
    }
}
