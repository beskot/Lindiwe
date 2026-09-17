namespace Lindiwe;

public interface IRepository
{
    Task AddAsync(Command command, CancellationToken cancellationToken);

    Task<IReadOnlyList<Command>> GetPendingAsync(CancellationToken cancellationToken);

    Task DeleteAsync(string id, CancellationToken cancellationToken);
}

public sealed class MemoryRepository : IRepository
{
    private readonly Dictionary<string, Command> _commands = [];
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task AddAsync(Command command, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            _commands.TryAdd(command.Id, command);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IReadOnlyList<Command>> GetPendingAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            return _commands.Values.ToArray();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            _commands.Remove(id);
        }
        finally
        {
            _lock.Release();
        }
    }
}