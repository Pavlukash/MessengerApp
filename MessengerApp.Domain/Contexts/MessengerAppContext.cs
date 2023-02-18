using MessengerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessengerApp.Domain.Contexts;

public sealed class MessengerAppContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; } = null!;

    public MessengerAppContext(DbContextOptions<MessengerAppContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}