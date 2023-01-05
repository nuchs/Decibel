using Mast;

namespace Lindt.Checks;

internal class Unresolved : ICheck
{
    public bool Enabled { get; set; } = true;

    public IEnumerable<Result> Check(IDatabase db)
        => from item in db.UnresolvedReferences
           select new Result(
                nameof(Unresolved),
                Level.Error,
                $"{item.Referent} is refered to by {item.Referee.Identifier} but it does not exist");
}
