using MessengerApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessengerApp.Domain.Contexts;

public class MessengerAppContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; } = null!;

    public MessengerAppContext(DbContextOptions<MessengerAppContext> options)
        : base(options)
    {
    }
}