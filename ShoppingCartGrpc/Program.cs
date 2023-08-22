using ShoppingCartGrpc.Data;
using Microsoft.EntityFrameworkCore;
using ShoppingCartGrpc.Services;
using DiscountGrpc.Protos;
using static System.Formats.Asn1.AsnWriter;
using ShoppingCartGrpc.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

//configuration
IConfiguration configuration = builder.Configuration;

//Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o => o.Address = new Uri(configuration.GetValue<string>("GrpcConfigs:DiscountUrl")));
builder.Services.AddScoped<DiscountService>();
builder.Services.AddDbContext<ShoppingCartContext>();
//builder.Services.AddDbContext<ShoppingCartContext>(options => options.UseInMemoryDatabase("ShoppingCart"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ShoppingCartService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

//TruncateTable();

app.Run();

async void TruncateTable()
{
    using var scope = app.Services.CreateScope();

    try
    {
        var scopedContext = scope.ServiceProvider.GetRequiredService<ShoppingCartContext>();

        var _logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var length = await scopedContext.ShoppingCart.CountAsync();

        if (length > 100)
        {
            //var itemsToDelete = _productsContext.Products.Where(x => x.ProductId > 184);

            var itemsToDelete = scopedContext.ShoppingCart.Take(1000);

            //var itemsToDelete = new List<Product>();

            scopedContext.ShoppingCart.RemoveRange(itemsToDelete);

            await scopedContext.SaveChangesAsync();

            _logger.LogInformation("Truncated...");

        }
    }catch(Exception) 
    {
        throw;
    }

}

void SeedDatabase()
{
    using var scope = app.Services.CreateScope();

    try
    {
        var scopedContext = scope.ServiceProvider.GetRequiredService<ShoppingCartContext>();
        Seeder.Initialize(scopedContext);
    }
    catch
    {
        throw;
    }
}

public static class Seeder
{
    public static void Initialize(ShoppingCartContext context)
    {
        context.Database.EnsureCreated();
        ShoppingCartContextSeed.SeedAsync(context);
        context.SaveChanges();
    }
}

//gRPC only works with https protocol
//ProductServer 7212
//DiscountServer 7169
//ShoppingCartServer 7093