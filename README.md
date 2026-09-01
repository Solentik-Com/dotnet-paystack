# Solentik.Paystack

An idiomatic, strongly typed .NET client for the Paystack API, with optional ASP.NET Core webhook routing and event handlers.

## Documentation

Full installation, configuration, resource, and ASP.NET Core webhook documentation is available at:

**https://paystack.solentik.com/dotnet/introduction.html**

## Packages

| Package | Purpose |
| --- | --- |
| `Solentik.Paystack` | Framework-neutral Paystack API client |
| `Solentik.Paystack.AspNetCore` | Verified webhook endpoint and typed event dispatch for ASP.NET Core |

Both packages target:

- .NET 8
- .NET 9
- .NET 10

## Installation

Install the core package:

```bash
dotnet add package Solentik.Paystack
```

For ASP.NET Core webhook support, also install:

```bash
dotnet add package Solentik.Paystack.AspNetCore
```

## Configuration

Add the secret key to configuration. Do not commit production keys to source control.

```json
{
  "Paystack": {
    "SecretKey": "sk_test_xxxx",
    "BaseAddress": "https://api.paystack.co/",
    "Timeout": "00:00:45",
    "Webhooks": {
      "Path": "/integrations/paystack/webhook",
      "MaximumBodySize": 524288
    }
  }
}
```

Register the client with dependency injection:

```csharp
using Solentik.Paystack.DependencyInjection;

builder.Services.AddPaystack(builder.Configuration);
```

Configuration can also be supplied directly:

```csharp
builder.Services.AddPaystack(options =>
{
    options.SecretKey = builder.Configuration["Paystack:SecretKey"]!;
    options.Timeout = TimeSpan.FromSeconds(45);
});
```

The package uses a single `IHttpClientFactory`-managed `HttpClient` internally, shared by every resource client, with standard retry/timeout/circuit-breaker resilience applied to GET requests. Inject `IPaystackClient` or an individual resource interface; do not construct or retain `HttpClient` instances yourself.

## Transactions

```csharp
using Solentik.Paystack;
using Solentik.Paystack.Transactions.Models;

public sealed class CheckoutService(IPaystackClient paystack)
{
    public async Task<string?> InitializeAsync(
        string email,
        long amount,
        CancellationToken cancellationToken)
    {
        var response = await paystack.Transactions.InitializeAsync(
            new InitializeTransactionRequest
            {
                Email = email,
                Amount = amount,
                Currency = "GHS"
            },
            cancellationToken);

        return response.Data?.AuthorizationUrl;
    }
}
```

Transaction amounts are supplied in the currency's smallest unit.

### Splitting a transaction with a subaccount

To route a share of a transaction to a subaccount at checkout, set `Subaccount` (and optionally
`TransactionCharge` and `Bearer`) on `InitializeTransactionRequest`:

```csharp
var response = await paystack.Transactions.InitializeAsync(
    new InitializeTransactionRequest
    {
        Email = email,
        Amount = amount,
        Subaccount = "ACCT_xxxx",
        Bearer = "subaccount" // "account" or "subaccount" - who pays the Paystack fees
    },
    cancellationToken);
```

`TransactionCharge` overrides the subaccount's configured percentage charge with a flat fee (in the
currency's smallest unit) for that transaction only.

For a multi-party split instead of a single subaccount, create a split with
`paystack.TransactionSplits.CreateAsync` and pass its code as `SplitCode`:

```csharp
var split = await paystack.TransactionSplits.CreateAsync(/* ... */);

var response = await paystack.Transactions.InitializeAsync(
    new InitializeTransactionRequest
    {
        Email = email,
        Amount = amount,
        SplitCode = split.Data?.SplitCode
    },
    cancellationToken);
```

`Subaccount`/`TransactionCharge`/`Bearer` and `SplitCode` are alternative ways to split a transaction;
Paystack does not expect both to be set on the same request.

## Customers

```csharp
using Solentik.Paystack.Customers.Models;

var response = await paystack.Customers.CreateAsync(
    new CreateCustomerRequest
    {
        Email = "customer@solentik.com",
        FirstName = "Ama",
        LastName = "Mensah"
    });

var customerCode = response.Data?.CustomerCode;
```

Customer operations include create, fetch, update, list, identity validation, risk actions, and reusable authorization management.

## Plans and subscriptions

```csharp
using Solentik.Paystack.Plans.Models;
using Solentik.Paystack.Subscriptions.Models;

var plan = await paystack.Plans.CreateAsync(
    new CreatePlanRequest
    {
        Name = "Monthly Pro",
        Amount = 5000,
        Interval = "monthly",
        Currency = "GHS"
    });

var subscription = await paystack.Subscriptions.CreateAsync(
    new CreateSubscriptionRequest
    {
        Customer = "CUS_xxxx",
        Plan = plan.Data!.PlanCode!
    });
```

Subscriptions can also be listed, fetched, enabled, disabled, and managed through card-update links.

## Transaction splits

```csharp
using Solentik.Paystack.TransactionSplits.Models;

var split = await paystack.TransactionSplits.CreateAsync(
    new CreateTransactionSplitRequest
    {
        Name = "Marketplace Split",
        Type = "percentage",
        Currency = "GHS",
        Subaccounts =
        [
            new SplitSubaccountRequest
            {
                Subaccount = "ACCT_xxxx",
                Share = 20
            }
        ]
    });
```

Splits support create, list, fetch, update, add/update subaccount, and remove subaccount operations.

## Subaccounts

```csharp
using Solentik.Paystack.Subaccounts.Models;

var subaccount = await paystack.Subaccounts.CreateAsync(
    new CreateSubaccountRequest
    {
        BusinessName = "Sunshine Studios",
        SettlementBank = "058",
        AccountNumber = "0123456047",
        PercentageCharge = 18.2m,
        PrimaryContactEmail = "dafe@sunshinestudios.com"
    });

var subaccountCode = subaccount.Data?.SubaccountCode;
```

Subaccounts support create, list, fetch, and update operations.

## Payment requests (invoicing)

```csharp
using Solentik.Paystack.PaymentRequests.Models;

var invoice = await paystack.PaymentRequests.CreateAsync(
    new CreatePaymentRequestRequest
    {
        Customer = "CUS_xxxx",
        Amount = 5000000,
        DueDate = DateTimeOffset.UtcNow.AddDays(7),
        Description = "Website design",
        LineItems =
        [
            new PaymentRequestLineItem { Name = "Design", Amount = 4000000 },
            new PaymentRequestLineItem { Name = "Hosting", Amount = 1000000 }
        ]
    });

var requestCode = invoice.Data?.RequestCode;
await paystack.PaymentRequests.FinalizeAsync(requestCode!);
await paystack.PaymentRequests.NotifyAsync(requestCode!);
```

Payment requests support create, list, fetch, verify, notify, totals, finalize, update, and archive operations.

Pass `SplitCode` on `CreatePaymentRequestRequest`/`UpdatePaymentRequestRequest` to route the invoice's
payment through a transaction split created with `paystack.TransactionSplits.CreateAsync`.

## Miscellaneous

```csharp
using Solentik.Paystack.Miscellaneous.Models;

// Bank list, filterable by country
var banks = await paystack.Miscellaneous.ListBanksAsync(
    new BankListOptions { Country = "ghana" });

// Mobile money channels use the same endpoint, filtered by type
var mobileMoneyChannels = await paystack.Miscellaneous.ListBanksAsync(
    new BankListOptions { Country = "ghana", Type = "mobile_money" });

var countries = await paystack.Miscellaneous.ListCountriesAsync();
var states = await paystack.Miscellaneous.ListStatesAsync("NG");
```

## Verification

```csharp
using Solentik.Paystack.Verification.Models;

// Resolve an account number (bank or mobile money) to an account name
var resolved = await paystack.Verification.ResolveAccountAsync("0123456789", "058");
var accountName = resolved.Data?.AccountName;

// Fuller KYC validation
var validated = await paystack.Verification.ValidateAccountAsync(
    new ValidateAccountRequest
    {
        AccountName = "Ann Bron",
        AccountNumber = "0123456789",
        AccountType = "personal",
        BankCode = "632005",
        CountryCode = "ZA",
        DocumentType = "identityNumber",
        DocumentNumber = "1234567890123"
    });

// Card BIN lookup
var cardInfo = await paystack.Verification.ResolveCardBinAsync("539983");
```

## ASP.NET Core webhooks

Register webhook services and map the endpoint:

```csharp
using Solentik.Paystack.AspNetCore.DependencyInjection;
using Solentik.Paystack.AspNetCore.Routing;
using Solentik.Paystack.AspNetCore.Webhooks;

builder.Services.AddPaystackWebhooks(builder.Configuration);

builder.Services.AddPaystackWebhookHandler<
    PaymentSuccess,
    PaymentSuccessHandler>();

var app = builder.Build();
app.MapPaystackWebhook();
app.Run();
```

The overload above binds `Paystack:Webhooks` from `appsettings.json`. The default endpoint is `POST /paystack/webhook`. You can alternatively configure options in code with `AddPaystackWebhooks(options => ...)`. An explicit route can override either configured value:

```csharp
app.MapPaystackWebhook("/another/paystack/endpoint");
```

### Application path and Dashboard URL

`Path` is the route inside the ASP.NET Core application, not a complete public URL. If the configured path is `/integrations/paystack/webhook` and the production domain is `https://api.example.com`, enter this complete URL in the Paystack Dashboard:

```text
https://api.example.com/integrations/paystack/webhook
```

Keeping the hostname out of application routing allows local, staging, and production deployments to use different domains with the same route. Include any externally visible reverse-proxy path base in the Dashboard URL.

### Maximum body size

`MaximumBodySize` is the largest webhook request body the endpoint will accept, measured in bytes. The default `524288` bytes equals 512 KiB. The package must temporarily buffer the exact raw bytes to calculate and verify Paystack's HMAC-SHA512 signature before parsing JSON, so this limit prevents oversized requests from consuming unbounded memory. Requests above the limit return HTTP 413. Normal Paystack webhook payloads should be much smaller than 512 KiB.

Create a handler:

```csharp
public sealed class PaymentSuccessHandler
    : IPaystackWebhookHandler<PaymentSuccess>
{
    public Task HandleAsync(
        PaymentSuccess webhookEvent,
        CancellationToken cancellationToken = default)
    {
        var reference = webhookEvent.Data
            .GetProperty("reference")
            .GetString();

        // Verify business state and fulfil the order idempotently.
        return Task.CompletedTask;
    }
}
```

Supported typed events include payment success, subscription lifecycle, invoice lifecycle, and disputes. Generic `WebhookReceived` and `WebhookHandled` handlers are also available.

Webhook requests are verified against the exact raw body using HMAC-SHA512 and a timing-safe comparison. Invalid signatures are rejected before dispatch.

## Errors

Unsuccessful Paystack responses throw `PaystackException`, which exposes:

- HTTP status code
- Paystack error type
- Paystack error code
- Paystack metadata

## Development

To run the complete test matrix:

```bash
dotnet test Solentik.Paystack.slnx -c Release
```

## License

Licensed under the MIT License.
