namespace Lindiwe;

public sealed class CommandWorker(
    IRepository repository,
    CommandService commandService,
    ILogger<CommandWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var commands = await repository.GetPendingAsync(stoppingToken);

            foreach (var command in commands)
            {
                try
                {
                    var completed = await commandService.ExecuteAsync(command, stoppingToken);

                    if (completed)
                    {
                        await repository.DeleteAsync(command.Id, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Command {CommandId} was not executed.", command.Id);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}