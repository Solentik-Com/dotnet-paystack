using Microsoft.Extensions.Options;
using Solentik.Paystack.Plans;
using Solentik.Paystack.Plans.Models;

namespace Solentik.Paystack.Tests;

public sealed class PlanClientTests
{
    [Fact]
    public async Task SupportsCompletePlanEndpointSurface()
    {
        const string plan = "{\"status\":true,\"data\":{\"plan_code\":\"PLN_1\",\"name\":\"Pro\",\"amount\":5000,\"interval\":\"monthly\"}}";
        var handler = new RecordingHttpMessageHandler(plan, "{\"status\":true,\"data\":[]}", plan, plan);
        var client = new PlanClient(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));

        var cancellationToken = TestContext.Current.CancellationToken;
        var created = await client.CreateAsync(new CreatePlanRequest { Name = "Pro", Amount = 5000, Interval = "monthly" }, cancellationToken);
        await client.ListAsync(new PlanListOptions { Interval = "monthly", Amount = 5000 }, cancellationToken);
        await client.FetchAsync("PLN/1", cancellationToken);
        await client.UpdateAsync("PLN_1", new UpdatePlanRequest { Amount = 6000, UpdateExistingSubscriptions = true }, cancellationToken);

        Assert.Equal("PLN_1", created.Data?.PlanCode);
        Assert.Collection(handler.Requests,
            request => { Assert.Equal(HttpMethod.Post, request.Method); Assert.Equal("/plan", request.Uri.AbsolutePath); Assert.Contains("\"amount\":5000", request.Body); },
            request => { Assert.Equal(HttpMethod.Get, request.Method); Assert.Contains("interval=monthly", request.Uri.Query); Assert.Contains("amount=5000", request.Uri.Query); },
            request => Assert.Equal("/plan/PLN%2F1", request.Uri.AbsolutePath),
            request => { Assert.Equal(HttpMethod.Put, request.Method); Assert.Equal("/plan/PLN_1", request.Uri.AbsolutePath); Assert.Contains("\"update_existing_subscriptions\":true", request.Body); });
    }

    [Fact]
    public async Task CreateAsync_RejectsNonPositiveAmount()
    {
        var client = new PlanClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.CreateAsync(
            new CreatePlanRequest { Name = "Bad", Amount = 0, Interval = "monthly" },
            TestContext.Current.CancellationToken));
    }
}
