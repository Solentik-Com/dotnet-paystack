using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Customers.Models;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;

namespace Solentik.Paystack.Customers;

internal sealed class CustomerClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), ICustomerClient
{
    public Task<PaystackResponse<Customer>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<Customer>("customer", request, cancellationToken);
    }

    public Task<PaystackResponse<Customer>> FetchAsync(string emailOrCode, CancellationToken cancellationToken = default) =>
        GetAsync<Customer>($"customer/{RequestUtilities.EscapeRequired(emailOrCode, nameof(emailOrCode))}", cancellationToken);

    public Task<PaystackResponse<Customer>> UpdateAsync(string code, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PutAsync<Customer>($"customer/{RequestUtilities.EscapeRequired(code, nameof(code))}", request, cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<Customer>>> ListAsync(CustomerListOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddPositive(query, "page", options?.Page);
        RequestUtilities.Add(query, "from", options?.From?.ToString("O", CultureInfo.InvariantCulture));
        RequestUtilities.Add(query, "to", options?.To?.ToString("O", CultureInfo.InvariantCulture));
        return GetAsync<IReadOnlyList<Customer>>(RequestUtilities.WithQuery("customer", query), cancellationToken);
    }

    public Task<PaystackResponse<JsonElement>> ValidateIdentityAsync(string code, ValidateCustomerIdentityRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<JsonElement>($"customer/{RequestUtilities.EscapeRequired(code, nameof(code))}/identification", request, cancellationToken);
    }

    public Task<PaystackResponse<Customer>> SetRiskActionAsync(SetCustomerRiskActionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<Customer>("customer/set_risk_action", request, cancellationToken);
    }

    public Task<PaystackResponse<JsonElement>> DeactivateAuthorizationAsync(string authorizationCode, CancellationToken cancellationToken = default) =>
        PostAsync<JsonElement>("customer/authorization/deactivate", new DeactivateAuthorizationRequest(authorizationCode), cancellationToken);

    public Task<PaystackResponse<InitializeAuthorizationData>> InitializeAuthorizationAsync(InitializeAuthorizationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<InitializeAuthorizationData>("customer/authorization/initialize", request, cancellationToken);
    }

    public Task<PaystackResponse<AuthorizationStatus>> VerifyAuthorizationAsync(string reference, CancellationToken cancellationToken = default) =>
        GetAsync<AuthorizationStatus>($"customer/authorization/verify/{RequestUtilities.EscapeRequired(reference, nameof(reference))}", cancellationToken);

    private sealed record DeactivateAuthorizationRequest(
        [property: JsonPropertyName("authorization_code")] string AuthorizationCode);
}
