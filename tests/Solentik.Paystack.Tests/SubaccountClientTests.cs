using Microsoft.Extensions.Options;
using Solentik.Paystack.Subaccounts;
using Solentik.Paystack.Subaccounts.Models;

namespace Solentik.Paystack.Tests;

public sealed class SubaccountClientTests
{
    [Fact]
    public async Task SupportsCompleteSubaccountEndpointSurface()
    {
        const string subaccount = "{\"status\":true,\"data\":{\"id\":123,\"business_name\":\"Sunshine Studios\",\"subaccount_code\":\"ACCT_1\"}}";
        var handler = new RecordingHttpMessageHandler(subaccount, "{\"status\":true,\"data\":[]}", subaccount, subaccount);
        var client = new SubaccountClient(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));

        var cancellationToken = TestContext.Current.CancellationToken;
        var created = await client.CreateAsync(
            new CreateSubaccountRequest
            {
                BusinessName = "Sunshine Studios",
                SettlementBank = "058",
                AccountNumber = "0123456047",
                PercentageCharge = 18.2m,
                PrimaryContactEmail = "dafe@sunshinestudios.com"
            },
            cancellationToken);
        await client.ListAsync(new SubaccountListOptions { PerPage = 10, Page = 1 }, cancellationToken);
        await client.FetchAsync("ACCT_1", cancellationToken);
        await client.UpdateAsync("ACCT_1", new UpdateSubaccountRequest { BusinessName = "Sunshine Studios (Updated)", Active = true }, cancellationToken);

        Assert.Equal("ACCT_1", created.Data?.SubaccountCode);
        Assert.Collection(handler.Requests,
            request => { AssertRoute(request, HttpMethod.Post, "/subaccount"); Assert.Contains("\"percentage_charge\":18.2", request.Body); },
            request => { AssertRoute(request, HttpMethod.Get, "/subaccount"); Assert.Contains("perPage=10", request.Uri.Query); Assert.Contains("page=1", request.Uri.Query); },
            request => AssertRoute(request, HttpMethod.Get, "/subaccount/ACCT_1"),
            request => { AssertRoute(request, HttpMethod.Put, "/subaccount/ACCT_1"); Assert.Contains("\"active\":true", request.Body); });
    }

    [Fact]
    public async Task CreateAsync_RejectsMissingBusinessName()
    {
        var client = new SubaccountClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentException>(() => client.CreateAsync(
            new CreateSubaccountRequest
            {
                BusinessName = "   ",
                SettlementBank = "058",
                AccountNumber = "0123456047",
                PercentageCharge = 18.2m
            },
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateAsync_RejectsNonPositivePercentageCharge()
    {
        var client = new SubaccountClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.CreateAsync(
            new CreateSubaccountRequest
            {
                BusinessName = "Sunshine Studios",
                SettlementBank = "058",
                AccountNumber = "0123456047",
                PercentageCharge = 0
            },
            TestContext.Current.CancellationToken));
    }

    private static void AssertRoute(RecordedRequest request, HttpMethod method, string path)
    {
        Assert.Equal(method, request.Method);
        Assert.Equal(path, request.Uri.AbsolutePath);
    }
}
