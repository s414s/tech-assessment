using BookingSystem.Application.Handlers.Bookings;
using BookingSystem.Application.Handlers.Items;
using BookingSystem.Domain.Contracts.Services;
using BookingSystem.Infrastructure;
using BookingSystem.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Presentation.Endpoints;
using BookingSystem.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

// Logs every request (verb, full URL, body) and response (status code, body), combined into one entry per request
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod
        | HttpLoggingFields.RequestBody
        | HttpLoggingFields.ResponseStatusCode
        | HttpLoggingFields.ResponseBody;
    options.CombineLogs = true;
});
builder.Services.AddHttpLoggingInterceptor<FullUrlLoggingInterceptor>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<IHandler<GetFilteredItemsQuery, GetFilteredItemsResponse>, GetFilteredItemsHandler>();
builder.Services.AddScoped<IHandler<GetFilteredBookingsQuery, GetFilteredBookingsResponse>, GetFilteredBookingsHandler>();
builder.Services.AddScoped<IHandler<GetItemBookingsQuery, GetItemBookingsResponse>, GetItemBookingsHandler>();
builder.Services.AddScoped<IHandler<CreateBookingCommand, CreateBookingResponse>, CreateBookingHandler>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Apply pending migrations (and seed data) on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    await db.Database.MigrateAsync();
}

// Before the exception handler so error responses are logged too
app.UseHttpLogging();
app.UseExceptionHandler();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapUsersEndpoints();
app.MapLocationsEndpoints();
app.MapItemsEndpoints();
app.MapBookingsEndpoints();

app.Run();
