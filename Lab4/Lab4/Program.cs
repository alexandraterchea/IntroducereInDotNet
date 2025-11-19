using AutoMapper;
using Lab4.Common.Mappings;
using Lab4.Features.Orders.Handlers;
using Lab4.Common.Middleware;
using MediatR ;
using Microsoft.Extensions.Caching.Memory;
using FluentValidation;
using FluentValidation.AspNetCore;
using Lab4.Features.Orders.Requests;
using Lab4.Validators;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddAutoMapper(typeof(AdvancedOrderMappingProfile).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderHandler).Assembly));

builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderProfileValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddTransient<CorrelationMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<CorrelationMiddleware>();
app.MapControllers();

app.Run();

