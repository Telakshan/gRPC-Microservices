using Grpc.Core;
using Grpc.Net.Client;
using ProductGrpc.Protos;
using ShoppingCartGrpc.Protos;

namespace ShoppingCartWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            using var shoppingCartChannel = GrpcChannel
                .ForAddress(_configuration.GetValue<string>("WorkerService:ShoppingCartServerUrl") ?? throw new ArgumentNullException("ShoppingCartServerUrl Not found!"));

            var shoppingCartClient = new ShoppingCartProtoService.ShoppingCartProtoServiceClient(shoppingCartChannel);

            //Get or create a shopping cart
            var shoppingCartModel = await GetOrCreateShoppingCart(shoppingCartClient);

            //Open client stream
            using var scClientStream = shoppingCartClient.AddItemIntoShoppingCart();

            //ProductGrpc channel
            using var productChannel = GrpcChannel.ForAddress(_configuration.GetValue<string>("WorkerService:ProductServiceUrl") ?? throw new ArgumentNullException("ProductServerUrl Not found!"));

            //ProductGrpc client
            var productClient = new ProductProtoService.ProductProtoServiceClient(productChannel);

            _logger.LogInformation("GetAllProducts started");

            //GetAllProducts stream started
            using var clientData = productClient.GetAllProducts(new GetAllProductsRequest());

            await foreach (var responseData in clientData.ResponseStream.ReadAllAsync()) 
            {
                _logger.LogInformation("GetAllProducts Stream Response: {responseData}", responseData);

                //For every product from responseData, instantiate an shoppingcart item.
                var addNewScItem = new AddItemIntoShoppingCartRequest
                {
                    Username = _configuration.GetValue<string>("WorkerService:UserName"),
                    DiscountCode = "CODE_100",
                    NewCartItem = new ShoppingCartItemModel
                    {
                        ProductId = responseData.ProductId,
                        Productname = responseData.Name,
                        Price = responseData.Price,
                        Color = "Black",
                        Quantity = 1
                    }
                };

                //add the items into shopping cart using client stream
                await scClientStream.RequestStream.WriteAsync(addNewScItem);

                _logger.LogInformation("ShoppingCart Client stream added new items: {addNewScItem}", addNewScItem);
            }

            //Close the stream
            await scClientStream.RequestStream.CompleteAsync();

            var addItemIntoShoppingCartResponse = await scClientStream;

            _logger.LogInformation("AddItemIntoShoppingCartResponse: {addItemIntoShoppingCartResponse}", addItemIntoShoppingCartResponse);

            await Task.Delay(_configuration.GetValue<int>("WorkerService:TaskInterval"), stoppingToken);
        }
    }

    public async Task<ShoppingCartModel> GetOrCreateShoppingCart(ShoppingCartProtoService.ShoppingCartProtoServiceClient shoppingCartClient)
    {
        ShoppingCartModel shoppingCartModel;

        try
        {
            _logger.LogInformation("GetShoppingCartAsync started");

            shoppingCartModel = await shoppingCartClient.GetShoppingCartAsync(new GetShoppingCartRequest { Username = _configuration.GetValue<string>("WorkerService:UserName") });
            
            _logger.LogInformation("GetShoppingCartAsync Response: {shoppingCartModel}", shoppingCartModel);
        }
        catch (RpcException exception)
        {
            if (exception.StatusCode == StatusCode.NotFound)
            {
                _logger.LogInformation("CreateShoppingCartAsync started");

                shoppingCartModel = await shoppingCartClient.CreateShoppingCartAsync(new ShoppingCartModel { Username = _configuration.GetValue<string>("WorkerService:UserName") });

                _logger.LogInformation("CreateShoppingCartAsync response: {shoppingCartModel}", shoppingCartModel);
            }
            else
            {
                throw exception;
            }
        }

        return shoppingCartModel;

    }
}

