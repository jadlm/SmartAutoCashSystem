using MySqlConnector;
using SmartAutoCashSystem.Models;
using SmartAutoCashSystem.Utils;

namespace SmartAutoCashSystem.Data.Repositories;

public class OrderRepository
{
    public async Task<int> CreateAsync(Order order)
    {
        const string insertOrder = """
            INSERT INTO Orders (Status, CreatedByUserID)
            VALUES (@status, @userId);
            SELECT LAST_INSERT_ID();
        """;

        await using var conn = Db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(insertOrder, conn);
        cmd.Parameters.AddWithValue("@status", order.Status);
        cmd.Parameters.AddWithValue("@userId", order.CreatedByUserID);
        var orderId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        if (order.Items.Count > 0)
            await InsertItemsAsync(conn, orderId, order.Items);

        return orderId;
    }

    public async Task UpdateStatusAsync(int orderId, string status)
    {
        const string sql = "UPDATE Orders SET Status = @status WHERE OrderID = @id";
        await using var conn = Db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@status", status);
        cmd.Parameters.AddWithValue("@id", orderId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<Order?> GetWithItemsAsync(int orderId)
    {
        const string orderSql = """
            SELECT OrderID, OrderDate, Status, CreatedByUserID
            FROM Orders
            WHERE OrderID = @id
        """;

        const string itemsSql = """
            SELECT oi.OrderItemID, oi.OrderID, oi.ProductID, oi.Quantity, oi.UnitPrice, oi.SubTotal,
                   p.Name, p.Barcode
            FROM OrderItems oi
            JOIN Products p ON p.ProductID = oi.ProductID
            WHERE oi.OrderID = @id
        """;

        await using var conn = Db.CreateConnection();
        await conn.OpenAsync();
        await using var orderCmd = new MySqlCommand(orderSql, conn);
        orderCmd.Parameters.AddWithValue("@id", orderId);
        await using var reader = await orderCmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        var order = new Order
        {
            OrderID = reader.GetInt32("OrderID"),
            OrderDate = reader.GetDateTime("OrderDate"),
            Status = reader.GetString("Status"),
            CreatedByUserID = reader.GetInt32("CreatedByUserID")
        };
        await reader.CloseAsync();

        await using var itemCmd = new MySqlCommand(itemsSql, conn);
        itemCmd.Parameters.AddWithValue("@id", orderId);
        await using var itemsReader = await itemCmd.ExecuteReaderAsync();
        while (await itemsReader.ReadAsync())
        {
            order.Items.Add(new OrderItem
            {
                OrderItemID = itemsReader.GetInt32("OrderItemID"),
                OrderID = orderId,
                ProductID = itemsReader.GetInt32("ProductID"),
                Quantity = itemsReader.GetInt32("Quantity"),
                UnitPrice = itemsReader.GetDecimal("UnitPrice"),
                SubTotal = itemsReader.GetDecimal("SubTotal"),
                Product = new Product
                {
                    ProductID = itemsReader.GetInt32("ProductID"),
                    Name = itemsReader.GetString("Name"),
                    Barcode = itemsReader.GetString("Barcode")
                }
            });
        }

        return order;
    }

    private static async Task InsertItemsAsync(MySqlConnection conn, int orderId, IEnumerable<OrderItem> items)
    {
        const string sql = """
            INSERT INTO OrderItems (OrderID, ProductID, Quantity, UnitPrice, SubTotal)
            VALUES (@orderId, @productId, @qty, @unit, @sub);
        """;

        foreach (var item in items)
        {
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@orderId", orderId);
            cmd.Parameters.AddWithValue("@productId", item.ProductID);
            cmd.Parameters.AddWithValue("@qty", item.Quantity);
            cmd.Parameters.AddWithValue("@unit", item.UnitPrice);
            cmd.Parameters.AddWithValue("@sub", item.SubTotal);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}

