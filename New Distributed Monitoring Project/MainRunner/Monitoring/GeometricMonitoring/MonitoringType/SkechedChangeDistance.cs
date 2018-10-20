using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.GeometricMonitoring.MonitoringType
{
    public abstract partial class MonitoringScheme
    {
        public sealed class SketchedChangeDistance : MonitoringScheme, IEquatable<SketchedChangeDistance>
        {
            public string SketchName { get; }
            public int norm { get; }

            public SketchedChangeDistance(string sketchName, int norm)
            {
                SketchName = sketchName;
                this.norm = norm;
            }

            public override string AsString() => "SKD";

            public bool Equals(SketchedChangeDistance other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(SketchName, other.SketchName) && norm == other.norm;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is SketchedChangeDistance && Equals((SketchedChangeDistance) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((SketchName != null ? SketchName.GetHashCode() : 0) * 397) ^ norm;
                }
            }
        }
    }
}