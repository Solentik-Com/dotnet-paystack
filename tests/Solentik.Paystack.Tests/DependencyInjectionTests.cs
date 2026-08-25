using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Solentik.Paystack.DependencyInjection;
using Solentik.Paystack.Transactions;
using Solentik.Paystack.Webhooks;

namespace Solentik.Paystack.Tests;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddPaystack_RegistersPublicServices()
    {
        var services = new ServiceCollection();
        services.AddPaystack(options => options.SecretKey = "sk_test_secret");

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IPaystackClient>();

        Assert.NotNull(client);
        Assert.IsAssignableFrom<ITransactionClient>(client.Transactions);
        Assert.NotNull(client.Customers);
        Assert.NotNull(client.Plans);
        Assert.NotNull(client.Subscriptions);
        Assert.NotNull(client.TransactionSplits);
        Assert.NotNull(client.Miscellaneous);
        Assert.NotNull(client.Verification);
        Assert.NotSame(client, provider.GetRequiredService<IPaystackClient>());
        Assert.NotNull(provider.GetRequiredService<IPaystackWebhookVerifier>());
    }

    [Fact]
    public async Task AddPaystack_SharesOneNamedHttpClientAcrossAllResourceClients()
    {
        var handler = new CountingHandler();
        var services = new ServiceCollection();
        services.AddPaystack(options => options.SecretKey = "sk_test_secret");
        services.AddHttpClient(PaystackServiceCollectionExtensions.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IPaystackClient>();

        await client.Transactions.FetchAsync(1, TestContext.Current.CancellationToken);
        await client.Customers.FetchAsync("cus_1", TestContext.Current.CancellationToken);

        Assert.Equal(2, handler.RequestCount);
    }

    private sealed class CountingHandler : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"status":true,"message":"ok","data":{}}""")
            });
        }
    }
}
