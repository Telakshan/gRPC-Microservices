using Grpc.Net.Client;
using ProductGrpc.Protos;

namespace ProductWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly ProductFactory _productFactory;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, ProductFactory productFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _productFactory = productFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //await Task.Delay(3000);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                using var channel = GrpcChannel.ForAddress(_configuration.GetValue<string>("WorkerService:ServerUrl") ?? throw new ArgumentNullException("ProductServerUrl Not found!"));
                var client = new ProductProtoService.ProductProtoServiceClient(channel);

                _logger.LogInformation("AddProduct started...");

                var response = await client.AddProductAsync(await _productFactory.Generate());

                Console.WriteLine("AddProductAsync response" + response.ToString());

                await Task.Delay(_configuration.GetValue<int>("WorkerService:TaskInterval"), stoppingToken);
            }
        }
    }

}
