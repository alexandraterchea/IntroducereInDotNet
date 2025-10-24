using AutoMapper;
using Lab4.Features.Orders.Dtos;
using Lab4.Persistence.Domain;

namespace Lab4.Common.Mappings.Resolvers
{
    public class AuthorInitialsResolver : IValueResolver<Order, OrderProfileDto, string>
    {
        public string Resolve(Order source, OrderProfileDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source.Author))
                return "?";

            var parts = source.Author.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0][0].ToString().ToUpper();

            return $"{char.ToUpper(parts.First()[0])}{char.ToUpper(parts.Last()[0])}";
        }
    }
}