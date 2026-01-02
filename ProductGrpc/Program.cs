using Microsoft.EntityFrameworkCore;
using ProductGrpc.Data;
using ProductGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(opt => opt.EnableDetailedErrors = true);
//builder.Services.AddDbContext<ProductsContext>(options => options.UseInMemoryDatabase("Products"));
builder.Services.AddDbContext<ProductsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ProductsContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<ProductService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

//SeedDatabase();

/*void SeedDatabase()
{
    using var scope = app.Services.CreateScope();
        
        try
        {
            var scopedContext = scope.ServiceProvider.GetRequiredService<ProductsContext>();
            Seeder.Initialize(scopedContext);
        }
        catch
        {
            throw;
        }
}*/

TruncateTable();

async void TruncateTable()
{
    using var scope = app.Services.CreateScope();

    try
    {
        var scopedContext = scope.ServiceProvider.GetRequiredService<ProductsContext>();

        var length = await scopedContext.Products.CountAsync();

        var itemsToDelete =  scopedContext.Products.Where(x => x.ProductId > 7);

        scopedContext.Products.RemoveRange(itemsToDelete);

        scopedContext.SaveChanges();

    }catch (Exception) 
    { 
        throw;
    }
}

app.Run();

/*public static class Seeder
{
    public static void Initialize(ProductsContext context)
    {
        context.Database.EnsureCreated();
        ProductsContextSeed.SeedAsync(context);
        context.SaveChanges();
    }
}*/