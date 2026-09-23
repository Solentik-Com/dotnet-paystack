using Microsoft.Extensions.Options;
using Solentik.Paystack.Transfers;

namespace Solentik.Paystack.Tests;

public sealed class TransferControlClientTests
{
    [Fact]
    public async Task SupportsCompleteTransferControlEndpointSurface()
    {
        const string ledgerResponse = """
            {"status":true,"message":"Balance ledger retrieved","data":[{"integration":463433,"domain":"test","balance":2078224968,"currency":"NGN","difference":-50000,"reason":"Who dey breet?","model_responsible":"Transfer","model_row":56610600,"id":149411613,"createdAt":"2021-04-08T09:39:49.000Z","updatedAt":"2021-04-08T09:39:49.000Z"}]}
            """;
        var handler = new RecordingHttpMessageHandler(
            "{\"status\":true,\"data\":[{\"currency\":\"NGN\",\"balance\":10000}]}",
            ledgerResponse,
            "{\"status\":true,\"message\":\"OTP has been resent\"}",
            "{\"status\":true,\"message\":\"OTP has been sent to mobile number ending with 4321\"}",
            "{\"status\":true,\"message\":\"OTP requirement for transfers has been disabled\"}",
            "{\"status\":true,\"message\":\"OTP requirement for transfers has been enabled\"}");
        var client = new TransferControlClient(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));

        var cancellationToken = TestContext.Current.CancellationToken;
        var balance = await client.CheckBalanceAsync(cancellationToken);
        var ledger = await client.FetchLedgerAsync(cancellationToken: cancellationToken);
        await client.ResendOtpAsync("TRF_12345", cancellationToken: cancellationToken);
        await client.RequestDisableOtpAsync(cancellationToken);
        await client.FinalizeDisableOtpAsync("123456", cancellationToken);
        await client.EnableOtpAsync(cancellationToken);

        Assert.Equal(10000, balance.Data?[0].Balance);
        var entry = Assert.Single(ledger.Data!);
        Assert.Equal("Transfer", entry.ModelResponsible);
        Assert.Equal(-50000, entry.Difference);
        Assert.Collection(handler.Requests,
            request => AssertRoute(request, HttpMethod.Get, "/balance"),
            request => AssertRoute(request, HttpMethod.Get, "/balance/ledger"),
            request => { AssertRoute(request, HttpMethod.Post, "/transfer/resend_otp"); Assert.Contains("\"reason\":\"resend_otp\"", request.Body); },
            request => AssertRoute(request, HttpMethod.Post, "/transfer/disable_otp"),
            request => { AssertRoute(request, HttpMethod.Post, "/transfer/disable_otp_finalize"); Assert.Contains("\"otp\":\"123456\"", request.Body); },
            request => AssertRoute(request, HttpMethod.Post, "/transfer/enable_otp"));
    }

    private static void AssertRoute(RecordedRequest request, HttpMethod method, string path)
    {
        Assert.Equal(method, request.Method);
        Assert.Equal(path, request.Uri.AbsolutePath);
    }
}
