using System.Text.Json.Serialization;

namespace Lindiwe;

public sealed record CreateCommandRequest(string Type, string PayloadJson);
public sealed record Command(string Id, string Type, IReadOnlyDictionary<string, string?[]> QueryParameters, string PayloadJson, DateTimeOffset CreatedAt);
public sealed record ErrorResponse(string Error);

[JsonSerializable(typeof(CreateCommandRequest))]
[JsonSerializable(typeof(Command))]
[JsonSerializable(typeof(ErrorResponse))]
internal partial class LindiweJsonSerializerContext : JsonSerializerContext;