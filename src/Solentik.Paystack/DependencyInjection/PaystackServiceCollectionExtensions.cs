using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Customers;
using Solentik.Paystack.Miscellaneous;
using Solentik.Paystack.PaymentRequests;
using Solentik.Paystack.Plans;
using Solentik.Paystack.Subaccounts;
using Solentik.Paystack.Subscriptions;
using Solentik.Paystack.TransactionSplits;
using Solentik.Paystack.Transactions;
using Solentik.Paystack.Verification;
using Solentik.Paystack.Webhooks;

namespace Solentik.Paystack.DependencyInjection;

/// <summary>Provides dependency-injection registration for Solentik.Paystack.</summary>
public static class PaystackServiceCollectionExtensions
{
    /// <summary>The name of the single <see cref="HttpClient"/> shared by every Paystack resource client.</summary>
    internal const string HttpClientName = "Solentik.Paystack";

    public static IServiceCollection AddPaystack(
        this IServiceCollection services,
        Action<PaystackOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.AddOptions<PaystackOptions>()
            .Configure(configure)
            .Validate(ValidateOptions, "Paystack configuration is invalid.")
            .ValidateOnStart();

        return AddServices(services);
    }

    public static IServiceCollection AddPaystack(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<PaystackOptions>()
            .Bind(configuration.GetSection(PaystackOptions.SectionName))
            .Validate(ValidateOptions, "Paystack configuration is invalid.")
            .ValidateOnStart();

        return AddServices(services);
    }

    private static IServiceCollection AddServices(IServiceCollection services)
    {
        // A single named client is shared by every resource client so they draw from one
        // connection pool instead of one each, and get one resilience pipeline between them.
        services.AddHttpClient(HttpClientName)
            .AddStandardResilienceHandler(resilience =>
                // Paystack "create" calls (POST) are not idempotent - an automatic retry after a
                // lost response could create a second customer, plan, split, etc. Only GET is safe.
                resilience.Retry.DisableForUnsafeHttpMethods());

        services.AddTransient<ITransactionClient>(sp =>
            new TransactionClient(CreateHttpClient(sp), sp.GetRequiredService<IOptions<PaystackOptions>>()));
        services.AddTransient<ICustomerClient>(sp =>
            new CustomerClient(CreateHttpClient(sp), sp.GetRequiredService<IOptions<PaystackOptions>>()));
        services.AddTransient<IPlanClient>(sp =>
            new PlanClient(CreateHttpClient(sp), sp.GetRequiredService<IOptions<PaystackOptions>>()));
        services.AddTransient<ISubscriptionClient>(sp =>
            new SubscriptionClient(CreateHttpClient(sp), sp.GetRequiredService<IOptions<PaystackOptions>>()));
        services.AddTransient<ITransactionSplitClient>(sp =>
            new TransactionSplitClient(CreateHttpClient(sp), sp.GetRequiredService<IOptions<PaystackOptions>>()));
        services.AddTransient<ISubaccountClient>(sp =>
            new SubaccountClient(CreateHttpClient(sp), sp.GetRequiredService<IOptions<PaystackOptions>>()));
        services.AddTransient<IMiscellaneousClient>(sp =>
            new MiscellaneousClient(CreateHttpClient(sp), sp.GetRequiredService<IOptions<PaystackOptions>>()));
        services.AddTransient<IVerificationClient>(sp =>
            new VerificationClient(CreateHttpClient(sp), sp.GetRequiredService<IOptions<PaystackOptions>>()));
        services.AddTransient<IPaymentRequestClient>(sp =>
            new PaymentRequestClient(CreateHttpClient(sp), sp.GetRequiredService<IOptions<PaystackOptions>>()));
        services.AddTransient<IPaystackClient, PaystackClient>();
        services.AddSingleton<IPaystackWebhookVerifier, PaystackWebhookVerifier>();
        return services;
    }

    private static HttpClient CreateHttpClient(IServiceProvider services) =>
        services.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientName);

    private static bool ValidateOptions(PaystackOptions options) =>
        !string.IsNullOrWhiteSpace(options.SecretKey) &&
        options.BaseAddress.IsAbsoluteUri &&
        options.Timeout > TimeSpan.Zero;
}
