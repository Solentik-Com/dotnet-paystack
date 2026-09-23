using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;
using Solentik.Paystack.Transfers.Models;

namespace Solentik.Paystack.Transfers;

internal sealed class TransferRecipientClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), ITransferRecipientClient
{
    public Task<PaystackResponse<TransferRecipient>> CreateAsync(CreateTransferRecipientRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRecipient(request);
        return PostAsync<TransferRecipient>("transferrecipient", request, cancellationToken);
    }

    public Task<PaystackResponse<BulkCreateRecipientsResult>> BulkCreateAsync(IReadOnlyList<CreateTransferRecipientRequest> recipients, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(recipients);
        if (recipients.Count == 0)
        {
            throw new ArgumentException("At least one recipient is required.", nameof(recipients));
        }

        foreach (var recipient in recipients)
        {
            ValidateRecipient(recipient);
        }

        return PostAsync<BulkCreateRecipientsResult>(
            "transferrecipient/bulk", new BulkCreateRecipientsPayload { Batch = recipients }, cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<TransferRecipient>>> ListAsync(TransferRecipientListOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddPositive(query, "page", options?.Page);
        RequestUtilities.Add(query, "from", options?.From?.ToString("O", CultureInfo.InvariantCulture));
        RequestUtilities.Add(query, "to", options?.To?.ToString("O", CultureInfo.InvariantCulture));
        return GetAsync<IReadOnlyList<TransferRecipient>>(RequestUtilities.WithQuery("transferrecipient", query), cancellationToken);
    }

    public Task<PaystackResponse<TransferRecipient>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default) =>
        GetAsync<TransferRecipient>($"transferrecipient/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", cancellationToken);

    public Task<PaystackResponse<TransferRecipient>> UpdateAsync(string idOrCode, UpdateTransferRecipientRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return PutAsync<TransferRecipient>(
            $"transferrecipient/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", request, cancellationToken);
    }

    public Task<PaystackResponse<JsonElement>> DeleteAsync(string idOrCode, CancellationToken cancellationToken = default) =>
        DeleteAsync<JsonElement>($"transferrecipient/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", cancellationToken);

    private static void ValidateRecipient(CreateTransferRecipientRequest request)
    {
        ValidateRequired(request.Type, nameof(request.Type));
        ValidateRequired(request.Name, nameof(request.Name));

        // Paystack requires account_number and bank_code for every recipient type except authorization-based ones.
        if (string.IsNullOrWhiteSpace(request.AuthorizationCode))
        {
            ValidateRequired(request.AccountNumber, nameof(request.AccountNumber));
            ValidateRequired(request.BankCode, nameof(request.BankCode));
        }
    }

    private static void ValidateRequired(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty.", parameterName);
        }
    }
}

internal sealed class BulkCreateRecipientsPayload
{
    [JsonPropertyName("batch")]
    public required IReadOnlyList<CreateTransferRecipientRequest> Batch { get; init; }
}
