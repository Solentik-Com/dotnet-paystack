using Microsoft.Extensions.Options;
using Solentik.Paystack.PaymentRequests;
using Solentik.Paystack.PaymentRequests.Models;

namespace Solentik.Paystack.Tests;

public sealed class PaymentRequestClientTests
{
    [Fact]
    public async Task SupportsCompletePaymentRequestEndpointSurface()
    {
        const string paymentRequest = "{\"status\":true,\"data\":{\"id\":123,\"request_code\":\"PRQ_1\",\"amount\":50000}}";
        const string totals = "{\"status\":true,\"data\":{\"pending\":[],\"successful\":[]}}";
        var handler = new RecordingHttpMessageHandler(
            paymentRequest,
            "{\"status\":true,\"data\":[]}",
            paymentRequest,
            paymentRequest,
            "{\"status\":true,\"data\":{}}",
            totals,
            paymentRequest,
            paymentRequest,
            "{\"status\":true,\"data\":{}}");
        var client = new PaymentRequestClient(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));

        var cancellationToken = TestContext.Current.CancellationToken;
        var created = await client.CreateAsync(
            new CreatePaymentRequestRequest
            {
                Customer = "CUS_xxxx",
                Amount = 50000,
                Description = "Website design",
                DueDate = new DateTimeOffset(2026, 12, 20, 0, 0, 0, TimeSpan.Zero)
            },
            cancellationToken);
        await client.ListAsync(new PaymentRequestListOptions { PerPage = 10, Page = 1, IncludeArchive = true }, cancellationToken);
        await client.FetchAsync("PRQ_1", cancellationToken);
        await client.VerifyAsync("PRQ_1", cancellationToken);
        await client.NotifyAsync("PRQ_1", cancellationToken);
        await client.GetTotalsAsync(cancellationToken);
        await client.FinalizeAsync("PRQ_1", cancellationToken: cancellationToken);
        await client.UpdateAsync("PRQ_1", new UpdatePaymentRequestRequest { Description = "Website design (updated)" }, cancellationToken);
        await client.ArchiveAsync("PRQ_1", cancellationToken);

        Assert.Equal("PRQ_1", created.Data?.RequestCode);
        Assert.Collection(handler.Requests,
            request => { AssertRoute(request, HttpMethod.Post, "/paymentrequest"); Assert.Contains("\"amount\":50000", request.Body); },
            request => { AssertRoute(request, HttpMethod.Get, "/paymentrequest"); Assert.Contains("perPage=10", request.Uri.Query); Assert.Contains("include_archive=true", request.Uri.Query); },
            request => AssertRoute(request, HttpMethod.Get, "/paymentrequest/PRQ_1"),
            request => AssertRoute(request, HttpMethod.Get, "/paymentrequest/verify/PRQ_1"),
            request => AssertRoute(request, HttpMethod.Post, "/paymentrequest/notify/PRQ_1"),
            request => AssertRoute(request, HttpMethod.Get, "/paymentrequest/totals"),
            request => { AssertRoute(request, HttpMethod.Post, "/paymentrequest/finalize/PRQ_1"); Assert.Contains("\"send_notification\":true", request.Body); },
            request => { AssertRoute(request, HttpMethod.Put, "/paymentrequest/PRQ_1"); Assert.Contains("Website design (updated)", request.Body); },
            request => AssertRoute(request, HttpMethod.Post, "/paymentrequest/archive/PRQ_1"));
    }

    [Fact]
    public async Task CreateAsync_RejectsMissingCustomer()
    {
        var client = new PaymentRequestClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentException>(() => client.CreateAsync(
            new CreatePaymentRequestRequest { Customer = "   ", Amount = 50000 },
            TestContext.Current.CancellationToken));
    }

    private static void AssertRoute(RecordedRequest request, HttpMethod method, string path)
    {
        Assert.Equal(method, request.Method);
        Assert.Equal(path, request.Uri.AbsolutePath);
    }
}
