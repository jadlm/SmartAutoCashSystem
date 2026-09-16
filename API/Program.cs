using SmartAutoCashSystem.Data.Repositories;
using SmartAutoCashSystem.Models;
using SmartAutoCashSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AuthRepository>();
builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddSingleton<OrderRepository>();
builder.Services.AddSingleton<PaymentRepository>();

builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<PaymentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/auth/login", async (AuthService auth, LoginRequest req) =>
{
    var user = await auth.LoginAsync(req.Email, req.Password);
    return user is null ? Results.Unauthorized() : Results.Ok(new { user.UserID, user.FullName, user.Email, Role = user.Role?.RoleName });
});

app.MapGet("/products", async (ProductService svc) => Results.Ok(await svc.GetActiveAsync()));

app.MapPost("/orders", async (OrderService svc, CreateOrderRequest req) =>
{
    var order = new Order
    {
        CreatedByUserID = req.CreatedByUserID,
        Status = "Created",
        Items = req.Items.Select(i => new OrderItem
        {
            ProductID = i.ProductID,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            SubTotal = i.UnitPrice * i.Quantity
        }).ToList()
    };

    var id = await svc.CreateAsync(order);
    return Results.Ok(new { OrderID = id });
});

app.MapPost("/orders/{orderId:int}/status", async (OrderService svc, int orderId, UpdateStatusRequest req) =>
{
    await svc.UpdateStatusAsync(orderId, req.Status);
    return Results.NoContent();
});

app.MapPost("/orders/{orderId:int}/pay", async (PaymentService svc, int orderId, PayRequest req) =>
{
    var id = await svc.PayAsync(orderId, req.Amount, req.PaymentMethod);
    return Results.Ok(new { PaymentID = id });
});

// Health/root endpoint for quick check
app.MapGet("/", () => Results.Ok(new
{
    Service = "SmartAutoCashSystem.API",
    Status = "Running",
    Swagger = "/swagger"
}));

app.Run();

public record LoginRequest(string Email, string Password);

public record CreateOrderRequest(int CreatedByUserID, List<CreateOrderItem> Items);

public record CreateOrderItem(int ProductID, int Quantity, decimal UnitPrice);

public record UpdateStatusRequest(string Status);

public record PayRequest(decimal Amount, string PaymentMethod);

