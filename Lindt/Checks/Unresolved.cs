using Mast;

namespace Lindt.Checks;

internal class Unresolved : ICheck
{
    public bool Enabled { get; set; } = true;

    public IEnumerable<Result> Check(IDatabase db)
    {
        List<Result> results = new();

        foreach (var item in db.UnresolvedReferences)
        {
            results.Add(new(
                nameof(Unresolved),
                Level.Error,
                $"{item.Referent} is refered to by {item.Referee.Identifier} but it does not exist"));
        }

        return results;
    }
}
