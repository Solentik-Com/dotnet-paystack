using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;
using Solentik.Paystack.Subscriptions.Models;

namespace Solentik.Paystack.Subscriptions;

internal sealed class SubscriptionClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), ISubscriptionClient
{
    public Task<PaystackResponse<Subscription>> CreateAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PostAsync<Subscription>("subscription", request, cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<Subscription>>> ListAsync(SubscriptionListOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddPositive(query, "page", options?.Page);
        RequestUtilities.Add(query, "customer", options?.Customer);
        RequestUtilities.Add(query, "plan", options?.Plan);
        return GetAsync<IReadOnlyList<Subscription>>(RequestUtilities.WithQuery("subscription", query), cancellationToken);
    }

    public Task<PaystackResponse<Subscription>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default) =>
        GetAsync<Subscription>($"subscription/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", cancellationToken);

    public Task<PaystackResponse<JsonElement>> EnableAsync(string code, string token, CancellationToken cancellationToken = default) =>
        SetStateAsync("enable", code, token, cancellationToken);

    public Task<PaystackResponse<JsonElement>> DisableAsync(string code, string token, CancellationToken cancellationToken = default) =>
        SetStateAsync("disable", code, token, cancellationToken);

    public Task<PaystackResponse<SubscriptionManagementLink>> GetUpdateLinkAsync(string code, CancellationToken cancellationToken = default) =>
        GetAsync<SubscriptionManagementLink>($"subscription/{RequestUtilities.EscapeRequired(code, nameof(code))}/manage/link", cancellationToken);

    public Task<PaystackResponse<JsonElement>> SendUpdateLinkAsync(string code, CancellationToken cancellationToken = default) =>
        PostAsync<JsonElement>($"subscription/{RequestUtilities.EscapeRequired(code, nameof(code))}/manage/email", null, cancellationToken);

    private Task<PaystackResponse<JsonElement>> SetStateAsync(string action, string code, string token, CancellationToken cancellationToken) =>
        PostAsync<JsonElement>($"subscription/{action}", new SubscriptionStateRequest(
            ValidateRequired(code, nameof(code)),
            ValidateRequired(token, nameof(token))), cancellationToken);

    private static string ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty.", parameterName);
        }

        return value;
    }

    private sealed record SubscriptionStateRequest(
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("token")] string Token);
}
