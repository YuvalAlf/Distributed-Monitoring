namespace Monitoring.GeometricMonitoring.Epsilon
{
    public sealed class ThresholdEpsilon : EpsilonType
    {
        public double Threshold => EpsilonValue;

        public ThresholdEpsilon(double epsilonValue) : base(epsilonValue)
        {
        }

        public override string AsString() => "Threshold " + Threshold;

        public override (double lowerThresh, double upperThresh) Calc(double currentValue)
        {
            if (currentValue < Threshold)
                return (double.NegativeInfinity, Threshold);
            else
                return (Threshold, double.PositiveInfinity);

        }
    }
}
