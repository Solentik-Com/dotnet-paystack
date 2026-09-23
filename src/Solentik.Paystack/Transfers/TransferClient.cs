using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;
using Solentik.Paystack.Transfers.Models;

namespace Solentik.Paystack.Transfers;

internal sealed class TransferClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), ITransferClient
{
    public Task<PaystackResponse<Transfer>> InitiateAsync(InitiateTransferRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequired(request.Recipient, nameof(request.Recipient));
        ValidatePositive(request.Amount, nameof(request.Amount));
        return PostAsync<Transfer>("transfer", request, cancellationToken);
    }

    public Task<PaystackResponse<Transfer>> FinalizeAsync(string transferCode, string otp, CancellationToken cancellationToken = default)
    {
        ValidateRequired(transferCode, nameof(transferCode));
        ValidateRequired(otp, nameof(otp));
        return PostAsync<Transfer>(
            "transfer/finalize_transfer",
            new FinalizeTransferPayload { TransferCode = transferCode, Otp = otp },
            cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<BulkTransferResult>>> BulkInitiateAsync(BulkTransferRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Transfers.Count == 0)
        {
            throw new ArgumentException("At least one transfer is required.", nameof(request));
        }

        return PostAsync<IReadOnlyList<BulkTransferResult>>("transfer/bulk", request, cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<Transfer>>> ListAsync(TransferListOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddPositive(query, "page", options?.Page);
        RequestUtilities.AddPositive(query, "recipient", options?.Recipient);
        RequestUtilities.Add(query, "from", options?.From?.ToString("O", CultureInfo.InvariantCulture));
        RequestUtilities.Add(query, "to", options?.To?.ToString("O", CultureInfo.InvariantCulture));
        return GetAsync<IReadOnlyList<Transfer>>(RequestUtilities.WithQuery("transfer", query), cancellationToken);
    }

    public Task<PaystackResponse<Transfer>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default) =>
        GetAsync<Transfer>($"transfer/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", cancellationToken);

    public Task<PaystackResponse<Transfer>> VerifyAsync(string reference, CancellationToken cancellationToken = default) =>
        GetAsync<Transfer>($"transfer/verify/{RequestUtilities.EscapeRequired(reference, nameof(reference))}", cancellationToken);

    private static void ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty.", parameterName);
        }
    }

    private static void ValidatePositive(long value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "The amount must be greater than zero.");
        }
    }
}

internal sealed class FinalizeTransferPayload
{
    [JsonPropertyName("transfer_code")]
    public required string TransferCode { get; init; }

    [JsonPropertyName("otp")]
    public required string Otp { get; init; }
}
