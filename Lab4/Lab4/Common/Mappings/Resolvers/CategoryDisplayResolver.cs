namespace Lab4.Common.Mappings.Resolvers;
using AutoMapper;
using Lab4.Features.Orders;
using Lab4.Features.Orders.Dtos;
using Lab4.Persistence.Domain;
public class CategoryDisplayResolver :IValueResolver<Order,OrderProfileDto,string>
{
    public string Resolve(Order source, OrderProfileDto destionation, string destMember, ResolutionContext context)
    {
        return source.Category switch
        {
            OrderCategory.Fiction => "Fiction & Literature",
            OrderCategory.NonFiction => "Non-Fiction",
            OrderCategory.Technical => "Technical & Professional",
            OrderCategory.Children => "Children's Orders",
            _ => "Uncategorized"
        };
    }
}