using System;
using Monitoring.Data;
using Monitoring.Nodes;
using Utils.AiderTypes;

namespace Monitoring.Servers
{
    public delegate Either<(NodeServer<TNode>, Communication), Communication> ResolveNodesFunction<TNode>
        (NodeServer<TNode> server, TNode[] nodes, Random rnd)
        where TNode: AbstractNode;
}