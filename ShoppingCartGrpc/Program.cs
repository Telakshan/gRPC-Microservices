using DiscountGrpc.Protos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartGrpc.Data;
using ShoppingCartGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

//configuration
IConfiguration configuration = builder.Configuration;

//Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o => o.Address = new Uri(configuration.GetValue<string>("GrpcConfigs:DiscountUrl")));
builder.Services.AddScoped<DiscountService>();
builder.Services.AddDbContext<ShoppingCartContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddDbContext<ShoppingCartContext>(options => options.UseInMemoryDatabase("ShoppingCart"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = configuration.GetValue<string>("GrpcConfigs:IdentityServer");
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
        };
    });

builder.Services.AddMvc();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ShoppingCartContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<ShoppingCartService>();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

TruncateTable();

app.Run();

#region Ignore
//Ignore

async void TruncateTable()
{
    using var scope = app.Services.CreateScope();

    try
    {
        var scopedContext = scope.ServiceProvider.GetRequiredService<ShoppingCartContext>();

        var _logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var length = await scopedContext.ShoppingCart.CountAsync();

        if (length == 100)
        {
            //var itemsToDelete = _productsContext.Products.Where(x => x.ProductId > 184);

            var itemsToDelete = scopedContext.ShoppingCart.Take(100);

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
    using var scope = app?.Services.CreateScope();

    try
    {
        var scopedContext = scope?.ServiceProvider.GetRequiredService<ShoppingCartContext>();
        Seeder.Initialize(scopedContext ?? throw new ArgumentNullException());
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

#endregion

//gRPC only works with https protocol
//ProductServer 7212
//DiscountServer 7169
//ShoppingCartServer 7093