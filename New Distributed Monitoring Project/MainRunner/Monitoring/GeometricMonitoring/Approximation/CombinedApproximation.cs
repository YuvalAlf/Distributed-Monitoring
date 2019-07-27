using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.TypeUtils;

namespace Monitoring.GeometricMonitoring.Approximation
{
    public sealed class CombinedApproximation : ApproximationType
    {
        private ApproximationType FirstApproximmation { get; }
        private ApproximationType[] InnerApproximmations { get; }

        public CombinedApproximation(ApproximationType          firstApproximation,
                                     params ApproximationType[] innerApproximmations)
        {
            FirstApproximmation = firstApproximation;
            InnerApproximmations = innerApproximmations;
        }

        public override string AsString()
        {
            return "Combined_" + FirstApproximmation.AsString() + "_" + string.Join("_", InnerApproximmations.Select(i => i.AsString()));
        }

        public override (double lowerThresh, double upperThresh) Calc(double currentValue)
        {
            var (lower, upper) = FirstApproximmation.Calc(currentValue);

            foreach (var approximation in InnerApproximmations)
            {
                lower = approximation.Calc(lower).lowerThresh;
                upper = approximation.Calc(upper).upperThresh;
            }

            return (lower, upper);
        }
    }
}
