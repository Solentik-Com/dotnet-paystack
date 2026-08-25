using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Solentik.Paystack.AspNetCore.DependencyInjection;
using Solentik.Paystack.AspNetCore.Routing;
using Solentik.Paystack.AspNetCore.Webhooks;

namespace Solentik.Paystack.AspNetCore.Tests;

public sealed class WebhookRoutingTests
{
    [Fact]
    public async Task MapPaystackWebhook_UsesConfiguredPath()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddPaystackWebhooks(options =>
            options.Path = "/integrations/paystack");
        await using var app = builder.Build();

        app.MapPaystackWebhook();

        Assert.Equal("/integrations/paystack", GetRoutePattern(app));
    }

    [Fact]
    public async Task MapPaystackWebhook_BindsPathFromConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Paystack:Webhooks:Path"] = "/configured/from-appsettings",
                ["Paystack:Webhooks:MaximumBodySize"] = "4096"
            })
            .Build();
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddPaystackWebhooks(configuration);
        await using var app = builder.Build();

        app.MapPaystackWebhook();

        Assert.Equal("/configured/from-appsettings", GetRoutePattern(app));
        Assert.Equal(
            4096,
            app.Services.GetRequiredService<IOptions<PaystackWebhookOptions>>()
                .Value
                .MaximumBodySize);
    }

    [Fact]
    public async Task ExplicitPath_OverridesConfiguredPath()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddPaystackWebhooks(options =>
            options.Path = "/configured/path");
        await using var app = builder.Build();

        app.MapPaystackWebhook("/explicit/path");

        Assert.Equal("/explicit/path", GetRoutePattern(app));
    }

    [Fact]
    public void AddPaystackWebhooks_DefaultsTo512KiB()
    {
        var services = new ServiceCollection();
        services.AddPaystackWebhooks();
        using var provider = services.BuildServiceProvider();

        Assert.Equal(
            512 * 1024,
            provider.GetRequiredService<IOptions<PaystackWebhookOptions>>()
                .Value
                .MaximumBodySize);
    }

    [Theory]
    [InlineData("")]
    [InlineData("paystack/webhook")]
    [InlineData("/paystack/webhook?token=value")]
    [InlineData("/paystack/webhook#fragment")]
    public void AddPaystackWebhooks_RejectsInvalidPath(string path)
    {
        var services = new ServiceCollection();
        services.AddPaystackWebhooks(options => options.Path = path);
        using var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(() =>
            provider.GetRequiredService<IOptions<PaystackWebhookOptions>>().Value);
    }

    private static string? GetRoutePattern(WebApplication app) =>
        ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Single()
            .RoutePattern
            .RawText;
}
