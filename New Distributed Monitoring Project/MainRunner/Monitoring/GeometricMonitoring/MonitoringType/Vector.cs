using System;

namespace Monitoring.GeometricMonitoring.MonitoringType
{
    public abstract partial class MonitoringScheme
    {
        public sealed class Vector : MonitoringScheme, IEquatable<Vector>
        {
            public override string AsString() => "Vector";

            public bool Equals(Vector other) => true;

            public override int GetHashCode() => typeof(Vector).GetHashCode();

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is Vector && Equals((Vector) obj);
            }

        }
    }
}
