using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.GeometricMonitoring.MonitoringType
{
    public abstract partial class MonitoringScheme
    {
        public sealed class OracleVector : MonitoringScheme, IEquatable<OracleVector>
        {
            public override string AsString() => "Oracle Vector";

            public bool Equals(OracleVector other) => true;

            public override int GetHashCode() => typeof(OracleVector).GetHashCode();

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is OracleVector vector && Equals(vector);
            }

        }
    }
}
