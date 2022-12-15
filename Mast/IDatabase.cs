using Mast.Dbo;
using Mast.Parsing;

namespace Mast;

public interface IDatabase
{
    IEnumerable<Function> Functions { get; }

    IEnumerable<StoredProcedure> Procedures { get; }

    IEnumerable<ScalarType> ScalarTypes { get; }

    IEnumerable<Schema> Schemas { get; }

    IEnumerable<Table> Tables { get; }

    IEnumerable<TableType> TableTypes { get; }

    IEnumerable<Trigger> Triggers { get; }

    IEnumerable<Reference> UnresolvedReferences { get; }

    IEnumerable<User> Users { get; }

    IEnumerable<View> Views { get; }
}