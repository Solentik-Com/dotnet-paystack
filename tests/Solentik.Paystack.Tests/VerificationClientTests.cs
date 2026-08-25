using Microsoft.Extensions.Options;
using Solentik.Paystack.Verification;
using Solentik.Paystack.Verification.Models;

namespace Solentik.Paystack.Tests;

public sealed class VerificationClientTests
{
    [Fact]
    public async Task ResolveAccountAsync_SendsAccountNumberAndBankCode()
    {
        const string resolved = "{\"status\":true,\"data\":{\"account_number\":\"0123456789\",\"account_name\":\"STEPHEN ASARE\",\"bank_id\":9}}";
        var handler = new RecordingHttpMessageHandler(resolved);
        var client = CreateClient(handler);

        var response = await client.ResolveAccountAsync("0123456789", "058", TestContext.Current.CancellationToken);

        Assert.Equal("STEPHEN ASARE", response.Data?.AccountName);
        var uri = handler.Requests[0].Uri;
        Assert.Equal("/bank/resolve", uri.AbsolutePath);
        Assert.Contains("account_number=0123456789", uri.Query);
        Assert.Contains("bank_code=058", uri.Query);
    }

    [Fact]
    public async Task ResolveAccountAsync_RequiresAccountNumberAndBankCode()
    {
        var client = CreateClient(new RecordingHttpMessageHandler());
        await Assert.ThrowsAsync<ArgumentException>(() => client.ResolveAccountAsync("", "058", TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentException>(() => client.ResolveAccountAsync("0123456789", "", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ValidateAccountAsync_PostsPayload()
    {
        var handler = new RecordingHttpMessageHandler("{\"status\":true,\"message\":\"Personal Details Validated\"}");
        var client = CreateClient(handler);

        await client.ValidateAccountAsync(
            new ValidateAccountRequest
            {
                AccountName = "Stephen Asare",
                AccountNumber = "0123456789",
                AccountType = "personal",
                BankCode = "632005",
                CountryCode = "ZA",
                DocumentType = "identityNumber",
                DocumentNumber = "1234567890123"
            },
            TestContext.Current.CancellationToken);

        var request = handler.Requests[0];
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal("/bank/validate", request.Uri.AbsolutePath);
        Assert.Contains("\"account_name\":\"Stephen Asare\"", request.Body);
        Assert.Contains("\"document_type\":\"identityNumber\"", request.Body);
    }

    [Fact]
    public async Task ResolveCardBinAsync_RequestsBinEndpoint()
    {
        const string bin = "{\"status\":true,\"data\":{\"bin\":\"539983\",\"brand\":\"Mastercard\",\"card_type\":\"DEBIT\",\"bank\":\"Guaranty Trust Bank\"}}";
        var handler = new RecordingHttpMessageHandler(bin);
        var client = CreateClient(handler);

        var response = await client.ResolveCardBinAsync("539983", TestContext.Current.CancellationToken);

        Assert.Equal("Mastercard", response.Data?.Brand);
        Assert.Equal("/decision/bin/539983", handler.Requests[0].Uri.AbsolutePath);
    }

    private static VerificationClient CreateClient(RecordingHttpMessageHandler handler) =>
        new(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
}
