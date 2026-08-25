namespace Solentik.Paystack.AspNetCore.Webhooks;

public sealed class PaystackWebhookOptions
{
    public const string SectionName = "Paystack:Webhooks";
    public const string DefaultPath = "/paystack/webhook";

    /// <summary>Gets or sets the route used to receive Paystack webhooks.</summary>
    public string Path { get; set; } = DefaultPath;

    /// <summary>Gets or sets the largest accepted webhook body, in bytes.</summary>
    public int MaximumBodySize { get; set; } = 512 * 1024;
}
