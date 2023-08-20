using ShoppingCartGrpc.Data;
using Microsoft.EntityFrameworkCore;
using ShoppingCartGrpc.Services;
using DiscountGrpc.Protos;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

//configuration
IConfiguration configuration = builder.Configuration;

//Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o => o.Address = new Uri(configuration.GetValue<string>("GrpcConfigs:DiscountUrl")));
builder.Services.AddScoped<DiscountService>();
builder.Services.AddDbContext<ShoppingCartContext>(options => options.UseInMemoryDatabase("ShoppingCart"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ShoppingCartService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

SeedDatabase();

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

app.Run();

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