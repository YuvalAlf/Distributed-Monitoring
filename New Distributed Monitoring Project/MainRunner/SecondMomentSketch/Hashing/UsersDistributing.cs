using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;
using Utils.MathUtils;

namespace SecondMomentSketch.Hashing
{
    public sealed class UsersDistributing
    {
        public string Name { get; }
        public Func<int, int, int, int> DistributeFunc { get; }

        public UsersDistributing(string name, Func<int, int, int, int> distributeFunc)
        {
            Name = name;
            DistributeFunc = distributeFunc;
        }

        public static UsersDistributing RoundRobin()
            => new UsersDistributing("Round_Robin", (node, userId, numOfNodes) => (node + 1) % numOfNodes);

        public static UsersDistributing HashUsers()
            => new UsersDistributing("Uniform_Hashing", (node, userId, numOfNodes) => userId % numOfNodes);

        public static UsersDistributing UnevenHashing()
        {
            return new UsersDistributing("Uneven_hashing", (node, userId, numOfNodes) =>
                                                           {
                                                               var rnd   = new Random(userId);
                                                               var count = 0;
                                                               while (rnd.NextBoolean() && rnd.NextBoolean())
                                                               {
                                                                   count++;
                                                               }

                                                               return count % numOfNodes;
                                                           });
        }
    }
}
