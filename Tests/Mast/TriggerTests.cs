using Mast.Dbo;

namespace Tests.Mast;

public class TriggerTests : BaseMastTest
{
    [Test]
    public void Content()
    {
        var expected = $"CREATE TRIGGER dbo.trig on dbo.tab after insert, update as select 1";

        parser.Parse(db, expected);
        var result = db.Triggers.First();

        Assert.That(result.Content, Is.EqualTo(expected));
    }

    [Test]
    public void Name()
    {
        var expected = "fred";
        var trigger = $"CREATE TRIGGER dbo.{expected} on dbo.tab after insert as select 1";

        parser.Parse(db, trigger);
        var result = db.Triggers.First();

        Assert.That(result.Name, Is.EqualTo(expected));
    }

    [Test]
    public void Schema()
    {
        var expected = "fred";
        var trigger = $"CREATE TRIGGER {expected}.stub on dbo.tab after insert as select 1";

        parser.Parse(db, trigger);
        var result = db.Triggers.First();

        Assert.That(result.Schema, Is.EqualTo(expected));
    }

    [Test]
    public void Target()
    {
        var expected = "dbo.fred";
        var trigger = $"CREATE TRIGGER dbo.stub on {expected} after insert as select 1";

        parser.Parse(db, trigger);
        var result = db.Triggers.First();

        Assert.That(result.Target, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("INSERT", TriggeredBy.Insert)]
    [TestCase("UPDATE", TriggeredBy.Update)]
    [TestCase("DELETE", TriggeredBy.Delete)]
    public void TriggerActions(string action, TriggeredBy expected)
    {
        var trigger = $"CREATE TRIGGER dbo.stub on dbo.tab after {action} as select 1";

        parser.Parse(db, trigger);
        var result = db.Triggers.First();

        Assert.That(result.TriggerActions.First(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("FOR", RunWhen.After)]
    [TestCase("AFTER", RunWhen.After)]
    [TestCase("INSTEAD OF", RunWhen.Instead)]
    public void When(string when, RunWhen expected)
    {
        var trigger = $"CREATE TRIGGER dbo.stub on dbo.tab {when} insert as select 1";

        parser.Parse(db, trigger);
        var result = db.Triggers.First();

        Assert.That(result.When, Is.EqualTo(expected));
    }
}
