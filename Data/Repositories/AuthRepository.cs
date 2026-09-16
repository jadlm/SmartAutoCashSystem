using MySqlConnector;
using SmartAutoCashSystem.Models;
using SmartAutoCashSystem.Utils;

namespace SmartAutoCashSystem.Data.Repositories;

public class AuthRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = """
            SELECT u.UserID, u.FullName, u.Email, u.PasswordHash, u.RoleID, u.CreatedAt,
                   r.RoleID AS RId, r.RoleName
            FROM Users u
            JOIN Roles r ON r.RoleID = u.RoleID
            WHERE u.Email = @email
        """;

        await using var conn = Db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@email", email);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return new User
        {
            UserID = reader.GetInt32("UserID"),
            FullName = reader.GetString("FullName"),
            Email = reader.GetString("Email"),
            PasswordHash = reader.GetString("PasswordHash"),
            RoleID = reader.GetInt32("RoleID"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            Role = new Role
            {
                RoleID = reader.GetInt32("RId"),
                RoleName = reader.GetString("RoleName")
            }
        };
    }
}

