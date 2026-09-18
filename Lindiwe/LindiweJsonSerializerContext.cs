using System.Text.Json.Serialization;

namespace Lindiwe;

public sealed record CreateCommandRequest(string Type, string PayloadJson);
public sealed record Command(string Id, string Type, IReadOnlyDictionary<string, string?[]> QueryParameters, string PayloadJson, DateTimeOffset CreatedAt);

[JsonSerializable(typeof(CreateCommandRequest))]
[JsonSerializable(typeof(Command))]
internal partial class LindiweJsonSerializerContext : JsonSerializerContext;