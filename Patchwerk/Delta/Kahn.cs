using System.Security.Policy;

namespace Patchwerk.Delta;

// Kahn's algorithm can be used to sort sets with a partial order
// (See https://en.wikipedia.org/wiki/Topological_sorting)
internal sealed class Kahn<N, C>
    where N : class
{
    private readonly HashSet<Edge> edges = new();
    private readonly HashSet<Node> nodes = new();

    internal void AddNode(N nodeId, C content, IEnumerable<N> children)
    {
        nodes.Add(new(nodeId, content));
        edges.UnionWith(children.Select(childId => new Edge(nodeId, childId)));
    }

    // All variable names are taken from the wiki page, so it should be fairly
    // easy to marry up whats happening
    internal IEnumerable<C> Sort()
    {
        Prune();
        List<C> L = new();
        HashSet<Node> S = new(nodes.Where(n => edges.All(e => e.To != n.Id)));

        while(S.Any())
        {
            var n = S.First();
            S.Remove(n);

            L.Add(n.Content);

            foreach (var edge in edges.Where(e => e.From == n.Id))
            {
                edges.Remove(edge);
                var m = edge.To;

                if (edges.All(e => e.To != m))
                {
                    S.Add(nodes.First(n => n.Id == m));
                }
            }
        }

        return L;
    }

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
