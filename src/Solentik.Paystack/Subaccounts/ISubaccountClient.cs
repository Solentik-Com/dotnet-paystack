using Solentik.Paystack.Models;
using Solentik.Paystack.Subaccounts.Models;

namespace Solentik.Paystack.Subaccounts;

public interface ISubaccountClient
{
    Task<PaystackResponse<Subaccount>> CreateAsync(CreateSubaccountRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<IReadOnlyList<Subaccount>>> ListAsync(SubaccountListOptions? options = null, CancellationToken cancellationToken = default);
    Task<PaystackResponse<Subaccount>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default);
    Task<PaystackResponse<Subaccount>> UpdateAsync(string idOrCode, UpdateSubaccountRequest request, CancellationToken cancellationToken = default);
}
