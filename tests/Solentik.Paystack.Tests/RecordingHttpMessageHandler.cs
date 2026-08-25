using System.Net;
using System.Text;

namespace Solentik.Paystack.Tests;

internal sealed class RecordingHttpMessageHandler(params string[] responseBodies) : HttpMessageHandler
{
    private readonly Queue<string> _responses = new(responseBodies);

    public List<RecordedRequest> Requests { get; } = [];

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null
            ? string.Empty
            : await request.Content.ReadAsStringAsync(cancellationToken);
        Requests.Add(new RecordedRequest(request.Method, request.RequestUri!, body));

        var responseBody = _responses.Count > 0
            ? _responses.Dequeue()
            : "{\"status\":true,\"data\":{}}";
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
        };
    }
}

internal sealed record RecordedRequest(HttpMethod Method, Uri Uri, string Body);
