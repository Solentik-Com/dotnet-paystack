using System.Text.Json;

namespace Solentik.Paystack.AspNetCore.Webhooks;

/// <summary>Routes an incoming Paystack webhook to the registered <see cref="IPaystackWebhookHandler{TEvent}"/> instances.</summary>
public interface IPaystackWebhookDispatcher
{
    /// <summary>
    /// Dispatches <see cref="WebhookReceived"/>, then the typed event for <paramref name="eventName"/> (if recognized),
    /// then <see cref="WebhookHandled"/> when it was.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="eventName"/> is a Paystack event this package models, regardless of
    /// whether any <see cref="IPaystackWebhookHandler{TEvent}"/> is registered for it; <see langword="false"/> for an
    /// unrecognized event name. This does not indicate that a handler ran - register a handler for
    /// <see cref="WebhookReceived"/> to observe every webhook, recognized or not.
    /// </returns>
    Task<bool> DispatchAsync(string eventName, JsonElement data, JsonElement payload, CancellationToken cancellationToken = default);
}
