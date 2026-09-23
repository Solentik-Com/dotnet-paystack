using Solentik.Paystack.Models;
using Solentik.Paystack.Transfers.Models;

namespace Solentik.Paystack.Transfers;

public interface ITransferClient
{
    Task<PaystackResponse<Transfer>> InitiateAsync(InitiateTransferRequest request, CancellationToken cancellationToken = default);

    /// <summary>Required if Transfers OTP is enabled on the integration (the default).</summary>
    Task<PaystackResponse<Transfer>> FinalizeAsync(string transferCode, string otp, CancellationToken cancellationToken = default);

    /// <summary>Requires Transfers OTP to be disabled first (see <c>ITransferControlClient</c>).</summary>
    Task<PaystackResponse<IReadOnlyList<BulkTransferResult>>> BulkInitiateAsync(BulkTransferRequest request, CancellationToken cancellationToken = default);

    Task<PaystackResponse<IReadOnlyList<Transfer>>> ListAsync(TransferListOptions? options = null, CancellationToken cancellationToken = default);

    Task<PaystackResponse<Transfer>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default);

    Task<PaystackResponse<Transfer>> VerifyAsync(string reference, CancellationToken cancellationToken = default);
}
