using Microsoft.EntityFrameworkCore;
using ProductGrpc.Data;
using ProductGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(opt => opt.EnableDetailedErrors = true);
//builder.Services.AddDbContext<ProductsContext>(options => options.UseInMemoryDatabase("Products"));
builder.Services.AddDbContext<ProductsContext>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

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