namespace Monitoring.GeometricMonitoring.Epsilon
{
    public abstract class EpsilonType
    {
        public double EpsilonValue { get; }

        protected EpsilonType(double epsilonValue) => EpsilonValue = epsilonValue;

        public abstract string AsString();

        public abstract (double lowerThresh, double upperThresh) Calc(double currentValue);
    }
}
