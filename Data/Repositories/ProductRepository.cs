using MySqlConnector;
using SmartAutoCashSystem.Models;
using SmartAutoCashSystem.Utils;

namespace SmartAutoCashSystem.Data.Repositories;

public class ProductRepository
{
    public async Task<IReadOnlyCollection<Product>> GetActiveAsync()
    {
        const string sql = """
            SELECT ProductID, Name, Barcode, Price, Stock, CategoryID, IsActive
            FROM Products
            WHERE IsActive = 1
        """;

        await using var conn = Db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        var list = new List<Product>();
        while (await reader.ReadAsync())
        {
            list.Add(MapProduct(reader));
        }
        return list;
    }

    public async Task<Product?> GetByIdAsync(int productId)
    {
        const string sql = """
            SELECT ProductID, Name, Barcode, Price, Stock, CategoryID, IsActive
            FROM Products
            WHERE ProductID = @id
            LIMIT 1
        """;

        await using var conn = Db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", productId);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return MapProduct(reader);
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        const string sql = """
            SELECT ProductID, Name, Barcode, Price, Stock, CategoryID, IsActive
            FROM Products
            WHERE Barcode = @barcode AND IsActive = 1
            LIMIT 1
        """;

        await using var conn = Db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@barcode", barcode);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return MapProduct(reader);
    }

    public async Task UpdateStockAsync(int productId, int newStock)
    {
        const string sql = "UPDATE Products SET Stock = @stock WHERE ProductID = @id";

        await using var conn = Db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@stock", newStock);
        cmd.Parameters.AddWithValue("@id", productId);
        await cmd.ExecuteNonQueryAsync();
    }

    private static Product MapProduct(MySqlDataReader reader)
    {
        var ordProductId = reader.GetOrdinal("ProductID");
        var ordName = reader.GetOrdinal("Name");
        var ordBarcode = reader.GetOrdinal("Barcode");
        var ordPrice = reader.GetOrdinal("Price");
        var ordStock = reader.GetOrdinal("Stock");
        var ordCategoryId = reader.GetOrdinal("CategoryID");
        var ordIsActive = reader.GetOrdinal("IsActive");

        return new Product
        {
            ProductID = reader.GetInt32(ordProductId),
            Name = reader.GetString(ordName),
            Barcode = reader.GetString(ordBarcode),
            Price = reader.GetDecimal(ordPrice),
            Stock = reader.GetInt32(ordStock),
            CategoryID = reader.IsDBNull(ordCategoryId) ? null : reader.GetInt32(ordCategoryId),
            IsActive = reader.GetBoolean(ordIsActive)
        };
    }
}

