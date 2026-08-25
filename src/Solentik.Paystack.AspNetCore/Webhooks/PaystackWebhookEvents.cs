using System.Text.Json;

namespace Solentik.Paystack.AspNetCore.Webhooks;

public abstract record PaystackWebhookEvent(string Name, JsonElement Data, JsonElement Payload);

public sealed record WebhookReceived(string EventName, JsonElement EventData, JsonElement RawPayload)
    : PaystackWebhookEvent(EventName, EventData, RawPayload);

public sealed record WebhookHandled(string EventName, JsonElement EventData, JsonElement RawPayload)
    : PaystackWebhookEvent(EventName, EventData, RawPayload);

public sealed record PaymentSuccess(JsonElement EventData, JsonElement RawPayload)
    : PaystackWebhookEvent("charge.success", EventData, RawPayload);

public sealed record SubscriptionCreated(JsonElement EventData, JsonElement RawPayload)
    : PaystackWebhookEvent("subscription.create", EventData, RawPayload);

public sealed record SubscriptionDisabled(JsonElement EventData, JsonElement RawPayload)
    : PaystackWebhookEvent("subscription.disable", EventData, RawPayload);

public sealed record SubscriptionNotRenew(JsonElement EventData, JsonElement RawPayload)
    : PaystackWebhookEvent("subscription.not_renew", EventData, RawPayload);

public sealed record InvoiceCreated(JsonElement EventData, JsonElement RawPayload)
    : PaystackWebhookEvent("invoice.create", EventData, RawPayload);

public sealed record InvoiceUpdated(JsonElement EventData, JsonElement RawPayload)
    : PaystackWebhookEvent("invoice.update", EventData, RawPayload);

public sealed record InvoicePaymentFailed(JsonElement EventData, JsonElement RawPayload)
    : PaystackWebhookEvent("invoice.payment_failed", EventData, RawPayload);

public sealed record ChargeDisputeCreated(JsonElement EventData, JsonElement RawPayload)
    : PaystackWebhookEvent("charge.dispute.create", EventData, RawPayload);
