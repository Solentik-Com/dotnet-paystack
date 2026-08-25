using Microsoft.Extensions.Options;
using Solentik.Paystack.Miscellaneous;
using Solentik.Paystack.Miscellaneous.Models;

namespace Solentik.Paystack.Tests;

public sealed class MiscellaneousClientTests
{
    [Fact]
    public async Task ListBanksAsync_AddsOnlyProvidedFilters()
    {
        const string banks = "{\"status\":true,\"data\":[{\"id\":1,\"name\":\"Access Bank\",\"code\":\"044\"}]}";
        var handler = new RecordingHttpMessageHandler(banks);
        var client = CreateClient(handler);

        var response = await client.ListBanksAsync(
            new BankListOptions { Country = "ghana", PayWithBank = true, Type = "mobile_money" },
            TestContext.Current.CancellationToken);

        Assert.Equal("Access Bank", response.Data?[0].Name);
        var uri = handler.Requests[0].Uri;
        Assert.Equal("/bank", uri.AbsolutePath);
        Assert.Contains("country=ghana", uri.Query);
        Assert.Contains("pay_with_bank=true", uri.Query);
        Assert.Contains("type=mobile_money", uri.Query);
    }

    [Fact]
    public async Task ListBanksAsync_WithNoOptions_RequestsUnfilteredList()
    {
        var handler = new RecordingHttpMessageHandler("{\"status\":true,\"data\":[]}");
        var client = CreateClient(handler);

        await client.ListBanksAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal("/bank", handler.Requests[0].Uri.AbsolutePath);
        Assert.Equal(string.Empty, handler.Requests[0].Uri.Query);
    }

    [Fact]
    public async Task ListCountriesAsync_RequestsCountryEndpoint()
    {
        const string countries = "{\"status\":true,\"data\":[{\"id\":1,\"name\":\"Ghana\",\"iso_code\":\"GH\"}]}";
        var handler = new RecordingHttpMessageHandler(countries);
        var client = CreateClient(handler);

        var response = await client.ListCountriesAsync(TestContext.Current.CancellationToken);

        Assert.Equal("GH", response.Data?[0].IsoCode);
        Assert.Equal("/country", handler.Requests[0].Uri.AbsolutePath);
    }

    [Fact]
    public async Task ListStatesAsync_RequiresCountryCode()
    {
        var client = CreateClient(new RecordingHttpMessageHandler());
        await Assert.ThrowsAsync<ArgumentException>(() => client.ListStatesAsync(" ", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ListStatesAsync_SendsCountryQuery()
    {
        const string states = "{\"status\":true,\"data\":[{\"name\":\"Accra\",\"abbreviation\":\"AC\"}]}";
        var handler = new RecordingHttpMessageHandler(states);
        var client = CreateClient(handler);

        var response = await client.ListStatesAsync("GH", TestContext.Current.CancellationToken);

        Assert.Equal("AC", response.Data?[0].Abbreviation);
        var uri = handler.Requests[0].Uri;
        Assert.Equal("/address_verification/states", uri.AbsolutePath);
        Assert.Contains("country=GH", uri.Query);
    }

    private static MiscellaneousClient CreateClient(RecordingHttpMessageHandler handler) =>
        new(new HttpClient(handler), Options.Create(new PaystackOptions { SecretKey = "sk_test" }));
}
