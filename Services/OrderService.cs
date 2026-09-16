using SmartAutoCashSystem.Data.Repositories;
using SmartAutoCashSystem.Models;

namespace SmartAutoCashSystem.Services;

public class OrderService
{
    private readonly OrderRepository _orders;
    private readonly ProductRepository _products;

    public OrderService(OrderRepository orders, ProductRepository products)
    {
        _orders = orders;
        _products = products;
    }

    public async Task<int> CreateAsync(Order order)
    {
        foreach (var item in order.Items)
        {
            var product = item.ProductID > 0
                ? await _products.GetByIdAsync(item.ProductID)
                : await _products.GetByBarcodeAsync(item.Product?.Barcode ?? string.Empty);
            if (product is null)
                throw new InvalidOperationException("Produit introuvable");
            item.ProductID = product.ProductID;
            item.UnitPrice = product.Price;
            item.SubTotal = product.Price * item.Quantity;
        }

        return await _orders.CreateAsync(order);
    }

    public Task UpdateStatusAsync(int orderId, string status) => _orders.UpdateStatusAsync(orderId, status);

    public Task<Order?> GetAsync(int orderId) => _orders.GetWithItemsAsync(orderId);
}

