using WorkflowEngine.Models;
using WorkflowEngine.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services for minimal APIs and JSON serialization
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<WorkflowRepository>();

var app = builder.Build();

// Enable Swagger (optional, for UI testing)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable routing and HTTP request pipeline
app.UseHttpsRedirection();

// ✅ POST: Create a workflow definition
app.MapPost("/workflow-definitions", (WorkflowDefinition def, WorkflowRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(def.Id))
        return Results.BadRequest("WorkflowDefinition must have a non-empty Id.");

    if (!def.States.Any(s => s.IsInitial))
        return Results.BadRequest("WorkflowDefinition must contain exactly one initial state.");

    if (def.States.Count(s => s.IsInitial) != 1)
        return Results.BadRequest("There must be exactly one initial state.");

    if (!repo.AddDefinition(def))
        return Results.Conflict("WorkflowDefinition with this ID already exists.");

    return Results.Created($"/workflow-definitions/{def.Id}", def);
});

// ✅ GET: Get a workflow definition by ID
app.MapGet("/workflow-definitions/{id}", (string id, WorkflowRepository repo) =>
{
    var definition = repo.GetDefinition(id);
    if (definition is null)
        return Results.NotFound($"WorkflowDefinition with ID '{id}' not found.");

    return Results.Ok(definition);
});

// ✅ POST: Start a workflow instance
app.MapPost("/workflow-instances", (string definitionId, WorkflowRepository repo) =>
{
    var instance = repo.StartInstance(definitionId);
    if (instance == null)
        return Results.NotFound($"WorkflowDefinition '{definitionId}' not found or missing valid initial state.");

    return Results.Created($"/workflow-instances/{instance.Id}", instance);
});

// ✅ GET: Retrieve a workflow instance
app.MapGet("/workflow-instances/{id}", (string id, WorkflowRepository repo) =>
{
    var instance = repo.GetInstance(id);
    if (instance is null)
        return Results.NotFound($"WorkflowInstance with ID '{id}' not found.");

    return Results.Ok(instance);
});

// ✅ POST: Execute an action on a workflow instance
app.MapPost("/workflow-instances/{id}/actions", (string id, string actionId, WorkflowRepository repo) =>
{
    var error = repo.ExecuteAction(id, actionId);
    if (error != null)
        return Results.BadRequest(error);

    var updated = repo.GetInstance(id);
    return Results.Ok(updated);
});

app.Run();
