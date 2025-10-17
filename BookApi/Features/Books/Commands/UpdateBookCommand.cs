using MediatR;
using BookApi.Common;

namespace BookApi.Features.Books.Commands
{
    public class UpdateBookCommand : IRequest<BookDto?>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Year { get; set; }
    }
}