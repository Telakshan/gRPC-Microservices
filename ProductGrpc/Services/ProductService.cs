using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ProductsContext productsContext, IMapper mapper, ILogger<ProductService> logger)
    {
        _productsContext = productsContext;
        _logger = logger;
        _mapper = mapper;
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
            throw new RpcException(new Status(StatusCode.NotFound, $"Product with ID={request.ProductId} not found"));
        }

        var productModel = _mapper.Map<ProductModel>(product);

        return productModel;
    }

    public override async Task GetAllProducts(GetAllProductsRequest request, IServerStreamWriter<ProductModel> responseStream, ServerCallContext context)
    {
        
        var productList = await _productsContext.Product.ToListAsync();

        if (productList == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Product list is null"));
        }

        foreach (var product in productList)
        {
            var productModel = _mapper.Map<ProductModel>(product);

            await responseStream.WriteAsync(productModel);
        }

    }

    public override async Task<ProductModel> AddProduct(AddProductRequest request, ServerCallContext context)
    {

        var product = _mapper.Map<Product>(request.Product);

        _productsContext.Product.Add(product);
        await _productsContext.SaveChangesAsync();

        return _mapper.Map<ProductModel>(product);

    }

    public override async Task<ProductModel> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
    {
        var product = _mapper.Map<Product>(request.Product);

        var productExists = await _productsContext.Product.AnyAsync(p => p.ProductId == product.ProductId);

        if (!productExists)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Product with ID={request.Product.ProductId} not found"));
        }

        _productsContext.Entry(product).State = EntityState.Modified;

        try
        {
            await _productsContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }

        return _mapper.Map<ProductModel>(product);
    }

    public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
    {
        var product = await _productsContext.Product.FindAsync(request.ProductId);

        if (product == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Product with ID={request.ProductId} not found"));
        }

        _productsContext.Product.Remove(product);

        var deleteCount = await _productsContext.SaveChangesAsync();

        return new DeleteProductResponse
        {
            Success = deleteCount > 0
        };
    }

    public override async Task<InsertBulkProductResponse> InsertBulkProduct(IAsyncStreamReader<ProductModel> requestStream, ServerCallContext context)
    {
        while(await requestStream.MoveNext())
        {
            var product = _mapper.Map<Product>(requestStream.Current);
            _productsContext.Product.Add(product);
        }

        var insertCount = await _productsContext.SaveChangesAsync();

        var response = new InsertBulkProductResponse
        {
            Success = insertCount > 0,
            InsertCount = insertCount
        };

        return response;
    }

}
