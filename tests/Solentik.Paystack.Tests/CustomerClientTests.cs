using Microsoft.Extensions.Options;
using Solentik.Paystack.Customers;
using Solentik.Paystack.Customers.Models;

namespace Solentik.Paystack.Tests;

public sealed class CustomerClientTests
{
    [Fact]
    public async Task SupportsCompleteCustomerEndpointSurface()
    {
        var handler = new RecordingHttpMessageHandler(
            CustomerResponse, CustomerResponse, CustomerResponse,
            "{\"status\":true,\"data\":[]}",
            EmptyResponse, CustomerResponse, EmptyResponse,
            "{\"status\":true,\"data\":{\"access_code\":\"access-1\",\"reference\":\"ref-1\"}}",
            "{\"status\":true,\"data\":{\"status\":\"active\",\"reference\":\"ref-1\"}}");
        var client = CreateClient(handler);

        var cancellationToken = TestContext.Current.CancellationToken;
        var created = await client.CreateAsync(new CreateCustomerRequest { Email = "buyer@solentik.com" }, cancellationToken);
        await client.FetchAsync("buyer+test@solentik.com", cancellationToken);
        await client.UpdateAsync("CUS_1", new UpdateCustomerRequest { FirstName = "Ada" }, cancellationToken);
        await client.ListAsync(new CustomerListOptions { PerPage = 20, Page = 2 }, cancellationToken);
        await client.ValidateIdentityAsync("CUS_1", new ValidateCustomerIdentityRequest { Type = "bank_account", Country = "GH", AccountNumber = "123" }, cancellationToken);
        await client.SetRiskActionAsync(new SetCustomerRiskActionRequest { Customer = "CUS_1", RiskAction = "allow" }, cancellationToken);
        await client.DeactivateAuthorizationAsync("AUTH_1", cancellationToken);
        var initialized = await client.InitializeAuthorizationAsync(new InitializeAuthorizationRequest { Email = "buyer@solentik.com", Channel = "direct_debit" }, cancellationToken);
        await client.VerifyAuthorizationAsync("ref/1", cancellationToken);

        Assert.Equal("CUS_1", created.Data?.CustomerCode);
        Assert.Equal("access-1", initialized.Data?.AccessCode);
        Assert.Collection(handler.Requests,
            request => AssertRoute(request, HttpMethod.Post, "/customer"),
            request => AssertRoute(request, HttpMethod.Get, "/customer/buyer%2Btest%40solentik.com"),
            request => AssertRoute(request, HttpMethod.Put, "/customer/CUS_1"),
            request => { AssertRoute(request, HttpMethod.Get, "/customer"); Assert.Contains("perPage=20", request.Uri.Query); },
            request => AssertRoute(request, HttpMethod.Post, "/customer/CUS_1/identification"),
            request => AssertRoute(request, HttpMethod.Post, "/customer/set_risk_action"),
            request => { AssertRoute(request, HttpMethod.Post, "/customer/authorization/deactivate"); Assert.Contains("AUTH_1", request.Body); },
            request => AssertRoute(request, HttpMethod.Post, "/customer/authorization/initialize"),
            request => AssertRoute(request, HttpMethod.Get, "/customer/authorization/verify/ref%2F1"));
    }

    private static CustomerClient CreateClient(HttpMessageHandler handler) =>
        new(new HttpClient(handler), Options.Create(TestOptions));

    private static void AssertRoute(RecordedRequest request, HttpMethod method, string path)
    {
        Assert.Equal(method, request.Method);
        Assert.Equal(path, request.Uri.AbsolutePath);
    }

    private static readonly PaystackOptions TestOptions = new() { SecretKey = "sk_test" };
    private const string CustomerResponse = "{\"status\":true,\"data\":{\"id\":1,\"customer_code\":\"CUS_1\",\"email\":\"buyer@solentik.com\"}}";
    private const string EmptyResponse = "{\"status\":true,\"message\":\"Success\"}";
}
