using WorkflowEngine.Models;

namespace WorkflowEngine.Repositories;

public class WorkflowRepository
{
    private readonly Dictionary<string, WorkflowDefinition> _definitions = new();
    private readonly Dictionary<string, WorkflowInstance> _instances = new();

    // Add a new workflow definition
    public bool AddDefinition(WorkflowDefinition definition)
    {
        if (_definitions.ContainsKey(definition.Id)) return false;
        _definitions[definition.Id] = definition;
        return true;
    }

    // Get workflow definition by ID
    public WorkflowDefinition? GetDefinition(string id)
    {
        _definitions.TryGetValue(id, out var def);
        return def;
    }

    // Start a new workflow instance from a definition
    public WorkflowInstance? StartInstance(string definitionId)
    {
        if (!_definitions.TryGetValue(definitionId, out var definition))
            return null;

        var initialState = definition.States.FirstOrDefault(s => s.IsInitial && s.Enabled);
        if (initialState == null)
            return null;

        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid().ToString(),
            DefinitionId = definitionId,
            CurrentStateId = initialState.Id,
            History = new List<TransitionRecord>()
        };


        _instances[instance.Id] = instance;
        return instance;
    }

    // Get a workflow instance by ID
    public WorkflowInstance? GetInstance(string id)
    {
        _instances.TryGetValue(id, out var instance);
        return instance;
    }

    // Execute an action on an instance
    public string? ExecuteAction(string instanceId, string actionId)
    {
        if (!_instances.TryGetValue(instanceId, out var instance))
            return "Instance not found.";

        if (!_definitions.TryGetValue(instance.DefinitionId, out var definition))
            return "Workflow definition not found.";

        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentStateId);
        if (currentState == null || currentState.IsFinal)
            return "Cannot execute actions on a final or invalid state.";

        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
            return "Action not found.";

        if (!action.Enabled)
            return "Action is disabled.";

        if (!action.FromStates.Contains(currentState.Id))
            return $"Action '{actionId}' is not allowed from current state '{currentState.Id}'.";

        var nextState = definition.States.FirstOrDefault(s => s.Id == action.ToState && s.Enabled);
        if (nextState == null)
            return $"Target state '{action.ToState}' is invalid or disabled.";

        // ✅ Passed all checks → perform transition
        instance.CurrentStateId = nextState.Id;
        instance.History.Add(new TransitionRecord
        {
            ActionId = actionId,
            Timestamp = DateTime.UtcNow
        });


        return null; // success
    }
}
