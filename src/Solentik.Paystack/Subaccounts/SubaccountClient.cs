using System.Globalization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;
using Solentik.Paystack.Subaccounts.Models;

namespace Solentik.Paystack.Subaccounts;

internal sealed class SubaccountClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), ISubaccountClient
{
    public Task<PaystackResponse<Subaccount>> CreateAsync(CreateSubaccountRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequired(request.BusinessName, nameof(request.BusinessName));
        ValidateRequired(request.SettlementBank, nameof(request.SettlementBank));
        ValidateRequired(request.AccountNumber, nameof(request.AccountNumber));
        ValidatePercentageCharge(request.PercentageCharge, nameof(request.PercentageCharge));
        return PostAsync<Subaccount>("subaccount", request, cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<Subaccount>>> ListAsync(SubaccountListOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddPositive(query, "page", options?.Page);
        RequestUtilities.Add(query, "from", options?.From?.ToString("O", CultureInfo.InvariantCulture));
        RequestUtilities.Add(query, "to", options?.To?.ToString("O", CultureInfo.InvariantCulture));
        return GetAsync<IReadOnlyList<Subaccount>>(RequestUtilities.WithQuery("subaccount", query), cancellationToken);
    }

    public Task<PaystackResponse<Subaccount>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default) =>
        GetAsync<Subaccount>($"subaccount/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", cancellationToken);

    public Task<PaystackResponse<Subaccount>> UpdateAsync(string idOrCode, UpdateSubaccountRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.PercentageCharge is not null)
        {
            ValidatePercentageCharge(request.PercentageCharge.Value, nameof(request.PercentageCharge));
        }

        return PutAsync<Subaccount>($"subaccount/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", request, cancellationToken);
    }

    private static void ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty.", parameterName);
        }
    }

    private static void ValidatePercentageCharge(decimal percentageCharge, string parameterName)
    {
        if (percentageCharge <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "The percentage charge must be greater than zero.");
        }
    }
}
