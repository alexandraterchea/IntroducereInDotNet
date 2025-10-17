using MediatR;
using BookApi.Persistence;
using BookApi.Persistence.Domain;
using BookApi.Features.Books.Queries;
using BookApi.Common;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Features.Books.Handlers
{
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, PaginatedResult<BookDto>>
    {
        private readonly BookDbContext _context;

        public GetAllBooksQueryHandler(BookDbContext context) => _context = context;

        public async Task<PaginatedResult<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var validPage = Math.Max(1, request.Page);
            var validPageSize = Math.Min(Math.Max(1, request.PageSize), 100);

            var totalCount = await _context.Books.CountAsync(cancellationToken);
            var books = await _context.Books
                .Skip((validPage - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<BookDto>
            {
                Items = books.Select(MapToDto).ToList(),
                Page = validPage,
                PageSize = validPageSize,
                TotalCount = totalCount
            };
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