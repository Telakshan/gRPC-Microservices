using Microsoft.EntityFrameworkCore;
using ShoppingCartGrpc.Models;

namespace ShoppingCartGrpc.Data;

public class ShoppingCartContext : DbContext
{

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = Products"
        );
    }

    public DbSet<ShoppingCart> ShoppingCart { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }

}

