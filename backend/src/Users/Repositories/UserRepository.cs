using LoginApi.Data;
using LoginApi.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Users.Repositories;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task UpdateAsync(User user);
    Task AddRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> FindRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(RefreshToken token);
}

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> FindByEmailAsync(string email) =>
        db.Users.Include(user => user.RefreshTokens)
            .FirstOrDefaultAsync(user => user.Email == email.Trim().ToLowerInvariant());

    public async Task UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        db.Users.Update(user);
        await db.SaveChangesAsync();
    }

    public async Task AddRefreshTokenAsync(RefreshToken token)
    {
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync();
    }

    public Task<RefreshToken?> FindRefreshTokenAsync(string token) =>
        db.RefreshTokens.Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(refreshToken => refreshToken.Token == token);

    public async Task RevokeRefreshTokenAsync(RefreshToken token)
    {
        token.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }
}
