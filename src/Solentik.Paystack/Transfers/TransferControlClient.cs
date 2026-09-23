using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;
using Solentik.Paystack.Transfers.Models;

namespace Solentik.Paystack.Transfers;

internal sealed class TransferControlClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), ITransferControlClient
{
    public Task<PaystackResponse<IReadOnlyList<BalanceEntry>>> CheckBalanceAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<BalanceEntry>>("balance", cancellationToken);

    public Task<PaystackResponse<IReadOnlyList<BalanceLedgerEntry>>> FetchLedgerAsync(BalanceLedgerListOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddPositive(query, "page", options?.Page);
        RequestUtilities.Add(query, "from", options?.From?.ToString("O", CultureInfo.InvariantCulture));
        RequestUtilities.Add(query, "to", options?.To?.ToString("O", CultureInfo.InvariantCulture));
        return GetAsync<IReadOnlyList<BalanceLedgerEntry>>(RequestUtilities.WithQuery("balance/ledger", query), cancellationToken);
    }

    public Task<PaystackResponse<JsonElement>> ResendOtpAsync(string transferCode, string reason = "resend_otp", CancellationToken cancellationToken = default)
    {
        ValidateRequired(transferCode, nameof(transferCode));
        return PostAsync<JsonElement>(
            "transfer/resend_otp",
            new ResendOtpPayload { TransferCode = transferCode, Reason = reason },
            cancellationToken);
    }

    public Task<PaystackResponse<JsonElement>> RequestDisableOtpAsync(CancellationToken cancellationToken = default) =>
        PostAsync<JsonElement>("transfer/disable_otp", null, cancellationToken);

    public Task<PaystackResponse<JsonElement>> FinalizeDisableOtpAsync(string otp, CancellationToken cancellationToken = default)
    {
        ValidateRequired(otp, nameof(otp));
        return PostAsync<JsonElement>("transfer/disable_otp_finalize", new OtpPayload { Otp = otp }, cancellationToken);
    }

    public Task<PaystackResponse<JsonElement>> EnableOtpAsync(CancellationToken cancellationToken = default) =>
        PostAsync<JsonElement>("transfer/enable_otp", null, cancellationToken);

    private static void ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty.", parameterName);
        }
    }
}

internal sealed class ResendOtpPayload
{
    [JsonPropertyName("transfer_code")]
    public required string TransferCode { get; init; }

    [JsonPropertyName("reason")]
    public required string Reason { get; init; }
}

internal sealed class OtpPayload
{
    [JsonPropertyName("otp")]
    public required string Otp { get; init; }
}
