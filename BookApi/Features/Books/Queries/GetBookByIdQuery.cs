using MediatR;
using BookApi.Common;

namespace BookApi.Features.Books.Queries
{
    public class GetBookByIdQuery : IRequest<BookDto?>
    {
        public int Id { get; set; }
    }
}