using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Solentik.Paystack.AspNetCore.Webhooks;
using Solentik.Paystack.Webhooks;

namespace Solentik.Paystack.AspNetCore.Routing;

public static class PaystackWebhookEndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapPaystackWebhook(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        var options = endpoints.ServiceProvider
            .GetRequiredService<IOptions<PaystackWebhookOptions>>()
            .Value;

        return endpoints.MapPaystackWebhook(options.Path);
    }

    public static RouteHandlerBuilder MapPaystackWebhook(
        this IEndpointRouteBuilder endpoints,
        string pattern)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);

        return endpoints.MapPost(pattern, PaystackWebhookEndpoint.HandleAsync)
            .WithName("PaystackWebhook");
    }
}

internal static class PaystackWebhookEndpoint
{
    public static async Task<IResult> HandleAsync(
        HttpContext context,
        IPaystackWebhookVerifier verifier,
        IPaystackWebhookDispatcher dispatcher,
        IOptions<PaystackWebhookOptions> options,
        CancellationToken cancellationToken)
    {
        var maximumBodySize = options.Value.MaximumBodySize;
        if (context.Request.ContentLength > maximumBodySize)
        {
            return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);
        }

        byte[] payload;
        try
        {
            payload = await ReadBodyAsync(context.Request.Body, maximumBodySize, cancellationToken);
        }
        catch (PayloadTooLargeException)
        {
            return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);
        }

        var signature = context.Request.Headers["x-paystack-signature"].ToString();
        if (!verifier.IsValid(payload, signature))
        {
            return Results.Unauthorized();
        }

        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            var root = document.RootElement;
            if (root.ValueKind != System.Text.Json.JsonValueKind.Object ||
                !root.TryGetProperty("event", out var eventProperty) ||
                eventProperty.ValueKind != System.Text.Json.JsonValueKind.String ||
                string.IsNullOrWhiteSpace(eventProperty.GetString()))
            {
                return Results.BadRequest(new { message = "A Paystack event name is required." });
            }

            var eventName = eventProperty.GetString()!;
            var data = root.TryGetProperty("data", out var dataProperty)
                ? dataProperty.Clone()
                : default;
            var clonedPayload = root.Clone();

            await dispatcher.DispatchAsync(eventName, data, clonedPayload, cancellationToken);
            return Results.Ok(new { message = "Webhook Received" });
        }
        catch (System.Text.Json.JsonException)
        {
            return Results.BadRequest(new { message = "The webhook body is not valid JSON." });
        }
    }

    private static async Task<byte[]> ReadBodyAsync(Stream body, int maximumBodySize, CancellationToken cancellationToken)
    {
        using var output = new MemoryStream();
        var buffer = new byte[Math.Min(81920, maximumBodySize)];
        while (true)
        {
            var read = await body.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
            if (read == 0)
            {
                return output.ToArray();
            }

            if (output.Length + read > maximumBodySize)
            {
                throw new PayloadTooLargeException();
            }

            await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
    }

    private sealed class PayloadTooLargeException : Exception;
}
