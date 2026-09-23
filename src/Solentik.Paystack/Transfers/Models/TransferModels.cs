using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.Transfers.Models;

/// <summary>Contains the values used to initiate a transfer out of the Paystack balance.</summary>
public sealed class InitiateTransferRequest
{
    /// <summary>Currently limited to "balance" by Paystack.</summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = "balance";

    /// <summary>The amount to transfer, in the currency's smallest unit.</summary>
    [JsonPropertyName("amount")]
    public required long Amount { get; init; }

    /// <summary>The recipient_code of a recipient created with <c>ITransferRecipientClient.CreateAsync</c>.</summary>
    [JsonPropertyName("recipient")]
    public required string Recipient { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>Defaults to "NGN" on Paystack's side if omitted.</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// A unique identifier for this transfer, 16-50 characters: lowercase letters, digits, dash, and
    /// underscore only. Paystack generates one if omitted.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>Required in Kenya for MPESA Paybill and Till transfers; unused elsewhere.</summary>
    [JsonPropertyName("account_reference")]
    public string? AccountReference { get; init; }
}

public sealed class BulkTransferItem
{
    /// <summary>The amount to transfer, in the currency's smallest unit.</summary>
    [JsonPropertyName("amount")]
    public required long Amount { get; init; }

    /// <summary>The recipient_code of a recipient created with <c>ITransferRecipientClient.CreateAsync</c>.</summary>
    [JsonPropertyName("recipient")]
    public required string Recipient { get; init; }

    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}

/// <summary>
/// Contains the values used to initiate several transfers in one request. You must disable Transfers OTP
/// (<c>ITransferControlClient.RequestDisableOtpAsync</c>/<c>FinalizeDisableOtpAsync</c>) to use this.
/// </summary>
public sealed class BulkTransferRequest
{
    /// <summary>Currently limited to "balance" by Paystack.</summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = "balance";

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = "NGN";

    [JsonPropertyName("transfers")]
    public required IReadOnlyList<BulkTransferItem> Transfers { get; init; }
}

public sealed class TransferListOptions
{
    public int? PerPage { get; init; }
    public int? Page { get; init; }

    /// <summary>Filters by the recipient's numeric ID (not its "RCP_..." code).</summary>
    public long? Recipient { get; init; }

    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}

/// <summary>
/// A Paystack transfer, as returned by <c>InitiateAsync</c>, <c>FinalizeAsync</c>, <c>ListAsync</c>,
/// <c>FetchAsync</c>, and <c>VerifyAsync</c>.
/// </summary>
public sealed class Transfer
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("domain")]
    public string? Domain { get; init; }

    /// <summary>The amount transferred, in the currency's smallest unit.</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("source")]
    public string? Source { get; init; }

    /// <summary>Shape not documented by Paystack; observed only as <c>null</c>.</summary>
    [JsonPropertyName("source_details")]
    public JsonElement? SourceDetails { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>
    /// The recipient's bare numeric ID on the response from <c>InitiateAsync</c>/<c>FinalizeAsync</c>, but a
    /// fully expanded object shaped like <see cref="TransferRecipient"/> on the response from
    /// <c>ListAsync</c>/<c>FetchAsync</c>/<c>VerifyAsync</c>. Left as <see cref="JsonElement"/> rather than
    /// two different property names, since which shape you get depends entirely on which call you made.
    /// </summary>
    [JsonPropertyName("recipient")]
    public JsonElement? Recipient { get; init; }

    /// <summary>
    /// One of: <c>"otp"</c> (awaiting an OTP the customer must supply to <c>FinalizeAsync</c>),
    /// <c>"abandoned"</c> (the OTP wasn't used within 30 minutes), <c>"failed"</c> (wrong OTP, or a general
    /// failure), <c>"received"</c> (awaiting a response from your configured approval URL, if you use one),
    /// <c>"pending"</c> (approved and being processed), <c>"rejected"</c> (your approval URL rejected it),
    /// <c>"blocked"</c> (your approval URL didn't respond in time), or <c>"success"</c>/<c>"reversed"</c> for
    /// a finished transfer.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>Shape not documented by Paystack; observed only as <c>null</c>.</summary>
    [JsonPropertyName("failures")]
    public JsonElement? Failures { get; init; }

    [JsonPropertyName("transfer_code")]
    public string? TransferCode { get; init; }

    /// <summary>Observed only as <c>null</c> so far. Kept as a raw value rather than a parsed date in case its
    /// format doesn't match <see cref="CreatedAt"/>/<see cref="UpdatedAt"/> when it is eventually populated.</summary>
    [JsonPropertyName("transferred_at")]
    public string? TransferredAt { get; init; }

    /// <summary>Populated only in specific failure/reversal scenarios Paystack hasn't fully documented.</summary>
    [JsonPropertyName("titan_code")]
    public string? TitanCode { get; init; }

    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTimeOffset? CreatedAt { get; init; }

    [JsonPropertyName("updatedAt")]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>Paystack's fee for this transfer, in the currency's smallest unit. Only present on <c>FetchAsync</c>/<c>VerifyAsync</c>.</summary>
    [JsonPropertyName("fee_charged")]
    public long? FeeCharged { get; init; }

    /// <summary>Shape not documented by Paystack; observed only as <c>null</c>.</summary>
    [JsonPropertyName("fees_breakdown")]
    public JsonElement? FeesBreakdown { get; init; }

    /// <summary>Shape not documented by Paystack; observed only as <c>null</c>.</summary>
    [JsonPropertyName("gateway_response")]
    public JsonElement? GatewayResponse { get; init; }

    /// <summary>The approval-URL session, when one is configured: <c>{ provider, id }</c>. Only present on <c>FetchAsync</c>/<c>VerifyAsync</c>.</summary>
    [JsonPropertyName("session")]
    public JsonElement? Session { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}

/// <summary>An individual result from <c>BulkInitiateAsync</c>; a smaller shape than <see cref="Transfer"/>.</summary>
public sealed class BulkTransferResult
{
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>The recipient_code, as a string (unlike the bare numeric ID <see cref="Transfer.Recipient"/> carries on a single initiate).</summary>
    [JsonPropertyName("recipient")]
    public string? Recipient { get; init; }

    /// <summary>The amount transferred, in the currency's smallest unit.</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; init; }

    [JsonPropertyName("transfer_code")]
    public string? TransferCode { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}
