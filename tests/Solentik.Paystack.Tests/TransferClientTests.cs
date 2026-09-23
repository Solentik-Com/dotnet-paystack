using Microsoft.Extensions.Options;
using Solentik.Paystack.Transfers;
using Solentik.Paystack.Transfers.Models;

namespace Solentik.Paystack.Tests;

public sealed class TransferClientTests
{
    [Fact]
    public async Task SupportsCompleteTransferEndpointSurface()
    {
        const string initiateResponse = """
            {"status":true,"message":"Transfer has been queued","data":{"domain":"test","amount":100000,"currency":"NGN","reference":"acv_9ee55786","source":"balance","source_details":null,"reason":"Bonus for the week","status":"success","failures":null,"transfer_code":"TRF_v5tip3zx8nna9o78","titan_code":null,"transferred_at":null,"id":860703114,"recipient":56824902,"createdAt":"2025-08-04T10:32:40.000Z","updatedAt":"2025-08-04T10:32:40.000Z"}}
            """;
        const string finalizeResponse = """
            {"status":true,"message":"Transfer has been queued","data":{"domain":"test","amount":1000000,"currency":"NGN","reference":"n7ll9pzl6b","source":"balance","reason":"E go better for you","status":"success","transfer_code":"TRF_zuirlnr9qblgfko","id":529410,"recipient":225204,"createdAt":"2018-08-02T10:02:55.000Z","updatedAt":"2018-08-02T10:12:05.000Z"}}
            """;
        const string bulkResponse = """
            {"status":true,"message":"3 transfers queued.","data":[{"reference":"acv_2627bbfe","recipient":"RCP_gd9vgag7n5lr5ix","amount":20000,"transfer_code":"TRF_o0mv5dc2lv4t2wdb","currency":"NGN","status":"success"}]}
            """;
        const string listResponse = """
            {"status":true,"message":"Transfers retrieved","data":[{"amount":4400,"currency":"NGN","status":"otp","transfer_code":"TRF_1ptvuv321ahaa7q","id":14,"recipient":{"recipient_code":"RCP_2x5j67tnnw1t98k","name":"Flesh"},"createdAt":"2017-02-03T17:21:54.000Z","updatedAt":"2017-02-03T17:21:54.000Z"}]}
            """;
        const string fetchResponse = """
            {"status":true,"message":"Transfer retrieved","data":{"amount":20000,"createdAt":"2024-02-01T08:32:21.000Z","updatedAt":"2024-02-01T08:34:07.000Z","currency":"NGN","domain":"test","id":451930323,"reference":"ge-bzrf8u8k2pygxrnqf","status":"success","transfer_code":"TRF_fpmd0l8uta8upow7","fee_charged":0,"recipient":{"recipient_code":"RCP_rjs1szi4ax5hoeo"}}}
            """;
        var handler = new RecordingHttpMessageHandler(initiateResponse, finalizeResponse, bulkResponse, listResponse, fetchResponse, fetchResponse);
        var client = new TransferClient(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));

        var cancellationToken = TestContext.Current.CancellationToken;
        var initiated = await client.InitiateAsync(
            new InitiateTransferRequest { Amount = 10000, Recipient = "RCP_recipient_code", Reason = "Monthly Salary" },
            cancellationToken);
        var finalized = await client.FinalizeAsync("TRF_12345", "123456", cancellationToken);
        var bulk = await client.BulkInitiateAsync(
            new BulkTransferRequest
            {
                Transfers = [new BulkTransferItem { Amount = 5000, Recipient = "RCP_001" }]
            },
            cancellationToken);
        var listed = await client.ListAsync(new TransferListOptions { PerPage = 2, Recipient = 56824902 }, cancellationToken);
        var fetched = await client.FetchAsync("TRF_12345", cancellationToken);
        await client.VerifyAsync("ref_unique_123", cancellationToken);

        Assert.Equal("TRF_v5tip3zx8nna9o78", initiated.Data?.TransferCode);
        Assert.Equal(56824902, initiated.Data?.Recipient?.GetInt64());
        Assert.Equal(new DateTimeOffset(2025, 8, 4, 10, 32, 40, TimeSpan.Zero), initiated.Data?.CreatedAt);
        Assert.Equal("TRF_zuirlnr9qblgfko", finalized.Data?.TransferCode);
        var bulkResult = Assert.Single(bulk.Data!);
        Assert.Equal("RCP_gd9vgag7n5lr5ix", bulkResult.Recipient);
        Assert.Equal("RCP_2x5j67tnnw1t98k", Assert.Single(listed.Data!).Recipient?.GetProperty("recipient_code").GetString());
        Assert.Equal(0, fetched.Data?.FeeCharged);
        Assert.Collection(handler.Requests,
            request => { AssertRoute(request, HttpMethod.Post, "/transfer"); Assert.Contains("\"source\":\"balance\"", request.Body); },
            request => { AssertRoute(request, HttpMethod.Post, "/transfer/finalize_transfer"); Assert.Contains("\"transfer_code\":\"TRF_12345\"", request.Body); Assert.Contains("\"otp\":\"123456\"", request.Body); },
            request => { AssertRoute(request, HttpMethod.Post, "/transfer/bulk"); Assert.Contains("\"currency\":\"NGN\"", request.Body); },
            request => { AssertRoute(request, HttpMethod.Get, "/transfer"); Assert.Contains("perPage=2", request.Uri.Query); Assert.Contains("recipient=56824902", request.Uri.Query); },
            request => AssertRoute(request, HttpMethod.Get, "/transfer/TRF_12345"),
            request => AssertRoute(request, HttpMethod.Get, "/transfer/verify/ref_unique_123"));
    }

    [Fact]
    public async Task InitiateAsync_RejectsMissingRecipient()
    {
        var client = new TransferClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentException>(() => client.InitiateAsync(
            new InitiateTransferRequest { Amount = 10000, Recipient = "   " },
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task InitiateAsync_RejectsNonPositiveAmount()
    {
        var client = new TransferClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.InitiateAsync(
            new InitiateTransferRequest { Amount = 0, Recipient = "RCP_recipient_code" },
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task BulkInitiateAsync_RejectsEmptyTransferList()
    {
        var client = new TransferClient(new HttpClient(new RecordingHttpMessageHandler()), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
        await Assert.ThrowsAsync<ArgumentException>(() => client.BulkInitiateAsync(
            new BulkTransferRequest { Transfers = [] },
            TestContext.Current.CancellationToken));
    }

    private static void AssertRoute(RecordedRequest request, HttpMethod method, string path)
    {
        Assert.Equal(method, request.Method);
        Assert.Equal(path, request.Uri.AbsolutePath);
    }
}
