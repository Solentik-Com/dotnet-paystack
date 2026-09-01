using System.Text.Json.Serialization;

namespace Solentik.Paystack.Transactions.Models;

/// <summary>Contains the values used to initialize a Paystack transaction.</summary>
public sealed class InitializeTransactionRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("amount")]
    public required long Amount { get; init; }

    [JsonPropertyName("callback_url")]
    public string? CallbackUrl { get; init; }

    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    [JsonPropertyName("plan")]
    public string? Plan { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("channels")]
    public IReadOnlyList<string>? Channels { get; init; }

    [JsonPropertyName("metadata")]
    public object? Metadata { get; init; }

    /// <summary>The code of a subaccount that should receive a portion of this transaction, as a flat amount or percentage.</summary>
    [JsonPropertyName("subaccount")]
    public string? Subaccount { get; init; }

    /// <summary>
    /// A flat fee (in the smallest currency unit) to charge in this transaction, overriding the subaccount's
    /// percentage charge. Only takes effect when <see cref="Subaccount"/> is set.
    /// </summary>
    [JsonPropertyName("transaction_charge")]
    public long? TransactionCharge { get; init; }

    /// <summary>
    /// Who bears Paystack charges for this transaction: <c>"account"</c> or <c>"subaccount"</c>. Only takes
    /// effect when <see cref="Subaccount"/> is set. Validated client-side against these two values.
    /// </summary>
    [JsonPropertyName("bearer")]
    public string? Bearer { get; init; }

    /// <summary>
    /// The split code of a dynamic transaction split to apply to this transaction, as an alternative to a
    /// single <see cref="Subaccount"/>. Create one with <c>paystack.TransactionSplits.CreateAsync</c>.
    /// </summary>
    [JsonPropertyName("split_code")]
    public string? SplitCode { get; init; }
}
