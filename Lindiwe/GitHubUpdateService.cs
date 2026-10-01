using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lindiwe;

public sealed class GitHubUpdateService(string currentVersion, HttpClient httpClient) : IUpdateService
{
    private const string Owner = "beskot";
    private const string Repository = "Lindiwe";
    private string? _newVersion;

    private readonly string _targetPath = Environment.ProcessPath ??
                                          throw new InvalidOperationException("The target path is invalid.");

    public async Task<bool> CheckForUpdateAsync(CancellationToken cancellationToken = default)
    {
        if (!SemanticVersion.TryParse(currentVersion, out var current))
        {
            return false;
        }

        var releases = await GetReleasesAsync(cancellationToken);

        var latest = releases
            .Where(release => !release.Draft)
            .Where(release => release.Prerelease)
            .Select(release => new
            {
                Release = release,
                Version = SemanticVersion.TryParse(release.TagName, out var ver) ? ver : SemanticVersion.Default()
            })
            .OrderByDescending(item => item.Version)
            .FirstOrDefault();

        if (latest is null || latest.Version.Equals(current))
        {
            return false;
        }

        _newVersion = latest.Version.ToString();
        return true;
    }

    public async Task DownloadAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_newVersion))
        {
            throw new Exception("No new version available");
        }

        var url = $"https://github.com/{Owner}/{Repository}/releases/download/v{_newVersion}/Lindiwe-linux-arm64";
        var response = await httpClient.GetAsync(
            url,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var binData = await response.Content.ReadAsStreamAsync(cancellationToken);
        var binName = Path.GetFileName(_targetPath);
        var binDir = Path.GetDirectoryName(_targetPath) ?? throw new InvalidOperationException("The target path is invalid.");
        var temporaryPath = Path.Combine(binDir, "partial_" + binName);

        await using var fs = File.Create(temporaryPath);
        await binData.CopyToAsync(fs, cancellationToken);

        if (OperatingSystem.IsLinux())
        {
            if (OperatingSystem.IsLinux())
            {
                File.SetUnixFileMode(
                    temporaryPath,
                    UnixFileMode.UserRead |
                    UnixFileMode.UserWrite |
                    UnixFileMode.UserExecute |
                    UnixFileMode.GroupRead |
                    UnixFileMode.GroupExecute |
                    UnixFileMode.OtherRead |
                    UnixFileMode.OtherExecute);
            }
            File.Move(temporaryPath, _targetPath, overwrite: true);
            return;
        }

        if (OperatingSystem.IsWindows())
        {
            var oldPath = Path.Combine(binDir,  "old_" + binName);

            if (File.Exists(oldPath))
            {
                File.Delete(oldPath);
            }

            File.Move(_targetPath, oldPath);

            if (File.Exists(_targetPath))
            {
                throw new InvalidOperationException();
            }

            File.Move(temporaryPath, _targetPath);

            return;
        }

        throw new PlatformNotSupportedException();
    }

    public void RestartApplication()
    {
        var workingDirectory = Path.GetDirectoryName(_targetPath) ??
                               throw new InvalidOperationException("No target path provided.");
        var startInfo = new ProcessStartInfo
        {
            FileName = _targetPath,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false
        };

        Process.Start(startInfo);
        Environment.Exit(0);
    }

    private async Task<IReadOnlyList<GitHubRelease>> GetReleasesAsync(CancellationToken cancellationToken)
    {
        var url = new Uri($"https://api.github.com/repos/{Owner}/{Repository}/releases?per_page=100");
        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.UserAgent.ParseAdd("Lindiwe-Updater/1.0");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync(stream,
            GitHubJsonSerializerContext.Default.IReadOnlyListGitHubRelease, cancellationToken) ?? [];
    }
}

public sealed record GitHubRelease(
    [property: JsonPropertyName("tag_name")] string TagName,
    [property: JsonPropertyName("draft")] bool Draft,
    [property: JsonPropertyName("prerelease")] bool Prerelease,
    [property: JsonPropertyName("name")] string Name);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(IReadOnlyList<GitHubRelease>))]
public sealed partial class GitHubJsonSerializerContext : JsonSerializerContext;