using AutoMapper;
using DiscountGrpc.Model;
using DiscountGrpc.Protos;

namespace DiscountGrpc.Mapping;

public class DiscountProfile: Profile
{
    public DiscountProfile()
    {
        CreateMap<Discount, DiscountModel>().ReverseMap();
    }
}
