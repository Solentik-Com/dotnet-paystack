using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Solentik.Paystack.AspNetCore.Webhooks;

internal sealed class PaystackWebhookDispatcher(IServiceProvider services) : IPaystackWebhookDispatcher
{
    public async Task<bool> DispatchAsync(
        string eventName,
        JsonElement data,
        JsonElement payload,
        CancellationToken cancellationToken = default)
    {
        await DispatchToHandlersAsync(new WebhookReceived(eventName, data, payload), cancellationToken);

        var handled = eventName switch
        {
            "charge.success" => await DispatchToHandlersAsync(new PaymentSuccess(data, payload), cancellationToken),
            "subscription.create" => await DispatchToHandlersAsync(new SubscriptionCreated(data, payload), cancellationToken),
            "subscription.disable" => await DispatchToHandlersAsync(new SubscriptionDisabled(data, payload), cancellationToken),
            "subscription.not_renew" => await DispatchToHandlersAsync(new SubscriptionNotRenew(data, payload), cancellationToken),
            "invoice.create" => await DispatchToHandlersAsync(new InvoiceCreated(data, payload), cancellationToken),
            "invoice.update" => await DispatchToHandlersAsync(new InvoiceUpdated(data, payload), cancellationToken),
            "invoice.payment_failed" => await DispatchToHandlersAsync(new InvoicePaymentFailed(data, payload), cancellationToken),
            "charge.dispute.create" => await DispatchToHandlersAsync(new ChargeDisputeCreated(data, payload), cancellationToken),
            _ => false
        };

        if (handled)
        {
            await DispatchToHandlersAsync(new WebhookHandled(eventName, data, payload), cancellationToken);
        }

        return handled;
    }

    private async Task<bool> DispatchToHandlersAsync<TEvent>(TEvent webhookEvent, CancellationToken cancellationToken)
        where TEvent : PaystackWebhookEvent
    {
        foreach (var handler in services.GetServices<IPaystackWebhookHandler<TEvent>>())
        {
            await handler.HandleAsync(webhookEvent, cancellationToken).ConfigureAwait(false);
        }

        return true;
    }
}
