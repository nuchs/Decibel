namespace Lindt;

public class ResultSet
{
    private List<Result> results = new();

    public IEnumerable<Result> Results => results;

    internal void AddResults(IEnumerable<Result> checkResults)
    {
        results.AddRange(checkResults);
    }
}
