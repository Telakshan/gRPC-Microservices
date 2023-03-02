using Grpc.Net.Client;
using ProductGrpc.Protos;
using System.ComponentModel.DataAnnotations;

namespace ProductGrpcClient;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Greetings gRPC");

        Thread.Sleep(2000);

        using var channel = GrpcChannel.ForAddress("https://localhost:7212");

        var client = new ProductProtoService.ProductProtoServiceClient(channel);


        var response = await client.GetProductAsync(new GetProductRequest
        {
            ProductId = 2
        }); 

        Console.WriteLine("ProductModel: " + response.ToString());

        Console.WriteLine("\nGetAllProductsAsync...");


        using (var clientData = client.GetAllProducts(new GetAllProductsRequest()))
        {
            while (await clientData.ResponseStream.MoveNext(new System.Threading.CancellationToken()))
            {
                var currentProduct = clientData.ResponseStream.Current;
                Console.WriteLine(currentProduct);
            }
        }

        Console.ReadLine();
    }
}