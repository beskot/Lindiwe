namespace Lindiwe;

public interface ICommandHandler
{
    string Type { get; }
    Task<bool> ExecuteAsync(string payloadJson, CancellationToken cancellationToken);
}