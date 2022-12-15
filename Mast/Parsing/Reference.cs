using Mast.Dbo;

namespace Mast.Parsing;

public sealed record Reference(DbObject Referee, FullyQualifiedName Referent);
