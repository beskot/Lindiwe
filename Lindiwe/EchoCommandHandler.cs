namespace Lindiwe;

public sealed class EchoCommandHandler(ILogger<EchoCommandHandler> logger) : ICommandHandler
{
    public string Type => "echo";

    public Task<bool> ExecuteAsync(string payloadJson, CancellationToken cancellationToken)
    {
        logger.LogInformation("Echo: {PayloadJson}", payloadJson);
        return Task.FromResult(true);
    }
}