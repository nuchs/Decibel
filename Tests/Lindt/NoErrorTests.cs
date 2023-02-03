using Lindt;
using Mast;

namespace Tests.Lindt;

public class NoErrorTests
{
    private readonly Linter sut = new();

    [Test]
    public void ValidFunctions()
    {
        var script = """
            CREATE FUNCTION bob(@a int, @b int) RETURNS TABLE RETURN SELECT 1
            GO
            CREATE PROCEDURE pob AS BEGIN SELECT bob() END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidProcedures()
    {
        var script = "CREATE PROCEDURE pob @a int AS BEGIN SELECT 1 END;";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidScalarTypes()
    {
        var script = """
            CREATE TYPE bob FROM INT
            GO
            CREATE PROCEDURE pob @arg bob AS BEGIN SELECT @arg END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidSchemas()
    {
        var script = """
            CREATE SCHEMA bob
            GO
            CREATE PROCEDURE bob.pob AS BEGIN SELECT 1 END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidTables()
    {
        var script = """
            CREATE TABLE alice 
            (
                col1 INT, 
                col2 INT, 
                col3 INT, 
                CONSTRAINT pk_alice_col1 PRIMARY KEY CLUSTERED (col1 ASC),
                CONSTRAINT uq_alice_col2 UNIQUE (col2),
                CONSTRAINT ck_alice_col1_col2 CHECK (col1 != col2),
                INDEX idx_alice_col3 (col3)
            )
            GO

            CREATE TABLE bob 
            (
                bol1 INT CONSTRAINT fk_bob_bol1 REFERENCES alice (bol1), 
                bol2 INT CONSTRAINT df_bob_bol2 DEFAULT null, 
                bol3 INT CONSTRAINT pk_bob_bol3 PRIMARY KEY,
                bol4 INT CONSTRAINT uq_bob_bol4 UNIQUE,
                bol5 INT CONSTRAINT nn_bob_bol5 NOT NULL,
                bol6 INT CONSTRAINT ck_bob_bol6 CHECK (bol6 > 1),
                bol7 INT INDEX idx_bob_bol7,
                CONSTRAINT fk_alice_col2 FOREIGN KEY (bol5) REFERENCES alice (bol2),
            )
            GO

            CREATE PROCEDURE pob AS BEGIN SELECT b.bol1 FROM bob b END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void BareColumnNames()
    {
        var script = """
            CREATE TABLE bob (col int)
            GO
            CREATE PROCEDURE pob AS BEGIN SELECT col FROM bob END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidTableTypes()
    {
        var script = """
            CREATE TYPE alice as TABLE
            (
                col1 INT,
                col2 INT,
                col3 INT,
                PRIMARY KEY (col1 ASC),
                UNIQUE (col2),
                CHECK (col3 > 0),
                INDEX idx_alice_col3 (col3 DESC)
            )
            GO

            CREATE TYPE bob AS TABLE 
            ( 
                bol1 INT DEFAULT 0,
                bol2 INT NOT NULL,
                bol3 INT IDENTITY(1,1),
                bol4 INT PRIMARY KEY,
                bol5 INT UNIQUE NONCLUSTERED,
                bol6 INT CHECK(bol6 > 0)
            )
            GO

            CREATE PROCEDURE pob @a alice, @b bob READONLY AS BEGIN SELECT b.bol1 FROM @b b END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidTrigger()
    {
        var script = """
            CREATE TABLE bob (col int)
            GO
            CREATE TRIGGER tigger on bob after insert as select 1
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }

    [Test]
    public void ValidViews()
    {
        var script = """
            CREATE TABLE bob (col int)
            GO
            CREATE VIEW vob (vcol) AS SELECT bob.col from bob
            GO
            CREATE PROCEDURE pob AS BEGIN SELECT v.vcol FROM vob v END;
            """;
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }


    [Test]
    public void ValidUsers()
    {
        var script = "CREATE USER bob";
        var db = new DbBuilder().AddFromTsqlScript(script).Build();

        var resultSet = sut.Run(db);

        Assert.That(resultSet.Results, Is.Empty);
    }
}
