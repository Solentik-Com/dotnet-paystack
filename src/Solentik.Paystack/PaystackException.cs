using System.Net;
using System.Text.Json;

namespace Solentik.Paystack;

/// <summary>Represents an unsuccessful response returned by Paystack.</summary>
public sealed class PaystackException : HttpRequestException
{
    public PaystackException(
        string message,
        HttpStatusCode statusCode,
        string type = "api_error",
        string? paystackCode = null,
        JsonElement? metadata = null,
        Exception? innerException = null)
        : base(message, innerException, statusCode)
    {
        Type = type;
        PaystackCode = paystackCode;
        Metadata = metadata;
    }

    public string Type { get; }

    public string? PaystackCode { get; }

    public JsonElement? Metadata { get; }
}
