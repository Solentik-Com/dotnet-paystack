using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Transfers.Models;

/// <summary>The available balance for one currency on the integration.</summary>
public sealed class BalanceEntry
{
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>The balance, in the currency's smallest unit.</summary>
    [JsonPropertyName("balance")]
    public long Balance { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}

public sealed class BalanceLedgerListOptions
{
    public int? PerPage { get; init; }
    public int? Page { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}

/// <summary>One pay-in or pay-out entry from <c>FetchLedgerAsync</c>.</summary>
public sealed class BalanceLedgerEntry
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("domain")]
    public string? Domain { get; init; }

    /// <summary>The account balance immediately after this entry, in the currency's smallest unit.</summary>
    [JsonPropertyName("balance")]
    public long Balance { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>The amount this entry changed the balance by (negative for a pay-out), in the currency's smallest unit.</summary>
    [JsonPropertyName("difference")]
    public long Difference { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>What caused this entry, e.g. <c>"Transfer"</c>, <c>"Transaction"</c>, <c>"Refund"</c>.</summary>
    [JsonPropertyName("model_responsible")]
    public string? ModelResponsible { get; init; }

    /// <summary>The ID of the <see cref="ModelResponsible"/> record, e.g. a transfer or transaction ID.</summary>
    [JsonPropertyName("model_row")]
    public long? ModelRow { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTimeOffset? CreatedAt { get; init; }

    [JsonPropertyName("updatedAt")]
    public DateTimeOffset? UpdatedAt { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}
