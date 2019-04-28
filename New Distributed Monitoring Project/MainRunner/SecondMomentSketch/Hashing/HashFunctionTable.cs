using System;
using Utils.TypeUtils;

namespace SecondMomentSketch.Hashing
{
    public sealed class HashFunctionTable
    {
        public Func<int, int>[] HashFunctions { get; }

        public HashFunctionTable(Func<int, int>[] hashFunctions) => HashFunctions = hashFunctions;

        public Func<int, int> this[int index] => HashFunctions[index];

        public static HashFunctionTable[] Init(int numOfNodes, int vectorLength, HashGenerator hashFunctionGenerator)
        {
            return ArrayUtils.Init(numOfNodes, nodeIndex =>
                                                   CreateHashFunctionTableForNode(nodeIndex,
                                                                                  vectorLength,
                                                                                  hashFunctionGenerator));
        }

        private static HashFunctionTable CreateHashFunctionTableForNode(int           nodeIndex,
                                                                        int           vectorLength,
                                                                        HashGenerator hashFunctionGenerator)
        {
            var hashFunctions = ArrayUtils.Init(vectorLength,
                                                vectorIndex => hashFunctionGenerator.GenerateHash(nodeIndex, vectorIndex));
            return new HashFunctionTable(hashFunctions);
        }
    }
}
