namespace Patchwerk.Delta;

// Kahn's algorithm can be used to sort sets with a partial order (See https://en.wikipedia.org/wiki/Topological_sorting)
internal sealed class Kahn<N, C>
    where N : class
{
    private readonly HashSet<Edge> edges = new();
    private readonly HashSet<Node> nodes = new();

    internal bool AddNode(N nodeId, C content, IEnumerable<N> children)
    {
        if (nodes.Any(n => n.Id == nodeId))
        {
            return false;
        }

        nodes.Add(new(nodeId, content));
        edges.UnionWith(children.Select(childId => new Edge(nodeId, childId)));

        return true;
    }

    // All variable names are taken from the wiki page, so it should be fairly
    // easy to marry up whats happening
    internal IEnumerable<C> Sort()
    {
        Prune();
        List<C> L = new();
        HashSet<Node> S = new(nodes.Where(n => NoIncomingEdges(n.Id)));

        while (S.Any())
        {
            var n = S.First();
            S.Remove(n);

            L.Add(n.Content);

            foreach (var edge in OutgoingEdgesFrom(n))
            {
                edges.Remove(edge);
                var m = edge.To;

                if (NoIncomingEdges(m))
                {
                    S.Add(nodes.First(n => n.Id == m));
                }
            }
        }

        if (edges.Any())
        {
            throw new InvalidOperationException("Db objects contain cyclic dependency, not possible to patch");
        }

        return L;
    }

    private bool NoIncomingEdges(N m) => edges.All(e => e.To != m);

    private IEnumerable<Kahn<N, C>.Edge> OutgoingEdgesFrom(Kahn<N, C>.Node n) => edges.Where(e => e.From == n.Id);

    // Remove any edges whose terminus is not in the set being sorted (as all of
    // the nodes in the set represent one type of DB Object this can happen when
    // you have something like a function referencing a different type of
    // object, like a view)
    private void Prune()
    {
        List<Edge> remove = new();

        foreach (var edge in edges)
        {
            if (!nodes.Any(n => n.Id == edge.To))
            {
                remove.Add(edge);
            }
        }

        edges.ExceptWith(remove);
    }

    private record Node(N Id, C Content);

    private record Edge(N From, N To);
}
