using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;
using Solentik.Paystack.PaymentRequests.Models;

namespace Solentik.Paystack.PaymentRequests;

internal sealed class PaymentRequestClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), IPaymentRequestClient
{
    public Task<PaystackResponse<PaymentRequest>> CreateAsync(CreatePaymentRequestRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequired(request.Customer, nameof(request.Customer));
        return PostAsync<PaymentRequest>("paymentrequest", request, cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<PaymentRequest>>> ListAsync(PaymentRequestListOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddPositive(query, "page", options?.Page);
        RequestUtilities.Add(query, "customer", options?.Customer);
        RequestUtilities.Add(query, "status", options?.Status);
        RequestUtilities.Add(query, "currency", options?.Currency);
        RequestUtilities.AddBool(query, "include_archive", options?.IncludeArchive);
        RequestUtilities.Add(query, "from", options?.From?.ToString("O", CultureInfo.InvariantCulture));
        RequestUtilities.Add(query, "to", options?.To?.ToString("O", CultureInfo.InvariantCulture));
        return GetAsync<IReadOnlyList<PaymentRequest>>(RequestUtilities.WithQuery("paymentrequest", query), cancellationToken);
    }

    public Task<PaystackResponse<PaymentRequest>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default) =>
        GetAsync<PaymentRequest>($"paymentrequest/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", cancellationToken);

    public Task<PaystackResponse<PaymentRequest>> VerifyAsync(string code, CancellationToken cancellationToken = default) =>
        GetAsync<PaymentRequest>($"paymentrequest/verify/{RequestUtilities.EscapeRequired(code, nameof(code))}", cancellationToken);

    public Task<PaystackResponse<JsonElement>> NotifyAsync(string code, CancellationToken cancellationToken = default) =>
        PostAsync<JsonElement>($"paymentrequest/notify/{RequestUtilities.EscapeRequired(code, nameof(code))}", null, cancellationToken);

    public Task<PaystackResponse<JsonElement>> GetTotalsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<JsonElement>("paymentrequest/totals", cancellationToken);

    public Task<PaystackResponse<PaymentRequest>> FinalizeAsync(string code, bool sendNotification = true, CancellationToken cancellationToken = default) =>
        PostAsync<PaymentRequest>(
            $"paymentrequest/finalize/{RequestUtilities.EscapeRequired(code, nameof(code))}",
            new FinalizePaymentRequestRequest(sendNotification),
            cancellationToken);

    public Task<PaystackResponse<PaymentRequest>> UpdateAsync(string idOrCode, UpdatePaymentRequestRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PutAsync<PaymentRequest>($"paymentrequest/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", request, cancellationToken);
    }

    public Task<PaystackResponse<JsonElement>> ArchiveAsync(string code, CancellationToken cancellationToken = default) =>
        PostAsync<JsonElement>($"paymentrequest/archive/{RequestUtilities.EscapeRequired(code, nameof(code))}", null, cancellationToken);

    private static void ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty.", parameterName);
        }
    }

    private sealed record FinalizePaymentRequestRequest(
        [property: JsonPropertyName("send_notification")] bool SendNotification);
}
