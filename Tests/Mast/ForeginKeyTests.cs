using Mast.Dbo;

namespace Tests.Mast;

public class ForeignKeyTests : BaseMastTest
{
    [Test]
    public void ColumnFromColumnConstraint()
    {
        var expected = "col1";
        var script = $"CREATE TABLE dbo.stub ({expected} int references fstub (fcol))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First().ForeginKey;

        Assert.That(result?.Column.Name, Is.EqualTo(expected));
    }

    [Test]
    public void ColumnFromTableConstraint()
    {
        var expected = "col1";
        var script = $"CREATE TABLE dbo.stub ({expected} int, foreign key ({expected}) references fstub (fcol))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().ForeignKeys.First();

        Assert.That(result?.Column.Name, Is.EqualTo(expected));
    }

    [Test]
    public void Content()
    {
        var expected = "foreign key (stub) references fstub (fcol)";
        var script = $"CREATE TABLE dbo.stub (stub int, {expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().ForeignKeys.First();

        Assert.That(result?.Content, Is.EqualTo(expected));
    }

    [Test]
    public void ForeginColumn()
    {
        var expected = "fcol";
        var script = $"CREATE TABLE dbo.stub (stub int references fstub ({expected}))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First().ForeginKey;

        Assert.That(result?.ForeignColumn, Is.EqualTo(expected));
    }

    [Test]
    public void ForeignTable()
    {
        var expected = "fstub";
        var script = $"CREATE TABLE dbo.stub (stub int references {expected} (fcol))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First().ForeginKey;

        Assert.That(result?.ForeignTable, Is.EqualTo(expected));
    }

    [Test]
    public void Name()
    {
        var expected = "fk1";
        var script = $"CREATE TABLE dbo.stub (stub int, constraint {expected} foreign key (stub) references fstub (fcol))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().ForeignKeys.First();

        Assert.That(result?.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", ChangeAction.NotSet)]
    [TestCase("on delete no action", ChangeAction.NoAction)]
    [TestCase("on delete cascade", ChangeAction.Cascade)]
    [TestCase("on delete set default", ChangeAction.SetDefault)]
    [TestCase("on delete set null", ChangeAction.SetNull)]
    public void OnDelete(string action, ChangeAction expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub int references ftab (fcol) {action})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First().ForeginKey;

        Assert.That(result?.OnDelete, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", ChangeAction.NotSet)]
    [TestCase("on update no action", ChangeAction.NoAction)]
    [TestCase("on update cascade", ChangeAction.Cascade)]
    [TestCase("on update set default", ChangeAction.SetDefault)]
    [TestCase("on update set null", ChangeAction.SetNull)]
    public void OnUpdate(string action, ChangeAction expected)
    {
        var script = $"CREATE TABLE dbo.stub (stub int references ftab (fcol) {action})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First().ForeginKey;

        Assert.That(result?.OnUpdate, Is.EqualTo(expected));
    }
}
