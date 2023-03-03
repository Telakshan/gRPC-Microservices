using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ProductGrpc.Data;
using ProductGrpc.Models;
using ProductGrpc.Protos;

namespace ProductGrpc.Services;

public class ProductService: ProductProtoService.ProductProtoServiceBase
{

    private readonly ProductsContext _productsContext;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ProductsContext productsContext, ILogger<ProductService> logger)
    {
        _productsContext = productsContext;
        _logger = logger;
    }

    public override Task<Empty> Test(Empty request, ServerCallContext context)
    {
        return base.Test(request, context);
    }

    public override async Task<ProductModel> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var product = await _productsContext.Product.FindAsync(request.ProductId);

        if(product == null)
        {
            //throw rpc exception
        }

        var productModel = new ProductModel
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Status = ProductStatus.Instock,
            CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
        };

        return productModel;
    }

    public override async Task GetAllProducts(GetAllProductsRequest request, IServerStreamWriter<ProductModel> responseStream, ServerCallContext context)
    {
        
        var productList = await _productsContext.Product.ToListAsync();

        if (productList == null)
        {
            //throw rpc exception
        }

        foreach (var product in productList)
        {

            var productModel = new ProductModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Status = ProductStatus.Instock,
                CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
            };

            await responseStream.WriteAsync(productModel);
        }

    }

    public override async Task<ProductModel> AddProduct(AddProductRequest request, ServerCallContext context)
    {
        var product = new Product
        {
            ProductId = request.Product.ProductId,
            Name = request.Product.Name,
            Description = request.Product.Description,
            Price = request.Product.Price,
            Status = Product.ProductStatus.INSTOCK,
            CreatedTime = DateTime.Now
        };

        await _productsContext.Product.AddAsync(product);
        await _productsContext.SaveChangesAsync();

        return new ProductModel
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Status = ProductStatus.Instock,
            CreatedTime = Timestamp.FromDateTime(product.CreatedTime)
        };

    }



}
