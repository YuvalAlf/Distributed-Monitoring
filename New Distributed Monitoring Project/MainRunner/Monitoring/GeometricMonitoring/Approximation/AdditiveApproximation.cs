namespace Monitoring.GeometricMonitoring.Approximation
{
    public sealed class AdditiveApproximation : ApproximationType
    {
        public double ApproximationValue { get; }

        public AdditiveApproximation(double approximationValue) => ApproximationValue = approximationValue;

        public override string AsString() => "Additive" + ApproximationValue;

        public override (double lowerThresh, double upperThresh) Calc(double currentValue) =>
            (currentValue - ApproximationValue, currentValue + ApproximationValue);
    }
}
