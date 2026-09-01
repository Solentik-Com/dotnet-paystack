using System.Text.Json;
using Solentik.Paystack.Models;
using Solentik.Paystack.PaymentRequests.Models;

namespace Solentik.Paystack.PaymentRequests;

/// <summary>Provides access to Paystack's Payment Request (invoicing) API.</summary>
public interface IPaymentRequestClient
{
    Task<PaystackResponse<PaymentRequest>> CreateAsync(CreatePaymentRequestRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<IReadOnlyList<PaymentRequest>>> ListAsync(PaymentRequestListOptions? options = null, CancellationToken cancellationToken = default);
    Task<PaystackResponse<PaymentRequest>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default);
    Task<PaystackResponse<PaymentRequest>> VerifyAsync(string code, CancellationToken cancellationToken = default);
    Task<PaystackResponse<JsonElement>> NotifyAsync(string code, CancellationToken cancellationToken = default);
    Task<PaystackResponse<JsonElement>> GetTotalsAsync(CancellationToken cancellationToken = default);
    Task<PaystackResponse<PaymentRequest>> FinalizeAsync(string code, bool sendNotification = true, CancellationToken cancellationToken = default);
    Task<PaystackResponse<PaymentRequest>> UpdateAsync(string idOrCode, UpdatePaymentRequestRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<JsonElement>> ArchiveAsync(string code, CancellationToken cancellationToken = default);
}
