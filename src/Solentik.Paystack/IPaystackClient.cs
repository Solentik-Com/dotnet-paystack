using Solentik.Paystack.Customers;
using Solentik.Paystack.Miscellaneous;
using Solentik.Paystack.PaymentRequests;
using Solentik.Paystack.Plans;
using Solentik.Paystack.Subaccounts;
using Solentik.Paystack.Subscriptions;
using Solentik.Paystack.TransactionSplits;
using Solentik.Paystack.Transactions;
using Solentik.Paystack.Transfers;
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
    ISubaccountClient Subaccounts { get; }
    IMiscellaneousClient Miscellaneous { get; }
    IVerificationClient Verification { get; }
    IPaymentRequestClient PaymentRequests { get; }
    ITransferClient Transfers { get; }
    ITransferRecipientClient TransferRecipients { get; }
    ITransferControlClient TransferControl { get; }
}

internal sealed class PaystackClient(
    ITransactionClient transactions,
    ICustomerClient customers,
    IPlanClient plans,
    ISubscriptionClient subscriptions,
    ITransactionSplitClient transactionSplits,
    ISubaccountClient subaccounts,
    IMiscellaneousClient miscellaneous,
    IVerificationClient verification,
    IPaymentRequestClient paymentRequests,
    ITransferClient transfers,
    ITransferRecipientClient transferRecipients,
    ITransferControlClient transferControl) : IPaystackClient
{
    public ITransactionClient Transactions { get; } = transactions;
    public ICustomerClient Customers { get; } = customers;
    public IPlanClient Plans { get; } = plans;
    public ISubscriptionClient Subscriptions { get; } = subscriptions;
    public ITransactionSplitClient TransactionSplits { get; } = transactionSplits;
    public ISubaccountClient Subaccounts { get; } = subaccounts;
    public IMiscellaneousClient Miscellaneous { get; } = miscellaneous;
    public IVerificationClient Verification { get; } = verification;
    public IPaymentRequestClient PaymentRequests { get; } = paymentRequests;
    public ITransferClient Transfers { get; } = transfers;
    public ITransferRecipientClient TransferRecipients { get; } = transferRecipients;
    public ITransferControlClient TransferControl { get; } = transferControl;
}
