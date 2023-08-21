using Grpc.Core;
using Grpc.Net.Client;
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

            var shoppingCartModel = await GetOrCreateShoppingCart(shoppingCartClient);

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

