namespace Monitoring.GeometricMonitoring.Epsilon
{
    public sealed class AdditiveEpsilon : EpsilonType
    {
        public AdditiveEpsilon(double epsilonValue) : base(epsilonValue)
        {
        }

        public override string AsString() => "Additive " + EpsilonValue;

        public override (double lowerThresh, double upperThresh) Calc(double currentValue) =>
            (currentValue - EpsilonValue, currentValue + EpsilonValue);
    }
}
