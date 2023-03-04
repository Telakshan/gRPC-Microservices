using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ShoppingCartGrpc.Data;
using ShoppingCartGrpc.Models;
using ShoppingCartGrpc.Protos;

namespace ShoppingCartGrpc.Services;

public class ShoppingCartService: ShoppingCartProtoService.ShoppingCartProtoServiceBase
{
    private readonly ShoppingCartContext _shoppingCartContext;
    private readonly IMapper _mapper;
    private readonly ILogger<ShoppingCartService> _logger;
    public ShoppingCartService(ShoppingCartContext shoppingCartContext, IMapper mapper, ILogger<ShoppingCartService> logger)
    {
        _shoppingCartContext = shoppingCartContext;
        _mapper = mapper;
        _logger = logger;
    }

    public override async Task<ShoppingCartModel> GetShoppingCart(GetShoppingCartRequest request, ServerCallContext context)
    {
        var shoppingCart = await _shoppingCartContext.ShoppingCart.FirstOrDefaultAsync(s => s.UserName == request.Username);

        if (shoppingCart == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Shopping Cart with username={request.Username} not found"));
        }

        return _mapper.Map<ShoppingCartModel>(shoppingCart);

    }

    public override Task<AddItemIntoShoppingCartResponse> AddItemIntoShoppingCart(IAsyncStreamReader<AddItemIntoShoppingCartRequest> requestStream, ServerCallContext context)
    {
        return base.AddItemIntoShoppingCart(requestStream, context);
    }

    public override async Task<RemoveItemIntoShoppingCartResponse> RemoveItemIntoShoppingCart(RemoveItemIntoShoppingCartRequest request, ServerCallContext context)
    {
        var shoppingCart = await _shoppingCartContext.ShoppingCart.FirstOrDefaultAsync(s => s.UserName == request.Username);

        if (shoppingCart == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName={request.Username} is not found."));
        }

        var itemToBeRemoved = shoppingCart.Items.FirstOrDefault(i => i.ProductId == request.RemoveCartItem.ProductId);

        if (itemToBeRemoved == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"CartItem with ProductId={request.RemoveCartItem.ProductId} is not found in the ShoppingCart."));
        }

        shoppingCart.Items.Remove(itemToBeRemoved);
        var removeCount = await _shoppingCartContext.SaveChangesAsync();

        return new RemoveItemIntoShoppingCartResponse
        {
            Success = removeCount > 0
        };

    }

    public override async Task<ShoppingCartModel> CreateShoppingCart(ShoppingCartModel request, ServerCallContext context)
    {
        var shoppingCart = _mapper.Map<ShoppingCart>(request);

        var cartExists = await _shoppingCartContext.ShoppingCart.AnyAsync(s => s.UserName == shoppingCart.UserName);

        if (cartExists)
        {
            _logger.LogError("Invalid UserName for ShoppingCart creation. UserName : {userName}", shoppingCart.UserName);
            throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName={request.Username} is already exist."));
        }

        _shoppingCartContext.ShoppingCart.Add(shoppingCart);
        await _shoppingCartContext.SaveChangesAsync();

        _logger.LogInformation("ShoppingCart is successfully created.UserName : {userName}", shoppingCart.UserName);

        return _mapper.Map<ShoppingCartModel>(shoppingCart);

    }
}
