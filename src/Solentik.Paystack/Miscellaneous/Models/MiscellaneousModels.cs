using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Miscellaneous.Models;

public sealed class BankListOptions
{
    public string? Country { get; init; }
    public bool? UseCursor { get; init; }
    public int? PerPage { get; init; }
    public bool? PayWithBankTransfer { get; init; }
    public bool? PayWithBank { get; init; }
    public bool? EnabledForVerification { get; init; }
    public string? Next { get; init; }
    public string? Previous { get; init; }
    public string? Gateway { get; init; }
    public string? Type { get; init; }
    public string? Currency { get; init; }
    public bool? IncludeNipSortCode { get; init; }
}

public sealed class Bank
{
    [JsonPropertyName("id")]
    public long Id { get; init; }
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("slug")]
    public string? Slug { get; init; }
    [JsonPropertyName("code")]
    public string? Code { get; init; }
    [JsonPropertyName("longcode")]
    public string? Longcode { get; init; }
    [JsonPropertyName("gateway")]
    public string? Gateway { get; init; }
    [JsonPropertyName("type")]
    public string? Type { get; init; }
    [JsonPropertyName("country")]
    public string? Country { get; init; }
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
    [JsonPropertyName("pay_with_bank")]
    public bool? PayWithBank { get; init; }
    [JsonPropertyName("active")]
    public bool? Active { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}

public sealed class Country
{
    [JsonPropertyName("id")]
    public long Id { get; init; }
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("iso_code")]
    public string? IsoCode { get; init; }
    [JsonPropertyName("default_currency_code")]
    public string? DefaultCurrencyCode { get; init; }
    [JsonPropertyName("calling_code")]
    public string? CallingCode { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}

public sealed class AddressVerificationState
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("slug")]
    public string? Slug { get; init; }
    [JsonPropertyName("abbreviation")]
    public string? Abbreviation { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}
