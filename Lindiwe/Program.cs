using Lindiwe;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, LindiweJsonSerializerContext.Default);
});

builder.Services.AddSingleton<IRepository, MemoryRepository>();
builder.Services.AddSingleton<ICommandHandler, EchoCommandHandler>();
builder.Services.AddSingleton<CommandService>();
builder.Services.AddHostedService<CommandWorker>();

var app = builder.Build();

app.MapPost("/commands", async (
    CreateCommandRequest request,
    IRepository repository,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Type))
    {
        return Results.BadRequest(new
        {
            error = "Type is required."
        });
    }

    if (string.IsNullOrWhiteSpace(request.PayloadJson))
    {
        return Results.BadRequest(new
        {
            error = "PayloadJson is required."
        });
    }

    var command = new Command(
        Id: Guid.NewGuid().ToString("N"),
        Type: request.Type,
        PayloadJson: request.PayloadJson,
        CreatedAt: DateTimeOffset.UtcNow);

    await repository.AddAsync(command, cancellationToken);

    return Results.Accepted($"/commands/{command.Id}", command);
});

app.Run();