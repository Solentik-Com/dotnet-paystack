using System.Text.Json.Serialization;

namespace Solentik.Paystack.Customers.Models;

public sealed class CreateCustomerRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }
    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }
    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }
    [JsonPropertyName("phone")]
    public string? Phone { get; init; }
    [JsonPropertyName("metadata")]
    public object? Metadata { get; init; }
}

public sealed class UpdateCustomerRequest
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }
    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }
    [JsonPropertyName("phone")]
    public string? Phone { get; init; }
    [JsonPropertyName("metadata")]
    public object? Metadata { get; init; }
}

public sealed class ValidateCustomerIdentityRequest
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }
    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }
    [JsonPropertyName("type")]
    public required string Type { get; init; }
    [JsonPropertyName("value")]
    public string? Value { get; init; }
    [JsonPropertyName("country")]
    public required string Country { get; init; }
    [JsonPropertyName("bvn")]
    public string? Bvn { get; init; }
    [JsonPropertyName("bank_code")]
    public string? BankCode { get; init; }
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; init; }
}

public sealed class SetCustomerRiskActionRequest
{
    [JsonPropertyName("customer")]
    public required string Customer { get; init; }
    [JsonPropertyName("risk_action")]
    public required string RiskAction { get; init; }
}

public sealed class InitializeAuthorizationRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }
    [JsonPropertyName("channel")]
    public required string Channel { get; init; }
    [JsonPropertyName("callback_url")]
    public string? CallbackUrl { get; init; }
    [JsonPropertyName("account")]
    public object? Account { get; init; }
    [JsonPropertyName("address")]
    public object? Address { get; init; }
}

public sealed class CustomerListOptions
{
    public int? PerPage { get; init; }
    public int? Page { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}
