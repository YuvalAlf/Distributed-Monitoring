namespace Monitoring.GeometricMonitoring.Approximation
{
    public abstract class ApproximationType
    {
        public abstract string AsString();

        public abstract (double lowerThresh, double upperThresh) Calc(double currentValue);
    }
}
