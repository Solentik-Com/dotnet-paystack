using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.TransactionSplits.Models;

public sealed class CreateTransactionSplitRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("type")]
    public required string Type { get; init; }
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }
    [JsonPropertyName("subaccounts")]
    public required IReadOnlyList<SplitSubaccountRequest> Subaccounts { get; init; }
    [JsonPropertyName("bearer_type")]
    public string? BearerType { get; init; }
    [JsonPropertyName("bearer_subaccount")]
    public string? BearerSubaccount { get; init; }
}

public sealed class UpdateTransactionSplitRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("active")]
    public bool? Active { get; init; }
    [JsonPropertyName("bearer_type")]
    public string? BearerType { get; init; }
    [JsonPropertyName("bearer_subaccount")]
    public string? BearerSubaccount { get; init; }
}

public sealed class SplitSubaccountRequest
{
    [JsonPropertyName("subaccount")]
    public required string Subaccount { get; init; }
    [JsonPropertyName("share")]
    public required decimal Share { get; init; }
}

public sealed class TransactionSplitListOptions
{
    public string? Name { get; init; }
    public bool? Active { get; init; }
    public string? SortBy { get; init; }
    public int? PerPage { get; init; }
    public int? Page { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}

public sealed class TransactionSplit
{
    [JsonPropertyName("id")]
    public long Id { get; init; }
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("split_code")]
    public string? SplitCode { get; init; }
    [JsonPropertyName("type")]
    public string? Type { get; init; }
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
    [JsonPropertyName("active")]
    public bool Active { get; init; }
    [JsonPropertyName("bearer_type")]
    public string? BearerType { get; init; }
    [JsonPropertyName("bearer_subaccount")]
    public string? BearerSubaccount { get; init; }
    [JsonPropertyName("subaccounts")]
    public JsonElement? Subaccounts { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}
