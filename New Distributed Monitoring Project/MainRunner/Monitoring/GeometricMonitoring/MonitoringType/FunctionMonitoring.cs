using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.GeometricMonitoring.MonitoringType
{
    public abstract partial class MonitoringScheme
    {
        public sealed class FunctionMonitoring : MonitoringScheme, IEquatable<FunctionMonitoring>
        {
            public override string AsString() => "FGM";

            public bool Equals(FunctionMonitoring other) => true;

            public override int GetHashCode() => typeof(Value).GetHashCode();

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is FunctionMonitoring && Equals((FunctionMonitoring) obj);
            }
        }
    }
}
