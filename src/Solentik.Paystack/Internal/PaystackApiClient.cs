using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Models;

namespace Solentik.Paystack.Internal;

internal abstract class PaystackApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;

    protected PaystackApiClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        var value = options.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = value.BaseAddress;
        _httpClient.Timeout = value.Timeout;
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", value.SecretKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    protected Task<PaystackResponse<T>> GetAsync<T>(string path, CancellationToken cancellationToken) =>
        SendAsync<T>(new HttpRequestMessage(HttpMethod.Get, path), cancellationToken);

    protected Task<PaystackResponse<T>> PostAsync<T>(
        string path,
        object? payload,
        CancellationToken cancellationToken) =>
        SendWithJsonAsync<T>(HttpMethod.Post, path, payload, cancellationToken);

    protected Task<PaystackResponse<T>> PutAsync<T>(
        string path,
        object payload,
        CancellationToken cancellationToken) =>
        SendWithJsonAsync<T>(HttpMethod.Put, path, payload, cancellationToken);

    private Task<PaystackResponse<T>> SendWithJsonAsync<T>(
        HttpMethod method,
        string path,
        object? payload,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(method, path);
        if (payload is not null)
        {
            request.Content = JsonContent.Create(payload, options: JsonOptions);
        }

        return SendAsync<T>(request, cancellationToken);
    }

    private async Task<PaystackResponse<T>> SendAsync<T>(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using (request)
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            PaystackResponse<T>? envelope = null;
            try
            {
                envelope = JsonSerializer.Deserialize<PaystackResponse<T>>(content, JsonOptions);
            }
            catch (JsonException exception)
            {
                if (response.IsSuccessStatusCode)
                {
                    throw new PaystackException(
                        "Paystack returned an invalid JSON response.",
                        response.StatusCode,
                        innerException: exception);
                }
            }

            if (!response.IsSuccessStatusCode || envelope is null || !envelope.Status)
            {
                ErrorEnvelope? error = null;
                try
                {
                    error = JsonSerializer.Deserialize<ErrorEnvelope>(content, JsonOptions);
                }
                catch (JsonException)
                {
                    // The body wasn't a valid error envelope; fall back to the status reason phrase below.
                }

                throw new PaystackException(
                    error?.Message ?? response.ReasonPhrase ?? "An unknown Paystack error occurred.",
                    response.StatusCode,
                    error?.Type ?? "api_error",
                    error?.Code,
                    error?.Meta);
            }

            return envelope;
        }
    }

    private sealed class ErrorEnvelope
    {
        public string? Message { get; init; }
        public string? Type { get; init; }
        public string? Code { get; init; }
        public JsonElement? Meta { get; init; }
    }
}
