using System;

namespace Monitoring.GeometricMonitoring.MonitoringType
{
    public abstract partial class MonitoringScheme
    {
        public sealed class Value : MonitoringScheme, IEquatable<Value>
        {
            public override string AsString() => "Value";

            public bool Equals(Value other) => true;

            public override int GetHashCode() => typeof(Value).GetHashCode();

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is Value && Equals((Value) obj);
            }

        }
    }
}
