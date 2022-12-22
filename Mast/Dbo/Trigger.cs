using Mast.Parsing;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class Trigger : DbObject
{
    public Trigger(CreateTriggerStatement trigger)
        : base(trigger)
    {
        Identifier = AssembleIdentifier(trigger);
        When = GetRunOption(trigger);
        TriggerActions = CollectTriggerActions(trigger);
        Target = GetTarget(trigger);
    }

    public string Target { get; }

    public IEnumerable<TriggeredBy> TriggerActions { get; }

    public RunWhen When { get; }

    private FullyQualifiedName AssembleIdentifier(CreateTriggerStatement node)
        => FullyQualifiedName.FromSchemaName(GetId(node.Name.SchemaIdentifier), GetId(node.Name.BaseIdentifier));

    private IEnumerable<TriggeredBy> CollectTriggerActions(CreateTriggerStatement trigger)
            => trigger.TriggerActions.Select(ta => MapTriggerAction(ta.TriggerActionType));

    private RunWhen GetRunOption(CreateTriggerStatement trigger)
        => trigger.TriggerType switch
        {
            TriggerType.For => RunWhen.After,
            TriggerType.After => RunWhen.After,
            TriggerType.InsteadOf => RunWhen.Instead,
            _ => throw new InvalidDataException($"Unrecognised trigger type {trigger.TriggerType}")
        };

    private string GetTarget(CreateTriggerStatement trigger)
        => $"{GetId(trigger.TriggerObject.Name.SchemaIdentifier)}.{GetId(trigger.TriggerObject.Name.BaseIdentifier)}";

    private TriggeredBy MapTriggerAction(TriggerActionType action)
        => action switch
        {
            TriggerActionType.Insert => TriggeredBy.Insert,
            TriggerActionType.Update => TriggeredBy.Update,
            TriggerActionType.Delete => TriggeredBy.Delete,
            _ => throw new NotSupportedException($"{action} is not a supported trigger type")
        };
}
