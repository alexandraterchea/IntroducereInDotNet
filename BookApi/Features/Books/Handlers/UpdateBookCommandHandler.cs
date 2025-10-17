using MediatR;
using BookApi.Persistence;
using BookApi.Persistence.Domain;
using BookApi.Features.Books.Commands;
using BookApi.Common;

namespace BookApi.Features.Books.Handlers
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto?>
    {
        private readonly BookDbContext _context;

        public UpdateBookCommandHandler(BookDbContext context) => _context = context;

        public async Task<BookDto?> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _context.Books.FindAsync(new object[] { request.Id }, cancellationToken);
            if (book == null) return null;

            book.Title = request.Title;
            book.Author = request.Author;
            book.Year = request.Year;

            _context.Books.Update(book);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(book);
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