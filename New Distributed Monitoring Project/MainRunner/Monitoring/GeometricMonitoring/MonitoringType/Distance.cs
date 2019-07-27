using System;

namespace Monitoring.GeometricMonitoring.MonitoringType
{
    public abstract partial class MonitoringScheme
    {
        public sealed class Distance : MonitoringScheme, IEquatable<Distance>
        {
            public int Norm { get; }

            public Distance(int norm) => Norm = norm;

            public override string AsString() => "Dist L" + (Norm != 0 ? Norm.ToString() : "-inf");

            public bool Equals(Distance other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Norm == other.Norm;
            }

            public override int GetHashCode() => Norm;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is Distance && Equals((Distance) obj);
            }

        }
    }
}
