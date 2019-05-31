using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TextualUtils
{
    public sealed class PathBuilder
    {
        public string DirectoryPath { get; }
        public string FileTitle { get; }
        public List<(string, string)> FileProperties { get; }

        public PathBuilder(string directoryPath, string fileTitle, List<(string, string)> fileProperties)
        {
            DirectoryPath = directoryPath;
            FileTitle = fileTitle;
            FileProperties = fileProperties;
        }

        public static PathBuilder Create(string directoryPath, string fileTitle)
            => new PathBuilder(directoryPath, fileTitle, new List<(string, string)>());

        public PathBuilder AddProperty(string propertyName, string propertyValue)
        {
            FileProperties.Add((propertyName, propertyValue));
            return this;
        }

        public string ToPath(string extensionWithoutDot)
        {
            StringBuilder str = new StringBuilder();
            str.Append(Path.Combine(DirectoryPath, FileTitle));
            foreach (var (propertyName, propertyValue) in FileProperties)
                str.AppendFormat("__{0}_{1}", propertyName, propertyValue);
            str.Append(".")
               .Append(extensionWithoutDot);
            return str.ToString();
        }
    }
}
