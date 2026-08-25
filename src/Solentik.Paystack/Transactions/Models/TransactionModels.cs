using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Transactions.Models;

public sealed class InitializeTransactionData
{
    [JsonPropertyName("authorization_url")]
    public required string AuthorizationUrl { get; init; }

    [JsonPropertyName("access_code")]
    public required string AccessCode { get; init; }

    [JsonPropertyName("reference")]
    public required string Reference { get; init; }
}

/// <summary>Represents common fields returned for a Paystack transaction.</summary>
public sealed class Transaction
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    [JsonPropertyName("amount")]
    public long Amount { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("paid_at")]
    public DateTimeOffset? PaidAt { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}

public sealed class TransactionListOptions
{
    public int? PerPage { get; init; }
    public int? Page { get; init; }
    public string? Customer { get; init; }
    public string? Status { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}
