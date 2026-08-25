using Solentik.Paystack.Models;
using Solentik.Paystack.TransactionSplits.Models;

namespace Solentik.Paystack.TransactionSplits;

public interface ITransactionSplitClient
{
    Task<PaystackResponse<TransactionSplit>> CreateAsync(CreateTransactionSplitRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<IReadOnlyList<TransactionSplit>>> ListAsync(TransactionSplitListOptions? options = null, CancellationToken cancellationToken = default);
    Task<PaystackResponse<TransactionSplit>> FetchAsync(string id, CancellationToken cancellationToken = default);
    Task<PaystackResponse<TransactionSplit>> UpdateAsync(string id, UpdateTransactionSplitRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<TransactionSplit>> AddSubaccountAsync(string id, SplitSubaccountRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<TransactionSplit>> RemoveSubaccountAsync(string id, string subaccountCode, CancellationToken cancellationToken = default);
}
