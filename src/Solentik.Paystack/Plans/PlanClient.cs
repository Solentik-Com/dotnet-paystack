using System.Globalization;
using Microsoft.Extensions.Options;
using Solentik.Paystack.Internal;
using Solentik.Paystack.Models;
using Solentik.Paystack.Plans.Models;

namespace Solentik.Paystack.Plans;

internal sealed class PlanClient(HttpClient httpClient, IOptions<PaystackOptions> options)
    : PaystackApiClient(httpClient, options), IPlanClient
{
    public Task<PaystackResponse<Plan>> CreateAsync(CreatePlanRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateAmount(request.Amount, nameof(request));
        return PostAsync<Plan>("plan", request, cancellationToken);
    }

    public Task<PaystackResponse<IReadOnlyList<Plan>>> ListAsync(PlanListOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new List<KeyValuePair<string, string>>();
        RequestUtilities.AddPositive(query, "perPage", options?.PerPage);
        RequestUtilities.AddPositive(query, "page", options?.Page);
        RequestUtilities.Add(query, "interval", options?.Interval);
        if (options?.Amount is not null)
        {
            ValidateAmount(options.Amount.Value, nameof(options.Amount));
            RequestUtilities.Add(query, "amount", options.Amount.Value.ToString(CultureInfo.InvariantCulture));
        }

        return GetAsync<IReadOnlyList<Plan>>(RequestUtilities.WithQuery("plan", query), cancellationToken);
    }

    public Task<PaystackResponse<Plan>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default) =>
        GetAsync<Plan>($"plan/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", cancellationToken);

    public Task<PaystackResponse<Plan>> UpdateAsync(string idOrCode, UpdatePlanRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Amount is not null)
        {
            ValidateAmount(request.Amount.Value, nameof(request));
        }

        return PutAsync<Plan>($"plan/{RequestUtilities.EscapeRequired(idOrCode, nameof(idOrCode))}", request, cancellationToken);
    }

    private static void ValidateAmount(long amount, string parameterName)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "The amount must be greater than zero.");
        }
    }
}
