using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Verification.Models;

public sealed class ValidateAccountRequest
{
    [JsonPropertyName("account_name")]
    public required string AccountName { get; init; }
    [JsonPropertyName("account_number")]
    public required string AccountNumber { get; init; }
    [JsonPropertyName("account_type")]
    public required string AccountType { get; init; }
    [JsonPropertyName("bank_code")]
    public required string BankCode { get; init; }
    [JsonPropertyName("country_code")]
    public required string CountryCode { get; init; }
    [JsonPropertyName("document_type")]
    public required string DocumentType { get; init; }
    [JsonPropertyName("document_number")]
    public string? DocumentNumber { get; init; }
}

public sealed class ResolvedAccount
{
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; init; }
    [JsonPropertyName("account_name")]
    public string? AccountName { get; init; }
    [JsonPropertyName("bank_id")]
    public long? BankId { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}

public sealed class CardBinInfo
{
    [JsonPropertyName("bin")]
    public string? Bin { get; init; }
    [JsonPropertyName("brand")]
    public string? Brand { get; init; }
    [JsonPropertyName("sub_brand")]
    public string? SubBrand { get; init; }
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; init; }
    [JsonPropertyName("country_name")]
    public string? CountryName { get; init; }
    [JsonPropertyName("card_type")]
    public string? CardType { get; init; }
    [JsonPropertyName("bank")]
    public string? Bank { get; init; }
    [JsonPropertyName("linked_bank_id")]
    public long? LinkedBankId { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}
