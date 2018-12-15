using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Utils.SparseTypes;

namespace Monitoring.GeometricMonitoring.Running
{
    public abstract class Runner
    {
        public AccumaltedResult AccumalatedResult { get; protected set; }

        protected Runner(AccumaltedResult accumalatedResult) => AccumalatedResult = accumalatedResult;

        public abstract AccumaltedResult Run(Vector[] change, Random rnd);

        public IEnumerable<AccumaltedResult> RunAll(IEnumerable<Vector[]> changes, Random rnd) 
            => changes.Select(change => this.Run(change, rnd));

        public AccumaltedResult RunToEnd(IEnumerable<Vector[]> changes, Random rnd) 
            => RunAll(changes, rnd).Last();
    }
}
