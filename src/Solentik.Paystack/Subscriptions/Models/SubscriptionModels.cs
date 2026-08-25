using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Subscriptions.Models;

public sealed class CreateSubscriptionRequest
{
    [JsonPropertyName("customer")]
    public required string Customer { get; init; }
    [JsonPropertyName("plan")]
    public required string Plan { get; init; }
    [JsonPropertyName("authorization")]
    public string? Authorization { get; init; }
    [JsonPropertyName("start_date")]
    public DateTimeOffset? StartDate { get; init; }
}

public sealed class SubscriptionListOptions
{
    public int? PerPage { get; init; }
    public int? Page { get; init; }
    public string? Customer { get; init; }
    public string? Plan { get; init; }
}

public sealed class Subscription
{
    [JsonPropertyName("id")]
    public long Id { get; init; }
    [JsonPropertyName("subscription_code")]
    public string? SubscriptionCode { get; init; }
    [JsonPropertyName("email_token")]
    public string? EmailToken { get; init; }
    [JsonPropertyName("status")]
    public string? Status { get; init; }
    [JsonPropertyName("amount")]
    public long Amount { get; init; }
    [JsonPropertyName("customer")]
    public JsonElement? Customer { get; init; }
    [JsonPropertyName("plan")]
    public JsonElement? Plan { get; init; }
    [JsonPropertyName("start_date")]
    public DateTimeOffset? StartDate { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}

public sealed class SubscriptionManagementLink
{
    [JsonPropertyName("link")]
    public required string Link { get; init; }
}
