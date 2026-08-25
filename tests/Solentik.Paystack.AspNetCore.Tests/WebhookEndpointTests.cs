using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Solentik.Paystack.AspNetCore.Routing;
using Solentik.Paystack.AspNetCore.Webhooks;
using Solentik.Paystack.Webhooks;

namespace Solentik.Paystack.AspNetCore.Tests;

public sealed class WebhookEndpointTests
{
    private const string Secret = "sk_test_secret";

    [Fact]
    public async Task ValidSignature_DispatchesRawPayload()
    {
        const string payload = "{\"event\":\"charge.success\",\"data\":{\"reference\":\"ref-1\"}}";
        var context = CreateContext(payload);
        context.Request.Headers["x-paystack-signature"] = Sign(payload);
        var dispatcher = new RecordingDispatcher();

        var result = await PaystackWebhookEndpoint.HandleAsync(
            context, CreateVerifier(), dispatcher, Options.Create(new PaystackWebhookOptions()), TestContext.Current.CancellationToken);

        Assert.Equal(StatusCodes.Status200OK, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        Assert.Equal("charge.success", dispatcher.EventName);
        Assert.Equal("ref-1", dispatcher.Data.GetProperty("reference").GetString());
    }

    [Fact]
    public async Task InvalidSignature_IsRejectedBeforeDispatch()
    {
        var context = CreateContext("{\"event\":\"charge.success\"}");
        context.Request.Headers["x-paystack-signature"] = "wrong";
        var dispatcher = new RecordingDispatcher();

        var result = await PaystackWebhookEndpoint.HandleAsync(
            context, CreateVerifier(), dispatcher, Options.Create(new PaystackWebhookOptions()), TestContext.Current.CancellationToken);

        Assert.Equal(StatusCodes.Status401Unauthorized, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
        Assert.Null(dispatcher.EventName);
    }

    [Fact]
    public async Task SignedMalformedJson_ReturnsBadRequest()
    {
        const string payload = "{not-json}";
        var context = CreateContext(payload);
        context.Request.Headers["x-paystack-signature"] = Sign(payload);

        var result = await PaystackWebhookEndpoint.HandleAsync(
            context, CreateVerifier(), new RecordingDispatcher(), Options.Create(new PaystackWebhookOptions()), TestContext.Current.CancellationToken);

        Assert.Equal(StatusCodes.Status400BadRequest, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
    }

    [Fact]
    public async Task MaximumIntegerBodyLimit_DoesNotOverflowBufferSize()
    {
        var context = CreateContext("{}");

        var result = await PaystackWebhookEndpoint.HandleAsync(
            context, CreateVerifier(), new RecordingDispatcher(),
            Options.Create(new PaystackWebhookOptions { MaximumBodySize = int.MaxValue }), TestContext.Current.CancellationToken);

        Assert.Equal(StatusCodes.Status401Unauthorized, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
    }

    [Fact]
    public async Task OversizedBody_ReturnsPayloadTooLarge()
    {
        var context = CreateContext(new string('x', 11));
        var result = await PaystackWebhookEndpoint.HandleAsync(
            context, CreateVerifier(), new RecordingDispatcher(),
            Options.Create(new PaystackWebhookOptions { MaximumBodySize = 10 }), TestContext.Current.CancellationToken);

        Assert.Equal(StatusCodes.Status413PayloadTooLarge, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
    }

    private static DefaultHttpContext CreateContext(string body)
    {
        var bytes = Encoding.UTF8.GetBytes(body);
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(bytes);
        context.Request.ContentLength = bytes.Length;
        return context;
    }

    private static IPaystackWebhookVerifier CreateVerifier() =>
        new PaystackWebhookVerifier(Options.Create(new PaystackOptions { SecretKey = Secret }));

    private static string Sign(string payload) => Convert.ToHexString(
        HMACSHA512.HashData(Encoding.UTF8.GetBytes(Secret), Encoding.UTF8.GetBytes(payload)));

    private sealed class RecordingDispatcher : IPaystackWebhookDispatcher
    {
        public string? EventName { get; private set; }
        public System.Text.Json.JsonElement Data { get; private set; }

        public Task<bool> DispatchAsync(string eventName, System.Text.Json.JsonElement data, System.Text.Json.JsonElement payload, CancellationToken cancellationToken = default)
        {
            EventName = eventName;
            Data = data;
            return Task.FromResult(true);
        }
    }
}
