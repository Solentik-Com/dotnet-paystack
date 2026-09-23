using Microsoft.Extensions.Options;
using Solentik.Paystack.Transfers;
using Solentik.Paystack.Transfers.Models;

namespace Solentik.Paystack.Tests;

public sealed class TransferRecipientClientTests
{
    [Fact]
    public async Task SupportsCompleteTransferRecipientEndpointSurface()
    {
        const string recipient = """
            {"status":true,"data":{"id":1,"recipient_code":"RCP_12345","name":"Stephen Asare","type":"nuban","details":{"account_number":"0123456789","account_name":null,"bank_code":"058","bank_name":"GTBank"},"createdAt":"2020-11-09T10:12:48.213Z","updatedAt":"2020-11-09T10:12:48.213Z"}}
            """;
        const string bulkCreateResponse = """
            {"status":true,"message":"Recipients added successfully","data":{"success":[{"domain":"test","name":"Habenero Mundane","type":"nuban","recipient_code":"RCP_wh5k8r4vzuh5c94","active":true,"id":10152540,"details":{"account_number":"0123456789","account_name":null,"bank_code":"033","bank_name":"United Bank For Africa"},"createdAt":"2020-11-09T10:12:48.213Z","updatedAt":"2020-11-09T10:12:48.213Z"}],"errors":[]}}
            """;
        var handler = new RecordingHttpMessageHandler(
            recipient, bulkCreateResponse,
            "{\"status\":true,\"data\":[]}", recipient, recipient, "{\"status\":true,\"message\":\"Transfer recipient set as inactive\"}");
        var client = new TransferRecipientClient(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));

        var cancellationToken = TestContext.Current.CancellationToken;
        var created = await client.CreateAsync(
            new CreateTransferRecipientRequest
            {
                Type = "nuban",
                Name = "Stephen Asare",
                AccountNumber = "0123456789",
                BankCode = "058",
                Currency = "NGN"
            },
            cancellationToken);
        var bulkCreated = await client.BulkCreateAsync(
            [new CreateTransferRecipientRequest { Type = "nuban", Name = "Princess Yankson", AccountNumber = "9876543210", BankCode = "011" }],
            cancellationToken);
        await client.ListAsync(new TransferRecipientListOptions { PerPage = 2 }, cancellationToken);
        await client.FetchAsync("RCP_12345", cancellationToken);
        await client.UpdateAsync("RCP_12345", new UpdateTransferRecipientRequest { Name = "Stephen" }, cancellationToken);
        await client.DeleteAsync("RCP_12345", cancellationToken);

        Assert.Equal("RCP_12345", created.Data?.RecipientCode);
        Assert.Equal("058", created.Data?.Details?.BankCode);
        var bulkSuccess = Assert.Single(bulkCreated.Data!.Success);
        Assert.Equal("RCP_wh5k8r4vzuh5c94", bulkSuccess.RecipientCode);
        Assert.Empty(bulkCreated.Data.Errors);
        Assert.Collection(handler.Requests,
            request => AssertRoute(request, HttpMethod.Post, "/transferrecipient"),
            request => { AssertRoute(request, HttpMethod.Post, "/transferrecipient/bulk"); Assert.Contains("\"batch\":", request.Body); },
            request => { AssertRoute(request, HttpMethod.Get, "/transferrecipient"); Assert.Contains("perPage=2", request.Uri.Query); },
            request => AssertRoute(request, HttpMethod.Get, "/transferrecipient/RCP_12345"),
            request => { AssertRoute(request, HttpMethod.Put, "/transferrecipient/RCP_12345"); Assert.Contains("\"name\":\"Stephen\"", request.Body); },
            request => AssertRoute(request, HttpMethod.Delete, "/transferrecipient/RCP_12345"));
    }

    [Fact]
    public async Task CreateAsync_RejectsMissingType()
    {
        var client = new TransferRecipientClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentException>(() => client.CreateAsync(
            new CreateTransferRecipientRequest { Type = "   ", Name = "Stephen Asare" },
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task BulkCreateAsync_RejectsEmptyRecipientList()
    {
        var client = new TransferRecipientClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentException>(() => client.BulkCreateAsync([], TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateAsync_RejectsMissingAccountNumberAndBankCodeWhenNotAuthorizationBased()
    {
        var client = new TransferRecipientClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentException>(() => client.CreateAsync(
            new CreateTransferRecipientRequest { Type = "nuban", Name = "Stephen Asare" },
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateAsync_AllowsAuthorizationBasedRecipientWithoutAccountNumberOrBankCode()
    {
        var handler = new RecordingHttpMessageHandler(
            "{\"status\":true,\"data\":{\"id\":1,\"recipient_code\":\"RCP_auth\",\"type\":\"authorization\"}}");
        var client = new TransferRecipientClient(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));

        var created = await client.CreateAsync(
            new CreateTransferRecipientRequest
            {
                Type = "authorization",
                Name = "Stephen Asare",
                AuthorizationCode = "AUTH_xxxx"
            },
            TestContext.Current.CancellationToken);

        Assert.Equal("RCP_auth", created.Data?.RecipientCode);
    }

    private static void AssertRoute(RecordedRequest request, HttpMethod method, string path)
    {
        Assert.Equal(method, request.Method);
        Assert.Equal(path, request.Uri.AbsolutePath);
    }
}
