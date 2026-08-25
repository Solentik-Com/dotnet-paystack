namespace Solentik.Paystack.Webhooks;

/// <summary>Verifies webhook signatures sent by Paystack.</summary>
public interface IPaystackWebhookVerifier
{
    /// <summary>Verifies a hexadecimal x-paystack-signature against the exact raw request body.</summary>
    bool IsValid(ReadOnlySpan<byte> payload, string? signature);

    /// <summary>Verifies a UTF-8 payload against a hexadecimal x-paystack-signature.</summary>
    bool IsValid(string payload, string? signature);
}
