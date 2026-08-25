namespace Solentik.Paystack;

/// <summary>Configures the Paystack API client.</summary>
public sealed class PaystackOptions
{
    public const string SectionName = "Paystack";

    /// <summary>Gets or sets the Paystack secret key used for API calls and webhook verification.</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Gets or sets the Paystack API base address.</summary>
    public Uri BaseAddress { get; set; } = new("https://api.paystack.co/");

    /// <summary>Gets or sets the timeout applied to Paystack API calls.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(45);
}
