using Microsoft.Extensions.Options;
using Solentik.Paystack.TransactionSplits;
using Solentik.Paystack.TransactionSplits.Models;

namespace Solentik.Paystack.Tests;

public sealed class TransactionSplitClientTests
{
    [Fact]
    public async Task SupportsCompleteTransactionSplitEndpointSurface()
    {
        const string split = "{\"status\":true,\"data\":{\"id\":123,\"name\":\"Test Split\",\"split_code\":\"SPL_1\"}}";
        var handler = new RecordingHttpMessageHandler(split, "{\"status\":true,\"data\":[]}", split, split, split, split);
        var client = new TransactionSplitClient(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));

        var cancellationToken = TestContext.Current.CancellationToken;
        var created = await client.CreateAsync(
            new CreateTransactionSplitRequest
            {
                Name = "Test Split",
                Type = "percentage",
                Currency = "NGN",
                Subaccounts = [new SplitSubaccountRequest { Subaccount = "ACCT_1", Share = 20 }]
            },
            cancellationToken);
        await client.ListAsync(new TransactionSplitListOptions { Active = true, SortBy = "name", PerPage = 10 }, cancellationToken);
        await client.FetchAsync("123/abc", cancellationToken);
        await client.UpdateAsync("123", new UpdateTransactionSplitRequest { Name = "Updated", Active = true }, cancellationToken);
        await client.AddSubaccountAsync("123", new SplitSubaccountRequest { Subaccount = "ACCT_2", Share = 30 }, cancellationToken);
        await client.RemoveSubaccountAsync("123", "ACCT_1", cancellationToken);

        Assert.Equal("SPL_1", created.Data?.SplitCode);
        Assert.Collection(handler.Requests,
            request => { AssertRoute(request, HttpMethod.Post, "/split"); Assert.Contains("\"share\":20", request.Body); },
            request => { AssertRoute(request, HttpMethod.Get, "/split"); Assert.Contains("active=true", request.Uri.Query); Assert.Contains("sort_by=name", request.Uri.Query); },
            request => AssertRoute(request, HttpMethod.Get, "/split/123%2Fabc"),
            request => { AssertRoute(request, HttpMethod.Put, "/split/123"); Assert.Contains("\"active\":true", request.Body); },
            request => { AssertRoute(request, HttpMethod.Post, "/split/123/subaccount/add"); Assert.Contains("ACCT_2", request.Body); },
            request => { AssertRoute(request, HttpMethod.Post, "/split/123/subaccount/remove"); Assert.Contains("ACCT_1", request.Body); });
    }

    [Fact]
    public async Task CreateAsync_RejectsEmptySubaccounts()
    {
        var client = new TransactionSplitClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentException>(() => client.CreateAsync(
            new CreateTransactionSplitRequest
            {
                Name = "Invalid", Type = "percentage", Currency = "NGN", Subaccounts = []
            },
            TestContext.Current.CancellationToken));
    }

    private static void AssertRoute(RecordedRequest request, HttpMethod method, string path)
    {
        Assert.Equal(method, request.Method);
        Assert.Equal(path, request.Uri.AbsolutePath);
    }
}
