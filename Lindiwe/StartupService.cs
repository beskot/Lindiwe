using System.Reflection;

namespace Lindiwe;

public record CommandOption(string Key, string Value);

public sealed class StartupService
{
    private const string HelpText = """
                                    Usage:
                                      lindiwe --run [-p <port>] [-i <ip>]
                                      lindiwe --help
                                      lindiwe --version
                                      lindiwe --update

                                    Options:
                                      -h, --help        Show the help message
                                      -r, --run         Run the service
                                      -p <port>         Port to listen on
                                      -i <ip>           IP address to bind
                                      -v, --version     Show version
                                    """;

    public bool CanRun { get; private set; }
    public string Url { get; private set; }

    private static string CreateCommand(string arg)
    {
        var pos = 0;
        while (pos < arg.Length && arg[pos] == '-')
        {
            pos++;
        }

        return arg[pos..];
    }

    private static CommandOption CreateCommandOption(string arg)
    {
        var p1 = 0;
        var p2 = arg.Length;

        for (var i = 0; i < arg.Length; i++)
        {
            if (arg[i] == '-')
            {
                p1 = i + 1;
            }

            if (arg[i] == '=')
            {
                p2 = i + 1;
                break;
            }
        }

        return new CommandOption(arg[p1..(p2-1)], arg[p2..]);
    }
    
    public async Task HandleAsync(string[] args)
    {
        try
        {
            var command = CreateCommand(args[0]);

            var result = command switch
            {
                "help" or "h" => HelpText,
                "run" or "r" => Run(args[1..]),
                "version" or "v" => GetVersion(),
                "update" => await Update(),
                _ => HelpText
            };

            Console.WriteLine(result);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Unexpected error: {exception.Message}");
            Environment.ExitCode = 1;
        }
    }

    private string Run(string[] options)
    {
        var dict = options.Select(CreateCommandOption).ToDictionary(p => p.Key, p => p.Value);
        Url = $"http://{dict["i"]}:{dict["p"]}";
        CanRun = true;
        return Url;
    }

    private static string GetVersion()
    {
        var asm = Assembly.GetExecutingAssembly();
        var infoVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (!string.IsNullOrWhiteSpace(infoVersion))
        {
            return infoVersion;
        }

        var fileVersion = asm.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        if (!string.IsNullOrWhiteSpace(fileVersion))
        {
            return fileVersion;
        }

        return asm.GetName().Version?.ToString() ?? "unknown";
    }

    private static async Task<string> Update()
    {
        var updateService = new GitHubUpdateService(GetVersion(), new HttpClient());
        if (!await updateService.CheckForUpdateAsync())
        {
            return "No update available";
        }

        await updateService.DownloadAsync();
        updateService.RestartApplication();

        return "Updated version check";
    }
}