using MediatR;
using BookApi.Common;

namespace BookApi.Features.Books.Commands
{
    public class CreateBookCommand : IRequest<BookDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}