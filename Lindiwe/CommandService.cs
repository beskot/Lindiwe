namespace Lindiwe;

public sealed class CommandService(IEnumerable<ICommandHandler> handlers)
{
    public async Task<bool> ExecuteAsync(Command command, CancellationToken cancellationToken)
    {
        var handler = handlers.FirstOrDefault(x => string.Equals(x.Type, command.Type, StringComparison.OrdinalIgnoreCase));

        if (handler is null)
        {
            return false;
        }

        return await handler.ExecuteAsync(command.PayloadJson, cancellationToken);
    }
}