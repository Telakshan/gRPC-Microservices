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

    public override async Task<AddItemIntoShoppingCartResponse> AddItemIntoShoppingCart(IAsyncStreamReader<AddItemIntoShoppingCartRequest> requestStream, ServerCallContext context)
    {

        while(await requestStream.MoveNext())
        {
            var shoppingCart = await _shoppingCartContext.ShoppingCart.FirstOrDefaultAsync(s => s.UserName == requestStream.Current.Username);

            if (shoppingCart == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName={requestStream.Current.Username} is not found."));
            }

            var newAddedCartItem = _mapper.Map<ShoppingCartItem>(requestStream.Current.NewCartItem);
            var cartItem = shoppingCart.Items.FirstOrDefault(i => i.ProductId == newAddedCartItem.ProductId);  
            
            if(cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                float discount = 100;
                newAddedCartItem.Price -= discount;
                shoppingCart.Items.Add(newAddedCartItem);
            }
        }

        var insertCount = await _shoppingCartContext.SaveChangesAsync();

        var response = new AddItemIntoShoppingCartResponse
        {
            Success = insertCount > 0,
            InsertCount = insertCount
        };

        return response;
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

        _logger.LogInformation($"ShoppingCartitem removed: {itemToBeRemoved.Id}");

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
            throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName={request.Username} already exist."));
        }

        _shoppingCartContext.ShoppingCart.Add(shoppingCart);
        await _shoppingCartContext.SaveChangesAsync();

        _logger.LogInformation("ShoppingCart is successfully created.UserName : {userName}", shoppingCart.UserName);

        return _mapper.Map<ShoppingCartModel>(shoppingCart);

    }
}
