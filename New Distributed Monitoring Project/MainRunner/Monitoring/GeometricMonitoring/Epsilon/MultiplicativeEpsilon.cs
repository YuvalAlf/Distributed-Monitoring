namespace Monitoring.GeometricMonitoring.Epsilon
{
    public sealed class MultiplicativeEpsilon : EpsilonType
    {
        public MultiplicativeEpsilon(double epsilonValue) : base(epsilonValue)
        {
        }

        public override string AsString() => "Multiplicative " + EpsilonValue;

        public override (double lowerThresh, double upperThresh) Calc(double currentValue)
        {

            if (currentValue > 0)
                return (currentValue * (1 - EpsilonValue), currentValue * (1 + EpsilonValue));
            else
                return (currentValue * (1 + EpsilonValue), currentValue * (1 - EpsilonValue));
        }
    }
}
