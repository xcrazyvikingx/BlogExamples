using Microsoft.EntityFrameworkCore;
using MultitenancyExample.EntityModels.Shared;

namespace MultitenancyExample.Contexts;

public class SharedContext : DbContext
{

    public DbSet<Document> Documents { get; set; }
    public SharedContext(DbContextOptions<SharedContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}