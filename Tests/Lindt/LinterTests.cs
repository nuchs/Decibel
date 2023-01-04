using Lindt;
using Mast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Lindt;
internal class LinterTests
{

    [Test]
    [TestCase("Unreferenced")]
    [TestCase("Unresolved")]
    public void ChecksEnabledByDefault(string check)
    {
        Linter sut = new();

        Assert.That(sut[check], Is.True);
    }

    [Test]
    [TestCase("Unreferenced")]
    [TestCase("Unresolved")]
    public void EnabledChecksRun(string check)
    {
        Linter sut = new();
        var db = new DbBuilder().AddFromTsqlScript("CREATE TYPE DBO.Bob FROM INT").Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results.Select(r => r.CheckName), Has.Member(check));
    }

    [Test]
    [TestCase("Unreferenced")]
    [TestCase("Unresolved")]
    public void DisabledChecksNotRun(string check)
    {
        Linter sut = new();
        sut[check] = false;
        var db = new DbBuilder().AddFromTsqlScript("CREATE TYPE DBO.Bob FROM INT").Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results.Select(r => r.CheckName), Has.No.Member(check));
    }
}
