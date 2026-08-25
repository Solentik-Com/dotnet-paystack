using Solentik.Paystack.Miscellaneous.Models;
using Solentik.Paystack.Models;

namespace Solentik.Paystack.Miscellaneous;

public interface IMiscellaneousClient
{
    Task<PaystackResponse<IReadOnlyList<Bank>>> ListBanksAsync(BankListOptions? options = null, CancellationToken cancellationToken = default);
    Task<PaystackResponse<IReadOnlyList<Country>>> ListCountriesAsync(CancellationToken cancellationToken = default);
    Task<PaystackResponse<IReadOnlyList<AddressVerificationState>>> ListStatesAsync(string countryCode, CancellationToken cancellationToken = default);
}
