using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Transfers.Models;

/// <summary>Contains the values used to create a transfer recipient.</summary>
public sealed class CreateTransferRecipientRequest
{
    /// <summary>
    /// Paystack documents <c>"nuban"</c> (Nigerian bank), <c>"ghipss"</c> (Ghanaian bank), <c>"mobile_money"</c>,
    /// and <c>"basa"</c> (South African bank). Not validated client-side, since Paystack may add more over time.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>The recipient's name, as registered on the account.</summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>Required unless <see cref="AuthorizationCode"/> is set.</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; init; }

    /// <summary>Required unless <see cref="AuthorizationCode"/> is set. Get valid codes from <c>Miscellaneous.ListBanksAsync</c>.</summary>
    [JsonPropertyName("bank_code")]
    public string? BankCode { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>Creates an authorization-based recipient (reuses a customer's existing authorization) instead
    /// of a bank/mobile-money recipient. When set, <see cref="AccountNumber"/>/<see cref="BankCode"/> are not required.</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; init; }

    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; init; }
}

public sealed class UpdateTransferRecipientRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }
}

public sealed class TransferRecipientListOptions
{
    public int? PerPage { get; init; }
    public int? Page { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}

/// <summary>The bank/mobile-money/authorization details of a <see cref="TransferRecipient"/>.</summary>
public sealed class TransferRecipientDetails
{
    /// <summary>Only present on an authorization-based recipient.</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; init; }

    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; init; }

    /// <summary>Frequently <c>null</c>; Paystack does not always resolve and return the account holder's name here.</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; init; }

    [JsonPropertyName("bank_code")]
    public string? BankCode { get; init; }

    [JsonPropertyName("bank_name")]
    public string? BankName { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}

/// <summary>A Paystack transfer recipient.</summary>
public sealed class TransferRecipient
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("domain")]
    public string? Domain { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("recipient_code")]
    public string? RecipientCode { get; init; }

    [JsonPropertyName("active")]
    public bool? Active { get; init; }

    [JsonPropertyName("details")]
    public TransferRecipientDetails? Details { get; init; }

    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTimeOffset? CreatedAt { get; init; }

    [JsonPropertyName("updatedAt")]
    public DateTimeOffset? UpdatedAt { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}

/// <summary>Result of <c>BulkCreateAsync</c>: recipients Paystack accepted, and any it rejected.</summary>
public sealed class BulkCreateRecipientsResult
{
    [JsonPropertyName("success")]
    public IReadOnlyList<TransferRecipient> Success { get; init; } = [];

    /// <summary>Shape not documented by Paystack; each entry is left as a raw <see cref="JsonElement"/>.</summary>
    [JsonPropertyName("errors")]
    public IReadOnlyList<JsonElement> Errors { get; init; } = [];
}
