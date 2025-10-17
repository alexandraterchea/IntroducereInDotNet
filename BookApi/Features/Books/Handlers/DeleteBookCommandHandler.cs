using MediatR;
using BookApi.Persistence;
using BookApi.Features.Books.Commands;

namespace BookApi.Features.Books.Handlers
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, bool>
    {
        private readonly BookDbContext _context;

        public DeleteBookCommandHandler(BookDbContext context) => _context = context;

        public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _context.Books.FindAsync(new object[] { request.Id }, cancellationToken);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}