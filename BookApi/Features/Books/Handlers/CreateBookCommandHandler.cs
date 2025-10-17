using MediatR;
using BookApi.Persistence;
using BookApi.Persistence.Domain;
using BookApi.Features.Books.Commands;
using BookApi.Common;

namespace BookApi.Features.Books.Handlers
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
    {
        private readonly BookDbContext _context;

        public CreateBookCommandHandler(BookDbContext context) => _context = context;

        public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var book = new Book
            {
                Title = request.Title,
                Author = request.Author,
                Year = request.Year
            };

            _context.Books.Add(book);
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