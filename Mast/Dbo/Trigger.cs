using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Mast.Dbo;

public sealed class Trigger : DbObject
{
    public Trigger(CreateTriggerStatement trigger)
        : base(trigger)
    {
        Name = GetName(trigger);
        Schema = GetSchema(trigger);
        When = GetRunOption(trigger);
        TriggerActions = CollectTriggerActions(trigger);
        Target = GetTarget(trigger);
    }

    public string Schema { get; }

    public string Target { get; }

    public IEnumerable<TriggeredBy> TriggerActions { get; }

    public RunWhen When { get; }

    private IEnumerable<TriggeredBy> CollectTriggerActions(CreateTriggerStatement trigger)
        => trigger.TriggerActions.Select(ta => MapTriggerAction(ta.TriggerActionType));

    private TriggeredBy MapTriggerAction(TriggerActionType action)
        => action switch
        {
            TriggerActionType.Insert => TriggeredBy.Insert,
            TriggerActionType.Update => TriggeredBy.Update,
            TriggerActionType.Delete => TriggeredBy.Delete,
            _ => throw new NotSupportedException($"{action} is not a supported trigger type")
        };

    private string GetName(CreateTriggerStatement trigger)
        => GetId(trigger.Name.BaseIdentifier);

    private RunWhen GetRunOption(CreateTriggerStatement trigger)
        => trigger.TriggerType switch
        {
            TriggerType.For => RunWhen.After,
            TriggerType.After => RunWhen.After,
            TriggerType.InsteadOf => RunWhen.Instead,
            _ => throw new InvalidDataException($"Unrecognised trigger type {trigger.TriggerType}")
        };

    private string GetSchema(CreateTriggerStatement trigger)
        => GetId(trigger.Name.SchemaIdentifier);

    private string GetTarget(CreateTriggerStatement trigger)
        => $"{GetId(trigger.TriggerObject.Name.SchemaIdentifier)}.{GetId(trigger.TriggerObject.Name.BaseIdentifier)}";
}
