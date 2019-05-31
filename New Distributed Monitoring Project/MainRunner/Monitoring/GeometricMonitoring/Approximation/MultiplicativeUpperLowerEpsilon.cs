namespace Monitoring.GeometricMonitoring.Approximation
{
    public sealed class MultiplicativeUpperLowerApproximation : ApproximationType
    {
        public double LowerApproximationValue { get; }
        public double UpperApproximationValue { get; }

        public MultiplicativeUpperLowerApproximation(double lowerApproximationValue, double upperApproximationValue)
        {
            LowerApproximationValue = lowerApproximationValue;
            UpperApproximationValue = upperApproximationValue;
        }

        public override string AsString() => $"Lower{LowerApproximationValue}Upper{UpperApproximationValue}";

        public override (double lowerThresh, double upperThresh) Calc(double currentValue)
        {
            if (currentValue > 0)
                return (currentValue * LowerApproximationValue, currentValue * UpperApproximationValue);
            else
                return (currentValue * UpperApproximationValue, currentValue * LowerApproximationValue);
        }
    }
}
