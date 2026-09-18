namespace Lindiwe;

public sealed class EchoCommandHandler(ILogger<EchoCommandHandler> logger) : ICommandHandler
{
    public string Type => "echo";

    public Task<bool> ExecuteAsync(Command command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Echo: {PayloadJson}", command.PayloadJson);
        return Task.FromResult(true);
    }
}