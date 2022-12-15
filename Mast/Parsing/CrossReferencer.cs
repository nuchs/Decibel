using Mast.Dbo;

namespace Mast.Parsing;

internal sealed class CrossReferencer
{
    private readonly Database db;

    internal CrossReferencer(Database db) => this.db = db;

    internal void Run()
    {
        AddReferencesFrom(db.ScalarTypes);
        AddReferencesFrom(db.Tables);
        AddReferencesFrom(db.TableTypes);
        AddReferencesFrom(db.Views);
        AddReferencesFrom(db.Functions);
        AddReferencesFrom(db.Procedures);
        AddReferencesFrom(db.Triggers);
        AddReferencesFrom(db.Users);
    }

    private void AddReferencesFrom(IEnumerable<DbObject> objectList)
    {
        foreach (var dbObject in objectList)
        {
            dbObject.CrossReference(db);
        }
    }
}
