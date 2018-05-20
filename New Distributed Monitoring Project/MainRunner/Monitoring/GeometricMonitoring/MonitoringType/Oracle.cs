using System;

namespace Monitoring.GeometricMonitoring.MonitoringType
{
    public abstract partial class MonitoringScheme
    {
        public sealed class Oracle : MonitoringScheme, IEquatable<Oracle>
        {
            public override string AsString() => "Oracle Scheme";

            public bool Equals(Oracle other) => true;

            public override int GetHashCode() => typeof(Oracle).GetHashCode();

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is Oracle && Equals((Oracle) obj);
            }

        }
    }
}
