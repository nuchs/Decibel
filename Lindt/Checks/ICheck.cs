using Mast;

namespace Lindt.Checks;

internal interface ICheck
{
    IEnumerable<Result> Check(IDatabase db);

    bool Enabled { get; set; }
}
