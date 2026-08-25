using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Customers.Models;

public sealed class Customer
{
    [JsonPropertyName("id")]
    public long Id { get; init; }
    [JsonPropertyName("customer_code")]
    public string? CustomerCode { get; init; }
    [JsonPropertyName("email")]
    public string? Email { get; init; }
    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }
    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }
    [JsonPropertyName("phone")]
    public string? Phone { get; init; }
    [JsonPropertyName("risk_action")]
    public string? RiskAction { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}

public sealed class InitializeAuthorizationData
{
    [JsonPropertyName("authorization_url")]
    public string? AuthorizationUrl { get; init; }
    [JsonPropertyName("access_code")]
    public required string AccessCode { get; init; }
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }
}

public sealed class AuthorizationStatus
{
    [JsonPropertyName("status")]
    public string? Status { get; init; }
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}
