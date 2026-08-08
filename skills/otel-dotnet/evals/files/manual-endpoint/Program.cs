using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var checkoutSource = new ActivitySource("eShop.Checkout");

app.MapPost("/checkout/{orderId}", (string orderId) =>
{
    using var activity = checkoutSource.StartActivity(
        "POST /checkout/{orderId}",
        ActivityKind.Server);
    activity?.SetTag("order.id", orderId);

    var total = CalculateTotal(orderId);
    return Results.Ok(new { orderId, total });
});

app.Run();

static decimal CalculateTotal(string orderId) => orderId.Length * 12.5m;
