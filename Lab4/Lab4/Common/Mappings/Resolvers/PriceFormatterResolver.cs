using AutoMapper;
using Lab4.Features.Orders.Dtos;
using Lab4.Persistence.Domain;

namespace Lab4.Common.Mappings.Resolvers
{
    public class PriceFormatterResolver : IValueResolver<Order, OrderProfileDto, string>
    {
        public string Resolve(Order source, OrderProfileDto destination, string destMember, ResolutionContext context)
        {
            return source.Price.ToString("C2");
        }
    }
}