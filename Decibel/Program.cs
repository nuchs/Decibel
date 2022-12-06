// See https://aka.ms/new-console-template for more information
using Log;
using Mast;
using Mast.Dbo;

var log = LoggerFactory.CreateLogger<Program>();

log.Info("Hello");

var table = File.ReadAllText(@"Tables\dbo.Customer.sql");
var type = File.ReadAllText(@"Types\dbo.InventoryItem.sql");
var func = File.ReadAllText(@"Functions\dbo.IsTurkeyFast.sql");
var proc1 = File.ReadAllText(@"StoredProcedures\dbo.GetCustomers.sql");
var proc2 = File.ReadAllText(@"StoredProcedures\dbo.GetFastTurkeys.sql");
var db = new Database();
var factory = new ScriptParser();
factory.Parse(db, func);
factory.Parse(db, proc1);
factory.Parse(db, proc2);
factory.Parse(db, table);
factory.Parse(db, type);

log.Info("Goodbye");
