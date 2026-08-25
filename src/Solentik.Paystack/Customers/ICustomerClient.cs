using System.Text.Json;
using Solentik.Paystack.Customers.Models;
using Solentik.Paystack.Models;

namespace Solentik.Paystack.Customers;

public interface ICustomerClient
{
    Task<PaystackResponse<Customer>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<Customer>> FetchAsync(string emailOrCode, CancellationToken cancellationToken = default);
    Task<PaystackResponse<Customer>> UpdateAsync(string code, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<IReadOnlyList<Customer>>> ListAsync(CustomerListOptions? options = null, CancellationToken cancellationToken = default);
    Task<PaystackResponse<JsonElement>> ValidateIdentityAsync(string code, ValidateCustomerIdentityRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<Customer>> SetRiskActionAsync(SetCustomerRiskActionRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<JsonElement>> DeactivateAuthorizationAsync(string authorizationCode, CancellationToken cancellationToken = default);
    Task<PaystackResponse<InitializeAuthorizationData>> InitializeAuthorizationAsync(InitializeAuthorizationRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<AuthorizationStatus>> VerifyAuthorizationAsync(string reference, CancellationToken cancellationToken = default);
}
