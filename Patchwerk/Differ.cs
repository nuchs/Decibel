using Mast;
using Mast.Dbo;
using System.Linq;

namespace Patchwerk;

public class Differ
{
    public string GeneratePatches(IDatabase before, IDatabase after)
    {
        List<string> patches = new();

        GenerateSchemaPatches(before, after, patches);
        GenerateUserPatches(before, after, patches);
        GenerateScalarTypePatches(before, after, patches);
        GenerateTableTypePatches(before, after, patches);
        GenerateTablePatches(before, after, patches);
        GenerateViewPatches(before, after, patches);
        GenerateFunctionPatches(before, after, patches);
        GenerateProcedurePatches(before, after, patches);
        GenerateTriggerPatches(before, after, patches);

        var megaPatch = string.Join("\nGO\n\n", patches);

        return megaPatch;
    }

    private void GenerateTriggerPatches(IDatabase before, IDatabase after, List<string> patches)
    {
    }

    private void GenerateProcedurePatches(IDatabase before, IDatabase after, List<string> patches)
    {
    }

    private void GenerateFunctionPatches(IDatabase before, IDatabase after, List<string> patches)
    {
    }

    private void GenerateViewPatches(IDatabase before, IDatabase after, List<string> patches)
    {
    }

    private void GenerateTablePatches(IDatabase before, IDatabase after, List<string> patches)
    {
    }

    private void GenerateTableTypePatches(IDatabase before, IDatabase after, List<string> patches)
    {
    }

    private void GenerateScalarTypePatches(IDatabase before, IDatabase after, List<string> patches)
    {
    }

    private void GenerateSchemaPatches(IDatabase before, IDatabase after, List<string> patches)
    {
        var added = after.Schemas.Except(before.Schemas);
        var removed = before.Schemas.Except(after.Schemas);

        foreach (var schema in added)
        {
            patches.Add(schema.Content);
        }

        foreach (var schema in removed)
        {
            patches.Add($"DROP SCHEMA {schema.Identifier}");
        }
    }

    private void GenerateUserPatches(IDatabase before, IDatabase after, List<string> patches)
    {
        var preIds = before.Users.Select(u => u.Identifier);
        var postIds = after.Users.Select(u => u.Identifier);

        foreach (var id in postIds.Except(preIds))
        {
            var user = after[id];
            patches.Add(user.Content);
        }

        foreach (var id in preIds.Except(postIds))
        {
            var user = before[id];
            patches.Add($"DROP USER {user.Identifier}");
        }

        foreach (var userId in preIds.Intersect(postIds))
        {
            if(before[userId] is User preUser && after[userId] is User postUser)
            {
                if (preUser != postUser)
                {
                    var schemaPart = preUser.DefaultSchema == postUser.DefaultSchema ? "" : $"DEFAULT_SCHEMA = {postUser.DefaultSchema}";
                    var loginPart = preUser.Login == postUser.Login ? "" : $"LOGIN = {postUser.Login}";
                    var languagePart = preUser.DefaultLanguage == postUser.DefaultLanguage ? "" : $"DEFAULT_LANGUAGE = {postUser.DefaultLanguage}";
                    var mods = string.Join(", ", new string[] { schemaPart, loginPart, languagePart, }.Where(s => !string.IsNullOrWhiteSpace(s)));
                    patches.Add($"ALTER USER {postUser.Identifier} WITH {mods}");
                }
            }
            else
            {
                throw new InvalidCastException($"Bad user id {userId} does not refer to user");
            }
        }
    }
}
