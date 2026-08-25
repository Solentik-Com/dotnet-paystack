using Microsoft.Extensions.Options;
using Solentik.Paystack.Subscriptions;
using Solentik.Paystack.Subscriptions.Models;

namespace Solentik.Paystack.Tests;

public sealed class SubscriptionClientTests
{
    [Fact]
    public async Task SupportsCompleteSubscriptionEndpointSurface()
    {
        const string subscription = "{\"status\":true,\"data\":{\"subscription_code\":\"SUB_1\",\"status\":\"active\"}}";
        const string empty = "{\"status\":true,\"message\":\"Success\"}";
        var handler = new RecordingHttpMessageHandler(
            subscription, "{\"status\":true,\"data\":[]}", subscription,
            empty, empty, "{\"status\":true,\"data\":{\"link\":\"https://manage.test/SUB_1\"}}", empty);
        var client = new SubscriptionClient(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        var startDate = new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero);

        var cancellationToken = TestContext.Current.CancellationToken;
        var created = await client.CreateAsync(new CreateSubscriptionRequest { Customer = "CUS_1", Plan = "PLN_1", StartDate = startDate }, cancellationToken);
        await client.ListAsync(new SubscriptionListOptions { Customer = "CUS_1", Plan = "PLN_1" }, cancellationToken);
        await client.FetchAsync("SUB/1", cancellationToken);
        await client.EnableAsync("SUB_1", "token+exact", cancellationToken);
        await client.DisableAsync("SUB_1", "token+exact", cancellationToken);
        var link = await client.GetUpdateLinkAsync("SUB_1", cancellationToken);
        await client.SendUpdateLinkAsync("SUB_1", cancellationToken);

        Assert.Equal("SUB_1", created.Data?.SubscriptionCode);
        Assert.Equal("https://manage.test/SUB_1", link.Data?.Link);
        Assert.Collection(handler.Requests,
            request => { Assert.Equal("/subscription", request.Uri.AbsolutePath); Assert.Contains("2026-09-01", request.Body); },
            request => { Assert.Contains("customer=CUS_1", request.Uri.Query); Assert.Contains("plan=PLN_1", request.Uri.Query); },
            request => Assert.Equal("/subscription/SUB%2F1", request.Uri.AbsolutePath),
            request => { Assert.Equal("/subscription/enable", request.Uri.AbsolutePath); AssertToken(request.Body); },
            request => { Assert.Equal("/subscription/disable", request.Uri.AbsolutePath); AssertToken(request.Body); },
            request => Assert.Equal("/subscription/SUB_1/manage/link", request.Uri.AbsolutePath),
            request => { Assert.Equal(HttpMethod.Post, request.Method); Assert.Equal("/subscription/SUB_1/manage/email", request.Uri.AbsolutePath); });
    }

    private static void AssertToken(string body)
    {
        using var document = System.Text.Json.JsonDocument.Parse(body);
        Assert.Equal("token+exact", document.RootElement.GetProperty("token").GetString());
    }
}
