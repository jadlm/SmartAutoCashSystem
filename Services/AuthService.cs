using SmartAutoCashSystem.Data.Repositories;
using SmartAutoCashSystem.Models;

namespace SmartAutoCashSystem.Services;

public class AuthService
{
    private readonly AuthRepository _repo;

    public AuthService(AuthRepository repo)
    {
        _repo = repo;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _repo.GetByEmailAsync(email);
        if (user is null)
            return null;

        var valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return valid ? user : null;
    }
}

