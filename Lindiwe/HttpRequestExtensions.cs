using System.Text;

namespace Lindiwe;

public static class HttpRequestExtensions
{
    extension(HttpRequest httpRequest)
    {
        public async Task<string> ToPayloadJsonString(CancellationToken cancellationToken = default)
        {
            using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false,
                bufferSize: 4096, leaveOpen: false);
            return await reader.ReadToEndAsync(cancellationToken);
        }

        public Dictionary<string, string?[]> ToQueryParameters()
        {
            return httpRequest.Query.ToDictionary(
                item => item.Key,
                item => item.Value.ToArray(),
                StringComparer.OrdinalIgnoreCase);
        }
    }
}