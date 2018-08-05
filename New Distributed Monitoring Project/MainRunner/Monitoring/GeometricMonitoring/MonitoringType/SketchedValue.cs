using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.GeometricMonitoring.MonitoringType
{
    public abstract partial class MonitoringScheme
    {
        public sealed class SketchedValue : MonitoringScheme, IEquatable<SketchedValue>
        {
            public override string AsString() => "Sketched Value Scheme";

            public bool Equals(SketchedValue other) => true;

            public override int GetHashCode() => typeof(SketchedValue).GetHashCode();

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is SketchedValue && Equals((SketchedValue)obj);
            }

        }
    }
}
