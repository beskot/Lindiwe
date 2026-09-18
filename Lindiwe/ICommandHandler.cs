namespace Lindiwe;

public interface ICommandHandler
{
    string Type { get; }
    Task<bool> ExecuteAsync(Command command, CancellationToken cancellationToken);
}