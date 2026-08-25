using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Solentik.Paystack.AspNetCore.Webhooks;

namespace Solentik.Paystack.AspNetCore.Tests;

public sealed class WebhookDispatcherTests
{
    public static TheoryData<string> RecognizedEvents => new()
    {
        "charge.success",
        "subscription.create",
        "subscription.disable",
        "subscription.not_renew",
        "invoice.create",
        "invoice.update",
        "invoice.payment_failed",
        "charge.dispute.create"
    };

    [Theory]
    [MemberData(nameof(RecognizedEvents))]
    public async Task DispatchAsync_RecognizesEverySupportedEvent(string eventName)
    {
        using var provider = new ServiceCollection().BuildServiceProvider();
        var dispatcher = new PaystackWebhookDispatcher(provider);
        using var document = JsonDocument.Parse("{\"id\":1}");

        var handled = await dispatcher.DispatchAsync(
            eventName,
            document.RootElement.Clone(),
            document.RootElement.Clone(),
            TestContext.Current.CancellationToken);

        Assert.True(handled);
    }

    [Fact]
    public async Task DispatchAsync_InvokesReceivedTypedAndHandledInOrder()
    {
        var calls = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton<IPaystackWebhookHandler<WebhookReceived>>(new CallbackHandler<WebhookReceived>(() => calls.Add("received")));
        services.AddSingleton<IPaystackWebhookHandler<PaymentSuccess>>(new CallbackHandler<PaymentSuccess>(() => calls.Add("payment")));
        services.AddSingleton<IPaystackWebhookHandler<WebhookHandled>>(new CallbackHandler<WebhookHandled>(() => calls.Add("handled")));
        using var provider = services.BuildServiceProvider();
        var dispatcher = new PaystackWebhookDispatcher(provider);
        using var document = JsonDocument.Parse("{\"reference\":\"ref-1\"}");

        await dispatcher.DispatchAsync(
            "charge.success",
            document.RootElement.Clone(),
            document.RootElement.Clone(),
            TestContext.Current.CancellationToken);

        Assert.Equal(["received", "payment", "handled"], calls);
    }

    [Fact]
    public async Task DispatchAsync_UnknownEvent_IsReceivedButNotHandled()
    {
        var received = 0;
        var handled = 0;
        var services = new ServiceCollection();
        services.AddSingleton<IPaystackWebhookHandler<WebhookReceived>>(new CallbackHandler<WebhookReceived>(() => received++));
        services.AddSingleton<IPaystackWebhookHandler<WebhookHandled>>(new CallbackHandler<WebhookHandled>(() => handled++));
        using var provider = services.BuildServiceProvider();
        var dispatcher = new PaystackWebhookDispatcher(provider);
        using var document = JsonDocument.Parse("{}");

        var recognized = await dispatcher.DispatchAsync(
            "future.event",
            document.RootElement.Clone(),
            document.RootElement.Clone(),
            TestContext.Current.CancellationToken);

        Assert.False(recognized);
        Assert.Equal(1, received);
        Assert.Equal(0, handled);
    }

    private sealed class CallbackHandler<TEvent>(Action callback) : IPaystackWebhookHandler<TEvent>
        where TEvent : PaystackWebhookEvent
    {
        public Task HandleAsync(TEvent webhookEvent, CancellationToken cancellationToken = default)
        {
            callback();
            return Task.CompletedTask;
        }
    }
}
