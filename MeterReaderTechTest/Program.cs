using AutoMapper;
using MeterReaderTechTest.Data;
using MeterReaderTechTest.DTOs;
using MeterReaderTechTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<CsvSeeder>();
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// Seed data before starting
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<CsvSeeder>();
    var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Test_Accounts.csv");
    seeder.SeedAccounts(path);
}

app.MapGet("/", () => "Hello, Meter Reader API!");

app.MapGet("/accounts", async (AppDbContext db, [FromServices] IMapper mapper) =>
{
    var accounts = await db.Accounts.Include(a => a.MeterReadings).ToListAsync();

    var accountDtos = mapper.Map<List<AccountDto>>(accounts);

    return Results.Ok(accountDtos);
});

app.Run();
