using DiscountGrpc.Protos;

namespace ShoppingCartGrpc.Services;

public class DiscountService
{ 
    private readonly DiscountProtoService.DiscountProtoServiceClient _client;
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(DiscountProtoService.DiscountProtoServiceClient client, ILogger<DiscountService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<DiscountModel> GetDiscount(string discountCode)
    {
        var discountRequest = new GetDiscountRequest { DiscountCode = discountCode };

        _logger.LogInformation("GetDiscountRequest from ShoppingCartService: {discountRequest}", discountRequest);

        return await _client.GetDiscountAsync(discountRequest);
    }
}
