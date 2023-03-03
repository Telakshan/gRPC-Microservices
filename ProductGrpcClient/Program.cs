using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ProductGrpc.Protos;

namespace ProductGrpcClient;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Greetings gRPC");

        Thread.Sleep(2000);

        using var channel = GrpcChannel.ForAddress("https://localhost:7212");

        var client = new ProductProtoService.ProductProtoServiceClient(channel);

/*        await GetProductAsync(client);

        await GetAllProducts(client);*/

        await AddProduct(client);

        //using (var clientData = client.GetAllProducts(new GetAllProductsRequest()))
        //{
        //    while (await clientData.ResponseStream.MoveNext(new CancellationToken()))
        //    {
        //        var currentProduct = clientData.ResponseStream.Current;
        //        Console.WriteLine(currentProduct);
        //    }
        //}

    }

    private static async Task GetAllProducts(ProductProtoService.ProductProtoServiceClient client)
    {
        Console.WriteLine("\nGetAllProducts foreach...");

        using var clientData = client.GetAllProducts(new GetAllProductsRequest());

        await foreach (var responseData in clientData.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(responseData);
        }

        Console.ReadLine();

    }

    private static async Task GetProductAsync(ProductProtoService.ProductProtoServiceClient client)
    {
        Console.WriteLine("GetProductAsync started...");

        var response = await client.GetProductAsync(new GetProductRequest
        {
            ProductId = 2
        });

        Console.WriteLine("ProductModel: " + response.ToString());
    }


    private static async Task AddProduct(ProductProtoService.ProductProtoServiceClient client)
    {
        Console.WriteLine("Add Product started...");

/*        var response = await client.AddProductAsync(
            new AddProductRequest
            {
                Product = new ProductModel
                {
                    Name = "Red",
                    Description = "New Red Phone Mi10T",
                    Price = 699,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            });

        Console.WriteLine("ProductModel: " + response.ToString());*/

        //await GetAllProducts(client);

    }



}