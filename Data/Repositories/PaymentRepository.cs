using MySqlConnector;
using SmartAutoCashSystem.Models;
using SmartAutoCashSystem.Utils;

namespace SmartAutoCashSystem.Data.Repositories;

public class PaymentRepository
{
    public async Task<int> CreateAsync(Payment payment)
    {
        const string sql = """
            INSERT INTO Payments (OrderID, PaymentMethod, Amount, Status)
            VALUES (@orderId, @method, @amount, @status);
            SELECT LAST_INSERT_ID();
        """;

        await using var conn = Db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@orderId", payment.OrderID);
        cmd.Parameters.AddWithValue("@method", payment.PaymentMethod);
        cmd.Parameters.AddWithValue("@amount", payment.Amount);
        cmd.Parameters.AddWithValue("@status", payment.Status);
        var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return id;
    }
}

