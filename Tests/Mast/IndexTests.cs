﻿using Mast.Dbo;
using Microsoft.SqlServer.Dac.Model;

namespace Tests.Mast;

public class IndexTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var expected = "index idx_1 (stub)";
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int, {expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Indices.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    public void ColumnIndex()
    {
        var expected = "index idx_1";
        var script = $"CREATE TABLE dbo.stub (stub int {expected})";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Columns.First().Index;

        Assert.That(result?.Content, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("idx", "idx")]
    [TestCase("[idx]", "idx")]
    public void Name(string name, string expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int, index {name} (stub))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Indices.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", false)]
    [TestCase(" NONCLUSTERED ", false)]
    [TestCase(" CLUSTERED ", true)]
    public void Clustered(string clustered, bool expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int, index idx{clustered}(stub))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Indices.First();

        Assert.That(result.Clustered, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", Direction.NotSet)]
    [TestCase("asc", Direction.Asc)]
    [TestCase("desc", Direction.Desc)]
    public void SortOrder(string sortOrder, Direction expected)
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int, index idx(stub {sortOrder}))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Indices.First().Columns.First();

        Assert.That(result.SortOrder, Is.EqualTo(expected));
    }

    [Test]
    public void SingleColumnCount()
    {
        var script = $"CREATE TYPE dbo.stub AS TABLE (stub int, index idx(stub))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Indices.First();

        Assert.That(result.Columns.Count(), Is.EqualTo(1));
    }

    [Test]
    public void SingleColumn()
    {
        var expected = "stub";
        var script = $"CREATE TYPE dbo.stub AS TABLE ({expected} int, index idx(stub))";

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.TableTypes.First().Indices.First();

        Assert.That(result.Columns.First().Column.Name, Is.EqualTo(expected));
    }

    [Test]
    public void CompoundColumnCount()
    {
        var script = """
            CREATE TABLE dbo.stub
            (
                stub1 int, 
                stub2 int,
                index idx (stub1, stub2)
            )
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Indices.First();

        Assert.That(result.Columns.Count(), Is.EqualTo(2));
    }

    [Test]
    public void CompoundColumn()
    {
        var script = $"""
            CREATE TABLE dbo.stub
            (
                col1 int, 
                col2 int,
                index idx (col1, col2)
            )
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Indices.First();

        Assert.That(
            result.Columns.Select(ic => ic.Column.Name),
            Is.EqualTo(new[] { "col1", "col2" }));
    }

    [Test]
    public void MultipleSortOrder()
    {
        var script = $"""
            CREATE TABLE dbo.stub
            (
                stub1 int, 
                stub2 int,
                stub3 int,
                index idx (stub1 asc, stub2 desc, stub3)
            )
            """;

        var db = dbBuilder.AddFromTsqlScript(script).Build();
        var result = db.Tables.First().Indices.First();

        Assert.That(
            result.Columns.Select(ic => ic.SortOrder),
            Is.EqualTo(new[] { Direction.Asc, Direction.Desc, Direction.NotSet }));
    }
}
