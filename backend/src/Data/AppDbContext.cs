using LoginApi.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Email).HasMaxLength(254).IsRequired();
            entity.Property(user => user.PasswordHash).IsRequired();
            entity.Property(user => user.Status).HasConversion<string>();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(token => token.Id);
            entity.HasIndex(token => token.Token).IsUnique();
            entity.Property(token => token.Token).IsRequired();
            entity.HasOne(token => token.User)
                  .WithMany(user => user.RefreshTokens)
                  .HasForeignKey(token => token.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
