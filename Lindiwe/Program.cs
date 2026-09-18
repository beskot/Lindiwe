using Lindiwe;
using Microsoft.AspNetCore.Mvc;

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

app.MapMethods(
    "/commands/{commandType}",
    [HttpMethods.Get, HttpMethods.Post],
    async (
        [FromRoute] string commandType,
        HttpRequest httpRequest,
        IRepository repository,
        CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(commandType))
        {
            return Results.BadRequest(new
            {
                error = "Command type is required."
            });
        }

        var payloadJson = httpRequest.Method switch
        {
            "POST" => await httpRequest.ToPayloadJsonString(cancellationToken),
            _ => string.Empty
        };

        var command = new Command(
            Id: Guid.NewGuid().ToString("N"),
            Type: commandType.Trim().ToLowerInvariant(),
            QueryParameters: httpRequest.ToQueryParameters(),
            PayloadJson: payloadJson,
            CreatedAt: DateTimeOffset.UtcNow);

        await repository.AddAsync(command, cancellationToken);

        return Results.Accepted($"/commands/{command.Id}", command);
    });

app.Run();