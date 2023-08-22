namespace ProductWorkerService;

public class ProductFactory
{
    private readonly ILogger<ProductFactory> _logger;
    private readonly IConfiguration _configuration;

    public ProductFactory(ILogger<ProductFactory> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task<AddProductRequest> Generate()
    {
        var productRequest = new AddProductRequest
        {
            Product = new ProductModel
            {
                Name = GenerateRandomStrings(5),
                Description = GenerateRandomStrings(10),
                Price = 699,
                Status = ProductStatus.Instock,
                CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
            }
        };

        return Task.FromResult(productRequest);
    }

    public string GenerateRandomStrings(int length)
    {
        Random random = new Random();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return (new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray()));
    }
}

