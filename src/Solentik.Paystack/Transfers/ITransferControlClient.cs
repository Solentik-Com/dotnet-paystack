using System.Text.Json;
using Solentik.Paystack.Models;
using Solentik.Paystack.Transfers.Models;

namespace Solentik.Paystack.Transfers;

public interface ITransferControlClient
{
    Task<PaystackResponse<IReadOnlyList<BalanceEntry>>> CheckBalanceAsync(CancellationToken cancellationToken = default);

    Task<PaystackResponse<IReadOnlyList<BalanceLedgerEntry>>> FetchLedgerAsync(BalanceLedgerListOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Generates a new OTP and sends it to the business phone number. The response carries only a message.</summary>
    Task<PaystackResponse<JsonElement>> ResendOtpAsync(string transferCode, string reason = "resend_otp", CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests disabling the Transfers OTP requirement; Paystack sends an OTP to confirm. Finalize with
    /// <see cref="FinalizeDisableOtpAsync"/>. The response carries only a message.
    /// </summary>
    Task<PaystackResponse<JsonElement>> RequestDisableOtpAsync(CancellationToken cancellationToken = default);

    /// <summary>The response carries only a message.</summary>
    Task<PaystackResponse<JsonElement>> FinalizeDisableOtpAsync(string otp, CancellationToken cancellationToken = default);

    /// <summary>The response carries only a message.</summary>
    Task<PaystackResponse<JsonElement>> EnableOtpAsync(CancellationToken cancellationToken = default);
}
