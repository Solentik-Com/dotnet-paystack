using Solentik.Paystack.Customers;
using Solentik.Paystack.Miscellaneous;
using Solentik.Paystack.Plans;
using Solentik.Paystack.Subscriptions;
using Solentik.Paystack.TransactionSplits;
using Solentik.Paystack.Transactions;
using Solentik.Paystack.Verification;

namespace Solentik.Paystack;

/// <summary>Provides access to Paystack API resources.</summary>
public interface IPaystackClient
{
    ITransactionClient Transactions { get; }
    ICustomerClient Customers { get; }
    IPlanClient Plans { get; }
    ISubscriptionClient Subscriptions { get; }
    ITransactionSplitClient TransactionSplits { get; }
    IMiscellaneousClient Miscellaneous { get; }
    IVerificationClient Verification { get; }
}

internal sealed class PaystackClient(
    ITransactionClient transactions,
    ICustomerClient customers,
    IPlanClient plans,
    ISubscriptionClient subscriptions,
    ITransactionSplitClient transactionSplits,
    IMiscellaneousClient miscellaneous,
    IVerificationClient verification) : IPaystackClient
{
    public ITransactionClient Transactions { get; } = transactions;
    public ICustomerClient Customers { get; } = customers;
    public IPlanClient Plans { get; } = plans;
    public ISubscriptionClient Subscriptions { get; } = subscriptions;
    public ITransactionSplitClient TransactionSplits { get; } = transactionSplits;
    public IMiscellaneousClient Miscellaneous { get; } = miscellaneous;
    public IVerificationClient Verification { get; } = verification;
}
