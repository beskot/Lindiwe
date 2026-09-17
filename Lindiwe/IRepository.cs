namespace Lindiwe;

public interface IRepository
{
    Task AddAsync(Command command, CancellationToken cancellationToken);

    Task<IReadOnlyList<Command>> GetPendingAsync(CancellationToken cancellationToken);

    Task DeleteAsync(string id, CancellationToken cancellationToken);
}

public class MemoryRepository : IRepository
{
    public Task AddAsync(Command command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Command>> GetPendingAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}