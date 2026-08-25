using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Solentik.Paystack.Webhooks;

public sealed class PaystackWebhookVerifier : IPaystackWebhookVerifier
{
    private readonly byte[] _secret;

    public PaystackWebhookVerifier(IOptions<PaystackOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (string.IsNullOrWhiteSpace(options.Value.SecretKey))
        {
            throw new ArgumentException("A Paystack secret key is required.", nameof(options));
        }

        _secret = Encoding.UTF8.GetBytes(options.Value.SecretKey);
    }

    public bool IsValid(string payload, string? signature)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return IsValid(Encoding.UTF8.GetBytes(payload), signature);
    }

    public bool IsValid(ReadOnlySpan<byte> payload, string? signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
        {
            return false;
        }

        byte[] suppliedHash;
        try
        {
            suppliedHash = Convert.FromHexString(signature);
        }
        catch (FormatException)
        {
            return false;
        }

        var expectedHash = HMACSHA512.HashData(_secret, payload);
        return suppliedHash.Length == expectedHash.Length &&
               CryptographicOperations.FixedTimeEquals(suppliedHash, expectedHash);
    }
}
