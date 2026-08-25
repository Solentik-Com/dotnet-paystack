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
}
