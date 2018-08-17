using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.GeometricMonitoring.MonitoringType
{
    public abstract partial class MonitoringScheme
    {
        public sealed class SketchedChangeValue : MonitoringScheme, IEquatable<SketchedChangeValue>
        {
            public SketchedChangeValue(string sketchName) => SketchName = sketchName;

            public string SketchName { get; }

            public override string AsString() => SketchName + " Sketched Change Value Scheme";

            public bool Equals(SketchedChangeValue other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(SketchName, other.SketchName);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is SketchedChangeValue && Equals((SketchedChangeValue) obj);
            }

            public override int GetHashCode()
            {
                return (SketchName != null ? SketchName.GetHashCode() : 0);
            }
        }
    }
}
