using AutoMapper;
using Defence.In.Depth.DataContracts;
using Defence.In.Depth.Domain.Model;
using Defence.In.Depth.Infrastructure.Entities;

namespace Defence.In.Depth
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDataContract>();
            CreateMap<ProductEntity, Product>();

            CreateMap<string, ProductId>().ConstructUsing(source => new ProductId(source));
            CreateMap<string, ProductName>().ConstructUsing(source => new ProductName(source));
            CreateMap<string, MarketId>().ConstructUsing(source => new MarketId(source));

            CreateMap<IDomainPrimitive<string>, string>().ConstructUsing(source => source.Value);
        }        
    }
}