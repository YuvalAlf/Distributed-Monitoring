using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.GeometricMonitoring.MonitoringType
{
    public abstract partial class MonitoringScheme
    {
        public sealed class Naive : MonitoringScheme, IEquatable<Naive>
        {
            public override string AsString() => "Naive";

            public bool Equals(Naive other) => true;

            public override int GetHashCode() => typeof(Naive).GetHashCode();

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is Naive && Equals((Naive)obj);
            }

        }
    }
}
