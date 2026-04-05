using Microsoft.EntityFrameworkCore;

namespace Server;

public class CustomerDbContext(DbContextOptions<CustomerDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; } = null!;
}

