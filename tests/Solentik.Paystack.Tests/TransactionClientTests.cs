using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Transactions;
using Solentik.Paystack.Transactions.Models;

namespace Solentik.Paystack.Tests;

public sealed class TransactionClientTests
{
    [Fact]
    public async Task InitializeAsync_SendsAuthenticatedRequestAndDeserializesResponse()
    {
        var handler = new RecordingHandler(HttpStatusCode.OK, """
            {"status":true,"message":"Authorization URL created","data":{"authorization_url":"https://checkout.test","access_code":"access","reference":"ref-1"}}
            """);
        var client = CreateClient(handler);

        var response = await client.InitializeAsync(
            new InitializeTransactionRequest
            {
                Email = "buyer@solentik.com",
                Amount = 5000,
                Currency = "GHS"
            },
            TestContext.Current.CancellationToken);

        Assert.True(response.Status);
        Assert.Equal("ref-1", response.Data?.Reference);
        Assert.Equal(HttpMethod.Post, handler.Method);
        Assert.Equal("https://api.paystack.co/transaction/initialize", handler.RequestUri?.ToString());
        Assert.Equal("Bearer", handler.AuthorizationScheme);
        Assert.Equal("sk_test_secret", handler.AuthorizationParameter);
        Assert.Contains("\"email\":\"buyer@solentik.com\"", handler.Body);
        Assert.Contains("\"amount\":5000", handler.Body);
    }

    [Fact]
    public async Task VerifyAsync_EscapesReferenceAndDeserializesTransaction()
    {
        var handler = new RecordingHandler(HttpStatusCode.OK, """
            {"status":true,"message":"Verification successful","data":{"id":42,"status":"success","reference":"order/42","amount":1200,"currency":"GHS","gateway_response":"Successful"}}
            """);
        var client = CreateClient(handler);

        var response = await client.VerifyAsync("order/42", TestContext.Current.CancellationToken);

        Assert.Equal(42, response.Data?.Id);
        Assert.Equal("Successful", response.Data?.AdditionalData?["gateway_response"].GetString());
        Assert.Equal("https://api.paystack.co/transaction/verify/order%2F42", handler.RequestUri?.ToString());
    }

    [Fact]
    public async Task ListAsync_AddsOnlyProvidedFilters()
    {
        var handler = new RecordingHandler(HttpStatusCode.OK, """
            {"status":true,"message":"Transactions retrieved","data":[]}
            """);
        var client = CreateClient(handler);

        await client.ListAsync(
            new TransactionListOptions
            {
                PerPage = 25,
                Page = 2,
                Customer = "CUS test",
                Status = "success"
            },
            TestContext.Current.CancellationToken);

        var uri = handler.RequestUri?.ToString();
        Assert.Contains("perPage=25", uri);
        Assert.Contains("page=2", uri);
        Assert.Contains("customer=CUS test", Uri.UnescapeDataString(uri!));
        Assert.Contains("status=success", uri);
    }

    [Fact]
    public async Task ApiFailure_ThrowsStructuredPaystackException()
    {
        var handler = new RecordingHandler(HttpStatusCode.BadRequest, """
            {"status":false,"message":"Invalid key","type":"validation_error","code":"invalid_key","meta":{"nextStep":"Check key"}}
            """);
        var client = CreateClient(handler);

        var exception = await Assert.ThrowsAsync<PaystackException>(
            () => client.FetchAsync(42, TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal("Invalid key", exception.Message);
        Assert.Equal("validation_error", exception.Type);
        Assert.Equal("invalid_key", exception.PaystackCode);
        Assert.Equal("Check key", exception.Metadata?.GetProperty("nextStep").GetString());
    }

    [Fact]
    public async Task ApiFailure_WithNonJsonBody_FallsBackToReasonPhrase()
    {
        var handler = new RecordingHandler(HttpStatusCode.BadGateway, "Bad Gateway");
        var client = CreateClient(handler);

        var exception = await Assert.ThrowsAsync<PaystackException>(
            () => client.FetchAsync(42, TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.BadGateway, exception.StatusCode);
        Assert.Equal("Bad Gateway", exception.Message);
        Assert.Equal("api_error", exception.Type);
        Assert.Null(exception.PaystackCode);
    }

    private static TransactionClient CreateClient(RecordingHandler handler) =>
        new(new HttpClient(handler), Options.Create(new PaystackOptions
        {
            SecretKey = "sk_test_secret",
            BaseAddress = new Uri("https://api.paystack.co/"),
            Timeout = TimeSpan.FromSeconds(10)
        }));

    private sealed class RecordingHandler(HttpStatusCode statusCode, string responseBody) : HttpMessageHandler
    {
        public HttpMethod? Method { get; private set; }
        public Uri? RequestUri { get; private set; }
        public string? AuthorizationScheme { get; private set; }
        public string? AuthorizationParameter { get; private set; }
        public string Body { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Method = request.Method;
            RequestUri = request.RequestUri;
            AuthorizationScheme = request.Headers.Authorization?.Scheme;
            AuthorizationParameter = request.Headers.Authorization?.Parameter;
            Body = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        }
    }
}
