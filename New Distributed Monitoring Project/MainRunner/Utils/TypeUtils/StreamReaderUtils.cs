using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utils.TypeUtils
{
    public static class StreamReaderUtils
    {
        private static readonly int BlockSize = 2048;

        public static IEnumerator<char> EnumarateChars(this StreamReader @this)
        {
            var charArray  = new char[BlockSize];
            var amountRead = 0;
            do
            {
                amountRead = @this.ReadBlock(charArray, 0, charArray.Length);
                for (int i = 0; i < amountRead; i++)
                    yield return charArray[i];
            } while (amountRead == BlockSize);
        }

        public static IEnumerator<string> EnumarateWords(this StreamReader @this)
        {
            StringBuilder currentWord = new StringBuilder();
            foreach (var ch in EnumarateChars(@this).ToIEnumerable())
                switch (ch)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        yield return currentWord.ToString();
                        currentWord.Clear();
                        break;
                    default:
                        currentWord.Append(ch);
                        break;
                }
        }

        public static IEnumerable<string> ReadLines(this Stream stream)
        {
            string line;
            using (var reader = new StreamReader(stream))
                while ((line = reader.ReadLine()) != null)
                    yield return line;
        }


        public static bool IsEOF(this BinaryReader binaryReader) 
            => (binaryReader.BaseStream.Position == binaryReader.BaseStream.Length);
    }
}
