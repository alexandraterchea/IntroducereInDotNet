using MediatR;
using BookApi.Persistence;
using BookApi.Persistence.Domain;
using BookApi.Features.Books.Queries;
using BookApi.Common;

namespace BookApi.Features.Books.Handlers
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
    {
        private readonly BookDbContext _context;

        public GetBookByIdQueryHandler(BookDbContext context) => _context = context;

        public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _context.Books.FindAsync(new object[] { request.Id }, cancellationToken);
            return book != null ? MapToDto(book) : null;
        }

        private static BookDto MapToDto(Book book) => new()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Year = book.Year
        };
    }
}