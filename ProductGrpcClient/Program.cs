﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ProductGrpc.Protos;

namespace ProductGrpcClient;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Greetings gRPC");

        //Thread.Sleep(2000);

        using var channel = GrpcChannel.ForAddress("https://localhost:7212");

        var client = new ProductProtoService.ProductProtoServiceClient(channel);

        //await GetProductAsync(client);

        //await AddProductAsync(client);

        //await UpdateProductAsync(client);

        //await GetAllProducts(client);

        //await InsertBulkProductAsync(client);

        //await DeleteProductAsync(client);

        Console.ReadLine();

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

    }

    private static async Task GetProductAsync(ProductProtoService.ProductProtoServiceClient client)
    {
        Console.WriteLine("\nGetProductAsync started...");

        var response = await client.GetProductAsync(new GetProductRequest
        {
            ProductId = 2
        });

        Console.WriteLine("ProductModel: " + response.ToString());
    }


    private static async Task AddProductAsync(ProductProtoService.ProductProtoServiceClient client)
    {
        Console.WriteLine("\nAdd Product started...");

        var response = await client.AddProductAsync(
            new AddProductRequest
            {
                Product = new ProductModel
                {
                    Name = "Note 23",
                    Description = "New Samsung Note",
                    Price = 699,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            });

        Console.WriteLine("ProductModel: " + response.ToString());

    }

    private static async Task DeleteProductAsync(ProductProtoService.ProductProtoServiceClient client)
    {
        Console.WriteLine("\nDelete Product started...");

        var response = await client.DeleteProductAsync(
            new DeleteProductRequest
            {
                ProductId = 5
            });

        Console.WriteLine("DeleteProductResponse: " + response.ToString());

    }

    private static async Task UpdateProductAsync(ProductProtoService.ProductProtoServiceClient client)
    {
        Console.WriteLine("\nUpdateProductAsync Product started...");

        var response = await client.UpdateProductAsync(
            new UpdateProductRequest
            {
                Product = new ProductModel
                {
                    ProductId = 3,
                    Name = "Samsung S23+",
                    Description = "New Samsung phone",
                    Price = 699,
                    Status = ProductStatus.Instock,
                    CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            });

        Console.WriteLine("UpdateProductResponse: " + response.ToString());
    }

    private static async Task InsertBulkProductAsync(ProductProtoService.ProductProtoServiceClient client)
    {
        Console.WriteLine("\nInsertBulkProductAsync Product started...");

        using var clientBulk = client.InsertBulkProduct();

        for (int i = 0; i < 3; i++)
        {
            var productModel = new ProductModel
            {
                Name = $"Product{i}",
                Description = "Bulk inserted product",
                Price = 399,
                Status = ProductStatus.Instock,
                CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            await clientBulk.RequestStream.WriteAsync(productModel);
        }

        await clientBulk.RequestStream.CompleteAsync();
        var responseBulk = await clientBulk;
        Console.WriteLine($"Status: {responseBulk.Success}. Insert Count: {responseBulk.InsertCount}");
    }

}