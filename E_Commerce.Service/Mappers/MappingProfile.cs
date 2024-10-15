using AutoMapper;
using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.Product;
using E_Commerce.Service.DTOs.PromoCode;
using E_Commerce.Service.DTOs.User;
namespace E_Commerce.Service.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductUpdateDto>().ReverseMap();
        CreateMap<Product, ProductCreateDto>().ReverseMap();
        CreateMap<User, UserUpdateDto>().ReverseMap();
        CreateMap<User, UserCreateDto>().ReverseMap();

        CreateMap<PromoCode, PromoCreateDto>().ReverseMap();
        CreateMap<PromoCode, PromoUpdateDto>().ReverseMap();
    }
}
