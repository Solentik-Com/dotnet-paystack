using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Models;

/// <summary>Represents the standard Paystack API response envelope.</summary>
public sealed class PaystackResponse<T>
{
    [JsonPropertyName("status")]
    public bool Status { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("meta")]
    public JsonElement? Meta { get; init; }
}
