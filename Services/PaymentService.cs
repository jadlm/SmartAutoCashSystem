using SmartAutoCashSystem.Data.Repositories;
using SmartAutoCashSystem.Models;

namespace SmartAutoCashSystem.Services;

public class PaymentService
{
    private readonly PaymentRepository _payments;
    private readonly OrderRepository _orders;
    private readonly ProductRepository _products;

    public PaymentService(PaymentRepository payments, OrderRepository orders, ProductRepository products)
    {
        _payments = payments;
        _orders = orders;
        _products = products;
    }

    public async Task<int> PayAsync(int orderId, decimal amount, string method)
    {
        var order = await _orders.GetWithItemsAsync(orderId) ?? throw new InvalidOperationException("Commande introuvable");
        var total = order.Items.Sum(i => i.SubTotal);
        if (amount < total)
            throw new InvalidOperationException("Montant insuffisant");

        var payment = new Payment
        {
            OrderID = orderId,
            Amount = amount,
            PaymentMethod = method,
            Status = "Paid",
            PaymentDate = DateTime.UtcNow
        };

        var paymentId = await _payments.CreateAsync(payment);
        await _orders.UpdateStatusAsync(orderId, "Paid");

        // Décrémenter le stock
        foreach (var item in order.Items)
        {
            var newStock = Math.Max(0, item.Product!.Stock - item.Quantity);
            await _products.UpdateStockAsync(item.ProductID, newStock);
        }

        return paymentId;
    }
}

