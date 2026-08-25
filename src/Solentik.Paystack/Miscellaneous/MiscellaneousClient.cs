using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Miscellaneous.Models;
using Solentik.Paystack.Models;

namespace Solentik.Paystack.Miscellaneous;

internal sealed class MiscellaneousClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), IMiscellaneousClient
{
    public Task<PaystackResponse<IReadOnlyList<Bank>>> ListBanksAsync(BankListOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.Add(query, "country", options?.Country);
        RequestUtilities.AddBool(query, "use_cursor", options?.UseCursor);
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddBool(query, "pay_with_bank_transfer", options?.PayWithBankTransfer);
        RequestUtilities.AddBool(query, "pay_with_bank", options?.PayWithBank);
        RequestUtilities.AddBool(query, "enabled_for_verification", options?.EnabledForVerification);
        RequestUtilities.Add(query, "next", options?.Next);
        RequestUtilities.Add(query, "previous", options?.Previous);
        RequestUtilities.Add(query, "gateway", options?.Gateway);
        RequestUtilities.Add(query, "type", options?.Type);
        RequestUtilities.Add(query, "currency", options?.Currency);
        RequestUtilities.AddBool(query, "include_nip_sort_code", options?.IncludeNipSortCode);

        return GetAsync<IReadOnlyList<Bank>>(RequestUtilities.WithQuery("bank", query), cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<Country>>> ListCountriesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<Country>>("country", cancellationToken);

    public Task<PaystackResponse<IReadOnlyList<AddressVerificationState>>> ListStatesAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddRequired(query, "country", countryCode, nameof(countryCode));

        return GetAsync<IReadOnlyList<AddressVerificationState>>(RequestUtilities.WithQuery("address_verification/states", query), cancellationToken);
    }
}
