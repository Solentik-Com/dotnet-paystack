using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Webhooks;

namespace Solentik.Paystack.Tests;

public sealed class WebhookVerifierTests
{
    private const string Secret = "sk_test_secret";
    private const string Payload = "{\"event\":\"charge.success\",\"data\":{\"id\":42}}";

    [Fact]
    public void Constructor_RejectsEmptySecretKey()
    {
        Assert.Throws<ArgumentException>(() =>
            new PaystackWebhookVerifier(Options.Create(new PaystackOptions())));
    }

    [Fact]
    public void IsValid_AcceptsMatchingPaystackSignature()
    {
        var signature = Convert.ToHexString(
            HMACSHA512.HashData(Encoding.UTF8.GetBytes(Secret), Encoding.UTF8.GetBytes(Payload)))
            .ToLowerInvariant();
        var verifier = CreateVerifier();

        Assert.True(verifier.IsValid(Payload, signature));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-hex")]
    [InlineData("00")]
    public void IsValid_RejectsMissingOrInvalidSignature(string? signature)
    {
        var verifier = CreateVerifier();

        Assert.False(verifier.IsValid(Payload, signature));
    }

    [Fact]
    public void IsValid_RejectsModifiedPayload()
    {
        var signature = Convert.ToHexString(
            HMACSHA512.HashData(Encoding.UTF8.GetBytes(Secret), Encoding.UTF8.GetBytes(Payload)));
        var verifier = CreateVerifier();

        Assert.False(verifier.IsValid(Payload + " ", signature));
    }

    private static PaystackWebhookVerifier CreateVerifier() =>
        new(Options.Create(new PaystackOptions { SecretKey = Secret }));
}
