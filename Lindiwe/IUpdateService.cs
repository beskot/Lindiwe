namespace Lindiwe;

public interface IUpdateService
{
    Task<bool> CheckForUpdateAsync(CancellationToken cancellationToken = default);
    Task DownloadAsync(CancellationToken cancellationToken = default);
    void RestartApplication();
}