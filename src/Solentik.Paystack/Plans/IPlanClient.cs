using Solentik.Paystack.Models;
using Solentik.Paystack.Plans.Models;

namespace Solentik.Paystack.Plans;

public interface IPlanClient
{
    Task<PaystackResponse<Plan>> CreateAsync(CreatePlanRequest request, CancellationToken cancellationToken = default);
    Task<PaystackResponse<IReadOnlyList<Plan>>> ListAsync(PlanListOptions? options = null, CancellationToken cancellationToken = default);
    Task<PaystackResponse<Plan>> FetchAsync(string idOrCode, CancellationToken cancellationToken = default);
    Task<PaystackResponse<Plan>> UpdateAsync(string idOrCode, UpdatePlanRequest request, CancellationToken cancellationToken = default);
}
