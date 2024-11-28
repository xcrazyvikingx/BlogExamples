using Microsoft.EntityFrameworkCore;
using MultitenancyExample.EntityModels.Main;

namespace MultitenancyExample.Contexts;

public class MainContext : DbContext
{
    public MainContext(DbContextOptions<MainContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<UserCustomer> UserCustomers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserCustomer>()
            .HasKey(uc => new { uc.UserId, uc.CustomerId });
    }
}
