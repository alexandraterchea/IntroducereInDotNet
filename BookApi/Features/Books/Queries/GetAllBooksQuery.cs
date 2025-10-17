using MediatR;
using BookApi.Common;

namespace BookApi.Features.Books.Queries
{
    public class GetAllBooksQuery : IRequest<PaginatedResult<BookDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}