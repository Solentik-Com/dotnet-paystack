namespace Solentik.Paystack.AspNetCore.Webhooks;

/// <summary>Handles one type of verified Paystack webhook event.</summary>
public interface IPaystackWebhookHandler<in TEvent> where TEvent : PaystackWebhookEvent
{
    Task HandleAsync(TEvent webhookEvent, CancellationToken cancellationToken = default);
}
