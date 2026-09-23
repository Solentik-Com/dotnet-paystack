using System.Text.Json;
using Solentik.Paystack.Models;
using Solentik.Paystack.Transfers.Models;

namespace Solentik.Paystack.Transfers;

public interface ITransferRecipientClient
{
    Task<PaystackResponse<TransferRecipient>> CreateAsync(CreateTransferRecipientRequest request, CancellationToken cancellationToken = default);

    Task<PaystackResponse<BulkCreateRecipientsResult>> BulkCreateAsync(IReadOnlyList<CreateTransferRecipientRequest> recipients, CancellationToken cancellationToken = default);

    Task<PaystackResponse<IReadOnlyList<TransferRecipient>>> ListAsync(TransferRecipientListOptions? options = null, CancellationToken cancellationToken = default);

    Task<PaystackResponse<TransferRecipient>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default);

    Task<PaystackResponse<TransferRecipient>> UpdateAsync(string idOrCode, UpdateTransferRecipientRequest request, CancellationToken cancellationToken = default);

    /// <summary>Sets the recipient to inactive rather than deleting it outright.</summary>
    Task<PaystackResponse<JsonElement>> DeleteAsync(string idOrCode, CancellationToken cancellationToken = default);
}
