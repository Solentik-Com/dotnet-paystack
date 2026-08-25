using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Solentik.Paystack.AspNetCore.Webhooks;

namespace Solentik.Paystack.AspNetCore.DependencyInjection;

public static class PaystackWebhookServiceCollectionExtensions
{
    public static IServiceCollection AddPaystackWebhooks(
        this IServiceCollection services,
        Action<PaystackWebhookOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = AddValidatedOptions(services);
        if (configure is not null)
        {
            options.Configure(configure);
        }

        return AddDispatcher(services);
    }

    public static IServiceCollection AddPaystackWebhooks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        AddValidatedOptions(services)
            .Bind(configuration.GetSection(PaystackWebhookOptions.SectionName));

        return AddDispatcher(services);
    }

    public static IServiceCollection AddPaystackWebhookHandler<TEvent, THandler>(this IServiceCollection services)
        where TEvent : PaystackWebhookEvent
        where THandler : class, IPaystackWebhookHandler<TEvent>
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<IPaystackWebhookHandler<TEvent>, THandler>();
        return services;
    }

    private static OptionsBuilder<PaystackWebhookOptions> AddValidatedOptions(IServiceCollection services) =>
        services.AddOptions<PaystackWebhookOptions>()
            .Validate(value => value.MaximumBodySize > 0, "MaximumBodySize must be greater than zero.")
            .Validate(
                value => IsValidPath(value.Path),
                "Path must start with '/' and cannot contain a query string or fragment.")
            .ValidateOnStart();

    private static IServiceCollection AddDispatcher(IServiceCollection services)
    {
        services.AddScoped<IPaystackWebhookDispatcher, PaystackWebhookDispatcher>();
        return services;
    }

    private static bool IsValidPath(string? path) =>
        !string.IsNullOrWhiteSpace(path) &&
        path.StartsWith("/", StringComparison.Ordinal) &&
        !path.Contains('?') &&
        !path.Contains('#');
}
