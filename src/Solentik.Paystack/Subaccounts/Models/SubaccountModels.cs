using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Subaccounts.Models;

public sealed class CreateSubaccountRequest
{
    [JsonPropertyName("business_name")]
    public required string BusinessName { get; init; }
    [JsonPropertyName("settlement_bank")]
    public required string SettlementBank { get; init; }
    [JsonPropertyName("account_number")]
    public required string AccountNumber { get; init; }
    [JsonPropertyName("percentage_charge")]
    public required decimal PercentageCharge { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("primary_contact_email")]
    public string? PrimaryContactEmail { get; init; }
    [JsonPropertyName("primary_contact_name")]
    public string? PrimaryContactName { get; init; }
    [JsonPropertyName("primary_contact_phone")]
    public string? PrimaryContactPhone { get; init; }
    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; init; }
}

public sealed class UpdateSubaccountRequest
{
    [JsonPropertyName("business_name")]
    public string? BusinessName { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("settlement_bank")]
    public string? SettlementBank { get; init; }
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; init; }
    [JsonPropertyName("active")]
    public bool? Active { get; init; }
    [JsonPropertyName("percentage_charge")]
    public decimal? PercentageCharge { get; init; }
    [JsonPropertyName("settlement_schedule")]
    public string? SettlementSchedule { get; init; }
    [JsonPropertyName("primary_contact_email")]
    public string? PrimaryContactEmail { get; init; }
    [JsonPropertyName("primary_contact_name")]
    public string? PrimaryContactName { get; init; }
    [JsonPropertyName("primary_contact_phone")]
    public string? PrimaryContactPhone { get; init; }
    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; init; }
}

public sealed class SubaccountListOptions
{
    public int? PerPage { get; init; }
    public int? Page { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}

public sealed class Subaccount
{
    [JsonPropertyName("id")]
    public long Id { get; init; }
    [JsonPropertyName("subaccount_code")]
    public string? SubaccountCode { get; init; }
    [JsonPropertyName("business_name")]
    public string? BusinessName { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("settlement_bank")]
    public string? SettlementBank { get; init; }
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; init; }
    [JsonPropertyName("percentage_charge")]
    public decimal PercentageCharge { get; init; }
    [JsonPropertyName("settlement_schedule")]
    public string? SettlementSchedule { get; init; }
    [JsonPropertyName("active")]
    public bool Active { get; init; }
    [JsonPropertyName("primary_contact_email")]
    public string? PrimaryContactEmail { get; init; }
    [JsonPropertyName("primary_contact_name")]
    public string? PrimaryContactName { get; init; }
    [JsonPropertyName("primary_contact_phone")]
    public string? PrimaryContactPhone { get; init; }
    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}
