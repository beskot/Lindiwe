namespace Lindiwe;

public sealed class CommandService
{
    private readonly IReadOnlyDictionary<string, ICommandHandler> _handlers;

    public CommandService(IEnumerable<ICommandHandler> handlers)
    {
        var map = new Dictionary<string, ICommandHandler>(StringComparer.OrdinalIgnoreCase);

        foreach (var handler in handlers)
        {
            if (string.IsNullOrWhiteSpace(handler.Type))
            {
                throw new InvalidOperationException($"Handler '{handler.GetType().Name}' has an empty Type.");
            }

            if (!map.TryAdd(handler.Type, handler))
            {
                throw new InvalidOperationException($"More than one handler is registered for command type '{handler.Type}'.");
            }
        }

        _handlers = map;
    }

    public CommandService(IReadOnlyDictionary<string, ICommandHandler> handlers)
    {
        _handlers = handlers;
    }

    public bool Supports(string commandType)
    {
        return !string.IsNullOrWhiteSpace(commandType) && _handlers.ContainsKey(commandType);
    }

    public async Task<bool> ExecuteAsync(Command command, CancellationToken cancellationToken)
    {
        if (!_handlers.TryGetValue(command.Type, out var handler))
        {
            return false;
        }

        return await handler.ExecuteAsync(command.PayloadJson, cancellationToken);
    }
}