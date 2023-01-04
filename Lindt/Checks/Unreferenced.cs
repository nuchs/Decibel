using Mast;
using Mast.Dbo;

namespace Lindt.Checks;

internal class Unreferenced : ICheck
{
    public bool Enabled { get; set; } = true;

    public IEnumerable<Result> Check(IDatabase db)
    {
        List<Result> results = new();

        results.AddRange(FindUnreferencedObjects(db.Functions, "Function"));
        results.AddRange(FindUnreferencedObjects(db.Schemas, "Schema"));
        results.AddRange(FindUnreferencedObjects(db.ScalarTypes, "Scalar type"));
        results.AddRange(FindUnreferencedObjects(db.Tables, "Table"));
        results.AddRange(FindUnreferencedObjects(db.TableTypes, "Table type"));
        results.AddRange(FindUnreferencedObjects(db.Views, "View"));

        return results;
    }

    private static IEnumerable<Result> FindUnreferencedObjects(IEnumerable<DbObject> dbos, string type)
        => from item in dbos
           where !item.ReferencedBy.Any()
           select new Result(
                nameof(Unreferenced),
                Level.Warn,
                $"{type} {item.Identifier} is not referenced and could potentially be removed");
    
}
