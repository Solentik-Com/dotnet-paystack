using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solentik.Paystack.PaymentRequests.Models;

public sealed class PaymentRequestLineItem
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("amount")]
    public required long Amount { get; init; }
    [JsonPropertyName("quantity")]
    public int? Quantity { get; init; }
}

public sealed class PaymentRequestTax
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("amount")]
    public required long Amount { get; init; }
}

public sealed class CreatePaymentRequestRequest
{
    [JsonPropertyName("customer")]
    public required string Customer { get; init; }
    [JsonPropertyName("amount")]
    public long? Amount { get; init; }
    [JsonPropertyName("due_date")]
    public DateTimeOffset? DueDate { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("line_items")]
    public IReadOnlyList<PaymentRequestLineItem>? LineItems { get; init; }
    [JsonPropertyName("tax")]
    public IReadOnlyList<PaymentRequestTax>? Tax { get; init; }
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
    [JsonPropertyName("send_notification")]
    public bool? SendNotification { get; init; }
    [JsonPropertyName("draft")]
    public bool? Draft { get; init; }
    [JsonPropertyName("has_invoice")]
    public bool? HasInvoice { get; init; }
    [JsonPropertyName("invoice_number")]
    public int? InvoiceNumber { get; init; }
    [JsonPropertyName("split_code")]
    public string? SplitCode { get; init; }
}

public sealed class UpdatePaymentRequestRequest
{
    [JsonPropertyName("customer")]
    public string? Customer { get; init; }
    [JsonPropertyName("amount")]
    public long? Amount { get; init; }
    [JsonPropertyName("due_date")]
    public DateTimeOffset? DueDate { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("line_items")]
    public IReadOnlyList<PaymentRequestLineItem>? LineItems { get; init; }
    [JsonPropertyName("tax")]
    public IReadOnlyList<PaymentRequestTax>? Tax { get; init; }
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
    [JsonPropertyName("send_notification")]
    public bool? SendNotification { get; init; }
    [JsonPropertyName("draft")]
    public bool? Draft { get; init; }
    [JsonPropertyName("invoice_number")]
    public int? InvoiceNumber { get; init; }
    [JsonPropertyName("split_code")]
    public string? SplitCode { get; init; }
}

public sealed class PaymentRequestListOptions
{
    public int? PerPage { get; init; }
    public int? Page { get; init; }
    public string? Customer { get; init; }
    public string? Status { get; init; }
    public string? Currency { get; init; }
    public bool? IncludeArchive { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}

public sealed class PaymentRequest
{
    [JsonPropertyName("id")]
    public long Id { get; init; }
    [JsonPropertyName("domain")]
    public string? Domain { get; init; }
    [JsonPropertyName("request_code")]
    public string? RequestCode { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("amount")]
    public long Amount { get; init; }
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
    [JsonPropertyName("due_date")]
    public DateTimeOffset? DueDate { get; init; }
    [JsonPropertyName("has_invoice")]
    public bool HasInvoice { get; init; }
    [JsonPropertyName("invoice_number")]
    public int? InvoiceNumber { get; init; }
    [JsonPropertyName("status")]
    public string? Status { get; init; }
    [JsonPropertyName("paid")]
    public bool Paid { get; init; }
    [JsonPropertyName("paid_at")]
    public DateTimeOffset? PaidAt { get; init; }
    [JsonPropertyName("pdf_url")]
    public string? PdfUrl { get; init; }
    [JsonPropertyName("line_items")]
    public IReadOnlyList<PaymentRequestLineItem>? LineItems { get; init; }
    [JsonPropertyName("tax")]
    public IReadOnlyList<PaymentRequestTax>? Tax { get; init; }
    [JsonPropertyName("customer")]
    public JsonElement? Customer { get; init; }
    [JsonPropertyName("split_code")]
    public string? SplitCode { get; init; }
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalData { get; init; }
}
