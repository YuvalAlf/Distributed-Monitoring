namespace Monitoring.GeometricMonitoring.Approximation
{
    public sealed class ThresholdApproximation : ApproximationType
    {
        public double Threshold { get; }

        public ThresholdApproximation(double threshold)
        {
            Threshold = threshold;
        }

        public override string AsString() => "Threshold" + Threshold;

        public override (double lowerThresh, double upperThresh) Calc(double currentValue)
        {
            if (currentValue < Threshold)
                return (double.NegativeInfinity, Threshold);
            else
                return (Threshold, double.PositiveInfinity);

        }
    }
}
