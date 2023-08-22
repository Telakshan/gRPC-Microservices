using DiscountGrpc.Model;
using Microsoft.EntityFrameworkCore;

namespace DiscountGrpc.Data;

public class DiscountContext: DbContext
{
    public DbSet<Discount> Discount { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {


        optionsBuilder.UseSqlServer(
            "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = Products"
            );

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        List<Discount> Discounts = new List<Discount>
        {
            new Discount{ DiscountId = 1, Code = "CODE_100", Amount = 100 },
            new Discount{ DiscountId = 2, Code = "CODE_200", Amount = 200 },
            new Discount{ DiscountId = 3, Code = "CODE_300", Amount = 300 }
        };
        
        modelBuilder.Entity<Discount>().HasData( Discounts );

    }
}