using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public static class Serialization
    {
        private static readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public static void SerializeTo<T>(this T @this, string filePath)
        {
            using (var stream = File.Create(filePath))
                binaryFormatter.Serialize(stream, @this);
        }
        public static T Deserialize<T>(this string filePath)
        {
            using (var stream = File.OpenRead(filePath))
                return (T)binaryFormatter.Deserialize(stream);
        }
    }
}
