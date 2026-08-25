using System.Text.Json;
using Solentik.Paystack.Models;
using Solentik.Paystack.Verification.Models;

namespace Solentik.Paystack.Verification;

public interface IVerificationClient
{
    Task<PaystackResponse<ResolvedAccount>> ResolveAccountAsync(string accountNumber, string bankCode, CancellationToken cancellationToken = default);
    Task<PaystackResponse<JsonElement>> ValidateAccountAsync(ValidateAccountRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<CardBinInfo>> ResolveCardBinAsync(string bin, CancellationToken cancellationToken = default);
}
