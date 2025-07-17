namespace WorkflowEngine.Models;

public class WorkflowInstance
{
    public string Id { get; set; } = default!;
    public string DefinitionId { get; set; } = default!;
    public string CurrentStateId { get; set; } = default!;
    public List<TransitionRecord> History { get; set; } = new();

}
