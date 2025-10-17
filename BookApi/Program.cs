using Microsoft.EntityFrameworkCore;
using BookApi.Persistence;
using BookApi.Features.Books.Commands;
using BookApi.Features.Books.Queries;
using BookApi.Features.Books.Handlers;
using BookApi.Validators;
using BookApi.Common;
using MediatR;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<BookDbContext>(opt =>
    opt.UseSqlite("Data Source=books.db"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseCors();



var group = app.MapGroup("/books").WithName("Books").WithOpenApi();

group.MapPost("/", CreateBook)
    .WithName("CreateBook")
    .WithOpenApi()
    .Produces<BookDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

group.MapGet("/{id}", GetBookById)
    .WithName("GetBookById")
    .WithOpenApi()
    .Produces<BookDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

group.MapGet("/", GetAllBooks)
    .WithName("GetAllBooks")
    .WithOpenApi()
    .Produces<PaginatedResult<BookDto>>(StatusCodes.Status200OK);

group.MapPut("/{id}", UpdateBook)
    .WithName("UpdateBook")
    .WithOpenApi()
    .Produces<BookDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest);

group.MapDelete("/{id}", DeleteBook)
    .WithName("DeleteBook")
    .WithOpenApi()
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);

app.Run();

async Task<IResult> CreateBook(CreateBookCommand command, IMediator mediator)
{
    try
    {
        var result = await mediator.Send(command);
        return Results.Created($"/books/{result.Id}", result);
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
    }
}

async Task<IResult> GetBookById(int id, IMediator mediator)
{
    var query = new GetBookByIdQuery { Id = id };
    var result = await mediator.Send(query);
    return result != null ? Results.Ok(result) : Results.NotFound();
}

async Task<IResult> GetAllBooks(int page = 1, int pageSize = 10, IMediator mediator = null!)
{
    var query = new GetAllBooksQuery { Page = page, PageSize = pageSize };
    var result = await mediator.Send(query);
    return Results.Ok(result);
}

async Task<IResult> UpdateBook(int id, UpdateBookCommand command, IMediator mediator)
{
    try
    {
        command.Id = id;
        var result = await mediator.Send(command);
        return result != null ? Results.Ok(result) : Results.NotFound();
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
    }
}

async Task<IResult> DeleteBook(int id, IMediator mediator)
{
    var command = new DeleteBookCommand { Id = id };
    var result = await mediator.Send(command);
    return result ? Results.NoContent() : Results.NotFound();
}