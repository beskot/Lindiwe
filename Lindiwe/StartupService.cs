namespace Lindiwe;

public sealed class StartupService
{
    private const string HelpText = """
                                    Lindiwe — usage:

                                      --help | -h   Show the help message
                                      --run | -r    Run the service
                                    """;

    public bool CanRun { get; private set; }

    private static (string Command, Dictionary<string, string> Args) Parse(string[] args)
    {
        var command = string.Empty;
        var commandArgs = new Dictionary<string, string>();

        for (var i = 0; i < args.Length; i++)
        {
            var current = args[i];
            if (!current.StartsWith("--"))
            {
                continue;
            }

            var name = current[2..];
            if (string.IsNullOrEmpty(command))
            {
                command = name;
                continue;
            }

            var hasValue = i + 1 < args.Length && !args[i + 1].StartsWith("--");
            commandArgs[name] = hasValue ? args[++i] : "true";
        }

        return (command, commandArgs);
    }

    public void Handle(IEnumerable<string> args)
    {
        try
        {
            var (command, commandArgs) = Parse([.. args]);

            var result = command switch
            {
                "--help" or "-h" => HelpText,
                "--run" or "-r" => Run(commandArgs),
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

    private string Run(Dictionary<string, string> args)
    {
        CanRun = true;
        return string.Join(" ", args);
    }
}