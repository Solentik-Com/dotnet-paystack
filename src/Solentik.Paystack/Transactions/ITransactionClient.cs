using Solentik.Paystack.Models;
using Solentik.Paystack.Transactions.Models;

namespace Solentik.Paystack.Transactions;

/// <summary>Provides access to Paystack transaction endpoints.</summary>
public interface ITransactionClient
{
    Task<PaystackResponse<InitializeTransactionData>> InitializeAsync(
        InitializeTransactionRequest request,
        CancellationToken cancellationToken = default);

    Task<PaystackResponse<Transaction>> VerifyAsync(
        string reference,
        CancellationToken cancellationToken = default);

    Task<PaystackResponse<Transaction>> FetchAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<PaystackResponse<IReadOnlyList<Transaction>>> ListAsync(
        TransactionListOptions? options = null,
        CancellationToken cancellationToken = default);
}
