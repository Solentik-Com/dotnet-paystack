using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Plans.Models;

public sealed class CreatePlanRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("amount")]
    public required long Amount { get; init; }
    [JsonPropertyName("interval")]
    public required string Interval { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
}

public sealed class UpdatePlanRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("amount")]
    public long? Amount { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("update_existing_subscriptions")]
    public bool? UpdateExistingSubscriptions { get; init; }
}

public sealed class PlanListOptions
{
    public int? PerPage { get; init; }
    public int? Page { get; init; }
    public string? Interval { get; init; }
    public long? Amount { get; init; }
}

public sealed class Plan
{
    [JsonPropertyName("id")]
    public long Id { get; init; }
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("plan_code")]
    public string? PlanCode { get; init; }
    [JsonPropertyName("amount")]
    public long Amount { get; init; }
    [JsonPropertyName("interval")]
    public string? Interval { get; init; }
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}
