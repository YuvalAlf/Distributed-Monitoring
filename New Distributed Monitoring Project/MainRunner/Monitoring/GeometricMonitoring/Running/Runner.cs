using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;

namespace Monitoring.GeometricMonitoring.Running
{
    public abstract class Runner
    {
        public AccumaltedResult AccumalatedResult { get; protected set; }

        protected Runner(AccumaltedResult accumalatedResult) => AccumalatedResult = accumalatedResult;

        public abstract AccumaltedResult Run(Vector<double>[] change, Random rnd);

        public IEnumerable<AccumaltedResult> RunAll(IEnumerable<Vector<double>[]> changes, Random rnd) 
            => changes.Select(change => this.Run(change, rnd));

        public AccumaltedResult RunToEnd(IEnumerable<Vector<double>[]> changes, Random rnd) 
            => RunAll(changes, rnd).Last();
    }
}
