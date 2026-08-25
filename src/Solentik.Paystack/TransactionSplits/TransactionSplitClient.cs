using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;
using Solentik.Paystack.TransactionSplits.Models;

namespace Solentik.Paystack.TransactionSplits;

internal sealed class TransactionSplitClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), ITransactionSplitClient
{
    public Task<PaystackResponse<TransactionSplit>> CreateAsync(CreateTransactionSplitRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Subaccounts.Count == 0)
        {
            throw new ArgumentException("At least one subaccount is required.", nameof(request));
        }
        ValidateSubaccounts(request.Subaccounts);
        return PostAsync<TransactionSplit>("split", request, cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<TransactionSplit>>> ListAsync(TransactionSplitListOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.Add(query, "name", options?.Name);
        RequestUtilities.AddBool(query, "active", options?.Active);
        RequestUtilities.Add(query, "sort_by", options?.SortBy);
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddPositive(query, "page", options?.Page);
        RequestUtilities.Add(query, "from", options?.From?.ToString("O", CultureInfo.InvariantCulture));
        RequestUtilities.Add(query, "to", options?.To?.ToString("O", CultureInfo.InvariantCulture));
        return GetAsync<IReadOnlyList<TransactionSplit>>(RequestUtilities.WithQuery("split", query), cancellationToken);
    }

    public Task<PaystackResponse<TransactionSplit>> FetchAsync(string id, CancellationToken cancellationToken = default) =>
        GetAsync<TransactionSplit>($"split/{RequestUtilities.EscapeRequired(id, nameof(id))}", cancellationToken);

    public Task<PaystackResponse<TransactionSplit>> UpdateAsync(string id, UpdateTransactionSplitRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PutAsync<TransactionSplit>($"split/{RequestUtilities.EscapeRequired(id, nameof(id))}", request, cancellationToken);
    }

    public Task<PaystackResponse<TransactionSplit>> AddSubaccountAsync(string id, SplitSubaccountRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateSubaccounts([request]);
        return PostAsync<TransactionSplit>($"split/{RequestUtilities.EscapeRequired(id, nameof(id))}/subaccount/add", request, cancellationToken);
    }

    public Task<PaystackResponse<TransactionSplit>> RemoveSubaccountAsync(string id, string subaccountCode, CancellationToken cancellationToken = default) =>
        PostAsync<TransactionSplit>(
            $"split/{RequestUtilities.EscapeRequired(id, nameof(id))}/subaccount/remove",
            new RemoveSubaccountRequest(ValidateRequired(subaccountCode, nameof(subaccountCode))),
            cancellationToken);

    private static void ValidateSubaccounts(IEnumerable<SplitSubaccountRequest> subaccounts)
    {
        foreach (var item in subaccounts)
        {
            ValidateRequired(item.Subaccount, nameof(item.Subaccount));
            if (item.Share <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(item.Share), "The share must be greater than zero.");
            }
        }
    }

    private static string ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty.", parameterName);
        }
        return value;
    }

    private sealed record RemoveSubaccountRequest(
        [property: JsonPropertyName("subaccount")] string Subaccount);
}
