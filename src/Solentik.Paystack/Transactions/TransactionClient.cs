using System.Globalization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;
using Solentik.Paystack.Transactions.Models;

namespace Solentik.Paystack.Transactions;

internal sealed class TransactionClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), ITransactionClient
{
    public Task<PaystackResponse<InitializeTransactionData>> InitializeAsync(
        InitializeTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("A customer email address is required.", nameof(request));
        }

        if (request.Amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "The amount must be greater than zero.");
        }

        return PostAsync<InitializeTransactionData>("transaction/initialize", request, cancellationToken);
    }

    public Task<PaystackResponse<Transaction>> VerifyAsync(
        string reference,
        CancellationToken cancellationToken = default) =>
        GetAsync<Transaction>(
            $"transaction/verify/{RequestUtilities.EscapeRequired(reference, nameof(reference))}",
            cancellationToken);

    public Task<PaystackResponse<Transaction>> FetchAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "The transaction ID must be greater than zero.");
        }

        return GetAsync<Transaction>($"transaction/{id}", cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<Transaction>>> ListAsync(
        TransactionListOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddPositive(query, "page", options?.Page);
        RequestUtilities.Add(query, "customer", options?.Customer);
        RequestUtilities.Add(query, "status", options?.Status);
        RequestUtilities.Add(query, "from", options?.From?.ToString("O", CultureInfo.InvariantCulture));
        RequestUtilities.Add(query, "to", options?.To?.ToString("O", CultureInfo.InvariantCulture));

        return GetAsync<IReadOnlyList<Transaction>>(RequestUtilities.WithQuery("transaction", query), cancellationToken);
    }
}
