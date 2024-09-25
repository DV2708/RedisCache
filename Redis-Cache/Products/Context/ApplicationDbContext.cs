using Microsoft.EntityFrameworkCore;
using Products.Model;

namespace Products.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<ConsumerProduct> Products { get; set; }
}
