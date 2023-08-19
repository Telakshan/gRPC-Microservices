using AutoMapper;
using DiscountGrpc.Data;
using DiscountGrpc.Protos;
using Grpc.Core;

namespace DiscountGrpc.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly DiscountContext _discountContext;
    private readonly IMapper _mapper;
    private readonly ILogger<DiscountService> _logger;

    public DiscountService(DiscountContext discountContext, IMapper mapper, ILogger<DiscountService> logger)
    {
        _discountContext = discountContext;
        _mapper = mapper;
        _logger = logger;
    }

    public override async Task<DiscountModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var discount = await _discountContext.Discounts.FindAsync(request.DiscountCode);

        _logger.LogInformation("Discount code: {discountCode}; Amount: {discountAmount}", discount?.Code, discount?.Amount);

        return _mapper.Map<DiscountModel>(discount);
    }
}