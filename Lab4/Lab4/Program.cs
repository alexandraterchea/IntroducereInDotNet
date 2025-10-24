using AutoMapper;
using Lab4.Common.Mappings;
using Lab4.Features.Orders.Handlers;
using MediatR ;
using Microsoft.Extensions.Caching.Memory;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddAutoMapper(typeof(AdvancedOrderMappingProfile).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderHandler).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

