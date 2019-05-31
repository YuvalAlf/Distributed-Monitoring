namespace Monitoring.GeometricMonitoring.Approximation
{
    public sealed class MultiplicativeApproximation : ApproximationType
    {
        public double ApproximationValue { get; }

        public MultiplicativeApproximation(double approximationValue)
        {
            ApproximationValue = approximationValue;
        }

        public override string AsString() => "Multiplicative" + ApproximationValue;

        public override (double lowerThresh, double upperThresh) Calc(double currentValue)
        {
            if (currentValue > 0)
                return (currentValue * (1 - ApproximationValue), currentValue * (1 + ApproximationValue));
            else
                return (currentValue * (1 + ApproximationValue), currentValue * (1 - ApproximationValue));
        }
    }
}
