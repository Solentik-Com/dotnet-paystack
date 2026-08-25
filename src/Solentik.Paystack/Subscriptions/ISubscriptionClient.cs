using System.Text.Json;
using Solentik.Paystack.Models;
using Solentik.Paystack.Subscriptions.Models;

namespace Solentik.Paystack.Subscriptions;

public interface ISubscriptionClient
{
    Task<PaystackResponse<Subscription>> CreateAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<IReadOnlyList<Subscription>>> ListAsync(SubscriptionListOptions? options = null, CancellationToken cancellationToken = default);
    Task<PaystackResponse<Subscription>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default);
    Task<PaystackResponse<JsonElement>> EnableAsync(string code, string token, CancellationToken cancellationToken = default);
    Task<PaystackResponse<JsonElement>> DisableAsync(string code, string token, CancellationToken cancellationToken = default);
    Task<PaystackResponse<SubscriptionManagementLink>> GetUpdateLinkAsync(string code, CancellationToken cancellationToken = default);
    Task<PaystackResponse<JsonElement>> SendUpdateLinkAsync(string code, CancellationToken cancellationToken = default);
}
