using System.Text.Json;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;
using Solentik.Paystack.Verification.Models;

namespace Solentik.Paystack.Verification;

internal sealed class VerificationClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), IVerificationClient
{
    public Task<PaystackResponse<ResolvedAccount>> ResolveAccountAsync(
        string accountNumber,
        string bankCode,
        CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddRequired(query, "account_number", accountNumber, nameof(accountNumber));
        RequestUtilities.AddRequired(query, "bank_code", bankCode, nameof(bankCode));

        return GetAsync<ResolvedAccount>(RequestUtilities.WithQuery("bank/resolve", query), cancellationToken);
    }

    public Task<PaystackResponse<JsonElement>> ValidateAccountAsync(ValidateAccountRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<JsonElement>("bank/validate", request, cancellationToken);
    }

    public Task<PaystackResponse<CardBinInfo>> ResolveCardBinAsync(string bin, CancellationToken cancellationToken = default) =>
        GetAsync<CardBinInfo>($"decision/bin/{RequestUtilities.EscapeRequired(bin, nameof(bin))}", cancellationToken);
}
